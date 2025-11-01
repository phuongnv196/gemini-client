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

using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GoogleGenAI.Client.Configuration;
using GoogleGenAI.Client.Models;

namespace GoogleGenAI.Client.Services;

/// <summary>
/// Service for model operations like listing models and generating content.
/// </summary>
public class ModelsService : IModelsService
{
    private readonly IApiClient _apiClient;
    private readonly GeminiClientOptions _options;
    private readonly ILogger<ModelsService> _logger;

    public ModelsService(IApiClient apiClient, IOptions<GeminiClientOptions> options, ILogger<ModelsService> logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Model>> ListAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing available models");
        
        var endpoint = _options.GetUseVertexAI() 
            ? $"/v1/projects/{_options.GetProjectId()}/locations/{_options.GetLocation()}/publishers/google/models"
            : $"/{_options.ApiVersion}/models";

        var response = await _apiClient.GetAsync<ModelsListResponse>(endpoint, cancellationToken);
        return response.Models ?? Enumerable.Empty<Model>();
    }

    public async Task<Model> GetAsync(string modelName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentException("Model name cannot be null or empty", nameof(modelName));

        _logger.LogDebug("Getting model information for {ModelName}", modelName);

        var endpoint = _options.GetUseVertexAI()
            ? $"/v1/projects/{_options.GetProjectId()}/locations/{_options.GetLocation()}/publishers/google/models/{modelName}"
            : $"/{_options.ApiVersion}/models/{modelName}";

        return await _apiClient.GetAsync<Model>(endpoint, cancellationToken);
    }

    public async Task<GenerateContentResponse> GenerateContentAsync(
        string modelName,
        IEnumerable<Content> contents,
        GenerateContentConfig? config = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentException("Model name cannot be null or empty", nameof(modelName));

        if (contents == null || !contents.Any())
            throw new ArgumentException("Contents cannot be null or empty", nameof(contents));

        _logger.LogDebug("Generating content with model {ModelName}", modelName);

        var request = new GenerateContentRequest
        {
            Contents = contents.ToList(),
            GenerationConfig = config?.GenerationConfig,
            SafetySettings = config?.SafetySettings ?? new List<SafetySetting>(),
            Tools = config?.Tools,
            ToolConfig = config?.ToolConfig,
            SystemInstruction = config?.SystemInstruction,
            CachedContent = config?.CachedContent
        };

        var endpoint = _options.GetUseVertexAI()
            ? $"/v1/projects/{_options.GetProjectId()}/locations/{_options.GetLocation()}/publishers/google/models/{modelName}:generateContent"
            : $"/{_options.ApiVersion}/models/{modelName}:generateContent";

        return await _apiClient.PostAsync<GenerateContentRequest, GenerateContentResponse>(endpoint, request, cancellationToken);
    }

    public async IAsyncEnumerable<GenerateContentResponse> GenerateContentStreamAsync(
        string modelName,
        IEnumerable<Content> contents,
        GenerateContentConfig? config = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentException("Model name cannot be null or empty", nameof(modelName));

        if (contents == null || !contents.Any())
            throw new ArgumentException("Contents cannot be null or empty", nameof(contents));

        _logger.LogDebug("Streaming content generation with model {ModelName}", modelName);

        var request = new GenerateContentRequest
        {
            Contents = contents.ToList(),
            GenerationConfig = config?.GenerationConfig,
            SafetySettings = config?.SafetySettings ?? new List<SafetySetting>(),
            Tools = config?.Tools,
            ToolConfig = config?.ToolConfig,
            SystemInstruction = config?.SystemInstruction,
            CachedContent = config?.CachedContent
        };

        var endpoint = _options.GetUseVertexAI()
            ? $"/v1/projects/{_options.GetProjectId()}/locations/{_options.GetLocation()}/publishers/google/models/{modelName}:streamGenerateContent"
            : $"/{_options.ApiVersion}/models/{modelName}:streamGenerateContent";

        await foreach (var response in _apiClient.PostStreamAsync<GenerateContentRequest, GenerateContentResponse>(endpoint, request, cancellationToken))
        {
            yield return response;
        }
    }
}

// Additional helper classes for API requests/responses

public class ModelsListResponse
{
    [JsonPropertyName("models")]
    public List<Model>? Models { get; set; }
    
    [JsonPropertyName("next_page_token")]
    public string? NextPageToken { get; set; }
}

public class GenerateContentRequest
{
    [JsonPropertyName("contents")]
    public List<Content> Contents { get; set; } = new();
    
    [JsonPropertyName("generation_config")]
    public GenerationConfig? GenerationConfig { get; set; }
    
    [JsonPropertyName("safety_settings")]
    public List<SafetySetting> SafetySettings { get; set; } = new();
    
    [JsonPropertyName("tools")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Tool>? Tools { get; set; }
    
    [JsonPropertyName("tool_config")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ToolConfig? ToolConfig { get; set; }
    
    [JsonPropertyName("system_instruction")]
    public Content? SystemInstruction { get; set; }
    
    [JsonPropertyName("cached_content")]
    public string? CachedContent { get; set; }
}
