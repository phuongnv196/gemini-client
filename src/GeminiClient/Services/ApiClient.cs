// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GoogleGenAI.Client.Configuration;
using GoogleGenAI.Client.Exceptions;
using GoogleGenAI.Client.Models;

namespace GoogleGenAI.Client.Services;

/// <summary>
/// Low-level API client for making HTTP requests to Google Generative AI APIs.
/// </summary>
public interface IApiClient : IDisposable
{
    /// <summary>
    /// Makes a POST request to the specified endpoint.
    /// </summary>
    Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Makes a GET request to the specified endpoint.
    /// </summary>
    Task<TResponse> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Makes a POST request with streaming response.
    /// </summary>
    IAsyncEnumerable<TResponse> PostStreamAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Default implementation of the API client.
/// </summary>
public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly GeminiClientOptions _options;
    private readonly ILogger<ApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed = false;

    public ApiClient(HttpClient httpClient, IOptions<GeminiClientOptions> options, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        _options.Validate();

        _httpClient.BaseAddress = new Uri(_options.GetBaseUrl());
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        
        // Set user agent
        _httpClient.DefaultRequestHeaders.UserAgent.Clear();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_options.UserAgent);

        // Configure authentication
        if (!_options.GetUseVertexAI())
        {
            // Gemini Developer API uses API key
            var apiKey = _options.GetApiKey();
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);
            }
        }
        // For Vertex AI, authentication is handled by Google.Apis.Auth
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var requestJson = JsonSerializer.Serialize(request, _jsonOptions);
        _logger.LogDebug("Making POST request to {Endpoint} with payload: {Request}", endpoint, requestJson);

        using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        
        var response = await ExecuteWithRetryAsync(async () =>
        {
            return await _httpClient.PostAsync(endpoint, content, cancellationToken);
        }, cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogDebug("Received response from {Endpoint}: {Response}", endpoint, responseContent);

        await HandleErrorResponseAsync(response, responseContent);

        try
        {
            var result = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
            return result ?? throw new GeminiException($"Failed to deserialize response of type {typeof(TResponse).Name}");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response from {Endpoint}", endpoint);
            throw new GeminiException($"Failed to deserialize response: {ex.Message}", ex);
        }
    }

    public async Task<TResponse> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        _logger.LogDebug("Making GET request to {Endpoint}", endpoint);

        var response = await ExecuteWithRetryAsync(async () =>
        {
            return await _httpClient.GetAsync(endpoint, cancellationToken);
        }, cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogDebug("Received response from {Endpoint}: {Response}", endpoint, responseContent);

        await HandleErrorResponseAsync(response, responseContent);

        try
        {
            var result = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
            return result ?? throw new GeminiException($"Failed to deserialize response of type {typeof(TResponse).Name}");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response from {Endpoint}", endpoint);
            throw new GeminiException($"Failed to deserialize response: {ex.Message}", ex);
        }
    }

    public async IAsyncEnumerable<TResponse> PostStreamAsync<TRequest, TResponse>(
        string endpoint, 
        TRequest request, 
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var requestJson = JsonSerializer.Serialize(request, _jsonOptions);
        _logger.LogDebug("Making streaming POST request to {Endpoint} with payload: {Request}", endpoint, requestJson);

        using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        
        var response = await ExecuteWithRetryAsync(async () =>
        {
            return await _httpClient.PostAsync(endpoint, content, cancellationToken);
        }, cancellationToken);

        //await HandleErrorResponseAsync(response, string.Empty);

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        string? line;
        while ((line = await reader.ReadLineAsync()) != null && !cancellationToken.IsCancellationRequested)
        {
            if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data: "))
                continue;

            var jsonData = line[6..]; // Remove "data: " prefix
            if (jsonData == "[DONE]")
                break;

            TResponse? streamResponse;
            try
            {
                streamResponse = JsonSerializer.Deserialize<TResponse>(jsonData, _jsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize streaming response chunk: {JsonData}", jsonData);
                continue;
            }

            if (streamResponse != null)
            {
                yield return streamResponse;
            }
        }
    }

    private async Task<HttpResponseMessage> ExecuteWithRetryAsync(
        Func<Task<HttpResponseMessage>> operation,
        CancellationToken cancellationToken)
    {
        var attempt = 0;
        var delay = TimeSpan.FromMilliseconds(_options.RetryDelayMilliseconds);

        while (attempt <= _options.MaxRetryAttempts)
        {
            try
            {
                var response = await operation();
                
                if (ShouldRetry(response))
                {
                    response.Dispose();
                    
                    if (attempt == _options.MaxRetryAttempts)
                    {
                        throw new GeminiException($"Request failed after {_options.MaxRetryAttempts} retry attempts");
                    }

                    _logger.LogWarning("Request failed with status {StatusCode}, retrying in {Delay}ms (attempt {Attempt}/{MaxAttempts})",
                        response.StatusCode, delay.TotalMilliseconds, attempt + 1, _options.MaxRetryAttempts);

                    await Task.Delay(delay, cancellationToken);
                    
                    if (_options.UseExponentialBackoff)
                    {
                        delay = TimeSpan.FromMilliseconds(Math.Min(
                            delay.TotalMilliseconds * 2,
                            _options.MaxRetryDelayMilliseconds));
                    }

                    attempt++;
                    continue;
                }

                return response;
            }
            catch (HttpRequestException ex) when (attempt < _options.MaxRetryAttempts)
            {
                _logger.LogWarning(ex, "Network error occurred, retrying in {Delay}ms (attempt {Attempt}/{MaxAttempts})",
                    delay.TotalMilliseconds, attempt + 1, _options.MaxRetryAttempts);

                await Task.Delay(delay, cancellationToken);
                
                if (_options.UseExponentialBackoff)
                {
                    delay = TimeSpan.FromMilliseconds(Math.Min(
                        delay.TotalMilliseconds * 2,
                        _options.MaxRetryDelayMilliseconds));
                }

                attempt++;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                throw new GeminiTimeoutException("Request timed out", ex);
            }
        }

        throw new GeminiException($"Request failed after {_options.MaxRetryAttempts} retry attempts");
    }

    private static bool ShouldRetry(HttpResponseMessage response)
    {
        return response.StatusCode == HttpStatusCode.InternalServerError ||
               response.StatusCode == HttpStatusCode.BadGateway ||
               response.StatusCode == HttpStatusCode.ServiceUnavailable ||
               response.StatusCode == HttpStatusCode.GatewayTimeout ||
               response.StatusCode == HttpStatusCode.TooManyRequests;
    }

    private static Task HandleErrorResponseAsync(HttpResponseMessage response, string responseContent)
    {
        if (response.IsSuccessStatusCode)
            return Task.CompletedTask;

        var statusCode = response.StatusCode;
        var errorMessage = $"Request failed with status {(int)statusCode} ({statusCode})";

        if (!string.IsNullOrEmpty(responseContent))
        {
            try
            {
                using var doc = JsonDocument.Parse(responseContent);
                if (doc.RootElement.TryGetProperty("error", out var errorElement))
                {
                    if (errorElement.TryGetProperty("message", out var messageElement))
                    {
                        errorMessage = messageElement.GetString() ?? errorMessage;
                    }
                }
            }
            catch (JsonException)
            {
                // If we can't parse the error response, use the raw content
                errorMessage = responseContent;
            }
        }

        throw statusCode switch
        {
            HttpStatusCode.Unauthorized => new GeminiAuthenticationException(errorMessage),
            HttpStatusCode.Forbidden => new GeminiForbiddenException(errorMessage),
            HttpStatusCode.NotFound => new GeminiNotFoundException(errorMessage),
            HttpStatusCode.BadRequest => new GeminiBadRequestException(errorMessage),
            HttpStatusCode.TooManyRequests => new GeminiRateLimitException(errorMessage, GetRetryAfter(response)),
            _ when ((int)statusCode >= 500) => new GeminiServerException(errorMessage, statusCode),
            _ => new GeminiException(errorMessage, "UNKNOWN_ERROR", statusCode)
        };
    }

    private static DateTimeOffset? GetRetryAfter(HttpResponseMessage response)
    {
        if (response.Headers.RetryAfter?.Delta.HasValue == true)
        {
            return DateTimeOffset.UtcNow.Add(response.Headers.RetryAfter.Delta.Value);
        }

        if (response.Headers.RetryAfter?.Date.HasValue == true)
        {
            return response.Headers.RetryAfter.Date.Value;
        }

        return null;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ApiClient));
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient?.Dispose();
            _disposed = true;
        }
    }
}
