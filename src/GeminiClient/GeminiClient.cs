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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using GoogleGenAI.Client.Configuration;
using GoogleGenAI.Client.Services;

namespace GoogleGenAI.Client;

/// <summary>
/// Main client for Google Generative AI APIs.
/// Provides access to all available services and maintains consistent configuration.
/// </summary>
public class GeminiClient : IGeminiClient
{
    private readonly IApiClient _apiClient;
    private readonly GeminiClientOptions _options;
    private readonly ILogger<GeminiClient> _logger;
    private readonly IServiceProvider? _serviceProvider;
    
    // Service instances
    private readonly Lazy<IModelsService> _models;
    private readonly Lazy<IChatsService> _chats;
    private readonly Lazy<IFilesService> _files;
    private readonly Lazy<ICachesService> _caches;
    private readonly Lazy<IBatchesService> _batches;
    private readonly Lazy<ITuningsService> _tunings;
    private readonly Lazy<ITokensService> _authTokens;
    private readonly Lazy<IOperationsService> _operations;

    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the GeminiClient.
    /// </summary>
    /// <param name="apiClient">The underlying API client for making HTTP requests.</param>
    /// <param name="options">Configuration options for the client.</param>
    /// <param name="logger">Logger instance for debugging and diagnostics.</param>
    /// <param name="serviceProvider">Service provider for dependency injection (optional).</param>
    public GeminiClient(
        IApiClient apiClient,
        IOptions<GeminiClientOptions> options,
        ILogger<GeminiClient> logger,
        IServiceProvider? serviceProvider = null)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider;

        // Validate configuration
        _options.Validate();

        // Initialize services lazily to avoid circular dependencies
        _models = new Lazy<IModelsService>(() => 
            _serviceProvider?.GetService<IModelsService>() ?? 
            new ModelsService(_apiClient, options, CreateLogger<ModelsService>()));

        _chats = new Lazy<IChatsService>(() => 
            _serviceProvider?.GetService<IChatsService>() ??
            new ChatsService(_apiClient, options, CreateLogger<ChatsService>(), Models));

        _files = new Lazy<IFilesService>(() => 
            _serviceProvider?.GetService<IFilesService>() ??
            new FilesService(_apiClient, options, CreateLogger<FilesService>()));

        _caches = new Lazy<ICachesService>(() => 
            _serviceProvider?.GetService<ICachesService>() ??
            new CachesService(_apiClient, options, CreateLogger<CachesService>()));

        _batches = new Lazy<IBatchesService>(() => 
            _serviceProvider?.GetService<IBatchesService>() ??
            new BatchesService(_apiClient, options, CreateLogger<BatchesService>()));

        _tunings = new Lazy<ITuningsService>(() => 
            _serviceProvider?.GetService<ITuningsService>() ??
            new TuningsService(_apiClient, options, CreateLogger<TuningsService>()));

        _authTokens = new Lazy<ITokensService>(() => 
            _serviceProvider?.GetService<ITokensService>() ??
            new TokensService(_apiClient, options, CreateLogger<TokensService>()));

        _operations = new Lazy<IOperationsService>(() => 
            _serviceProvider?.GetService<IOperationsService>() ??
            new OperationsService(_apiClient, options, CreateLogger<OperationsService>()));

        _logger.LogInformation("GeminiClient initialized with {ApiType} API", 
            IsVertexAI ? "Vertex AI" : "Gemini Developer");
    }

    private ILogger<T> CreateLogger<T>()
    {
        if (_serviceProvider != null)
        {
            var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
            if (loggerFactory != null)
            {
                return loggerFactory.CreateLogger<T>();
            }
        }

        // Fallback logger
        var factory = LoggerFactory.Create(builder => builder.AddConsole());
        return factory.CreateLogger<T>();
    }

    /// <summary>
    /// Access to model operations (list models, get model info, generate content).
    /// </summary>
    public IModelsService Models => _models.Value;

    /// <summary>
    /// Access to chat operations (generate content, chat sessions).
    /// </summary>
    public IChatsService Chats => _chats.Value;

    /// <summary>
    /// Access to file operations (upload, list, delete files).
    /// </summary>
    public IFilesService Files => _files.Value;

    /// <summary>
    /// Access to cache operations (create, list, delete caches).
    /// </summary>
    public ICachesService Caches => _caches.Value;

    /// <summary>
    /// Access to batch operations (create and manage batch jobs).
    /// </summary>
    public IBatchesService Batches => _batches.Value;

    /// <summary>
    /// Access to tuning operations (fine-tuning models).
    /// </summary>
    public ITuningsService Tunings => _tunings.Value;

    /// <summary>
    /// Access to authentication token operations.
    /// </summary>
    public ITokensService AuthTokens => _authTokens.Value;

    /// <summary>
    /// Access to long-running operations.
    /// </summary>
    public IOperationsService Operations => _operations.Value;

    /// <summary>
    /// Indicates whether this client is using Vertex AI API endpoints.
    /// </summary>
    public bool IsVertexAI => _options.GetUseVertexAI();

    /// <summary>
    /// Creates a quick chat session for simple conversations.
    /// </summary>
    /// <param name="modelName">Name of the model to use (e.g., "gemini-1.5-flash").</param>
    /// <param name="systemInstruction">Optional system instruction to guide the model's behavior.</param>
    /// <returns>A new chat session.</returns>
    public IChatSession CreateChatSession(string modelName, string? systemInstruction = null)
    {
        ThrowIfDisposed();

        var config = new Models.GenerateContentConfig();
        if (!string.IsNullOrEmpty(systemInstruction))
        {
            config.SystemInstruction = new Models.Content
            {
                Role = "system",
                Parts = new List<Models.Part>
                {
                    new() { Text = systemInstruction }
                }
            };
        }

        return Chats.CreateSession(modelName, config);
    }

    /// <summary>
    /// Quickly generates content without maintaining conversation history.
    /// </summary>
    /// <param name="modelName">Name of the model to use.</param>
    /// <param name="prompt">The text prompt to send to the model.</param>
    /// <param name="maxTokens">Maximum number of tokens to generate.</param>
    /// <param name="temperature">Temperature for randomness (0.0 to 2.0).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated response.</returns>
    public async Task<string> GenerateTextAsync(
        string modelName, 
        string prompt, 
        int? maxTokens = null, 
        float? temperature = null,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var config = new Models.GenerateContentConfig();
        if (maxTokens.HasValue || temperature.HasValue)
        {
            config.GenerationConfig = new Models.GenerationConfig
            {
                MaxOutputTokens = maxTokens,
                Temperature = temperature
            };
        }

        var contents = new[]
        {
            new Models.Content
            {
                Role = "user",
                Parts = new List<Models.Part>
                {
                    new() { Text = prompt }
                }
            }
        };

        var response = await Models.GenerateContentAsync(modelName, contents, config, cancellationToken);
        return response.Text ?? throw new InvalidOperationException("No text content in response");
    }

    /// <summary>
    /// Quickly generates content with streaming response.
    /// </summary>
    /// <param name="modelName">Name of the model to use.</param>
    /// <param name="prompt">The text prompt to send to the model.</param>
    /// <param name="maxTokens">Maximum number of tokens to generate.</param>
    /// <param name="temperature">Temperature for randomness (0.0 to 2.0).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of response chunks.</returns>
    public async IAsyncEnumerable<string> GenerateTextStreamAsync(
        string modelName, 
        string prompt, 
        int? maxTokens = null, 
        float? temperature = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var config = new Models.GenerateContentConfig();
        if (maxTokens.HasValue || temperature.HasValue)
        {
            config.GenerationConfig = new Models.GenerationConfig
            {
                MaxOutputTokens = maxTokens,
                Temperature = temperature
            };
        }

        var contents = new[]
        {
            new Models.Content
            {
                Role = "user",
                Parts = new List<Models.Part>
                {
                    new() { Text = prompt }
                }
            }
        };

        await foreach (var response in Models.GenerateContentStreamAsync(modelName, contents, config, cancellationToken))
        {
            var text = response.Text;
            if (!string.IsNullOrEmpty(text))
            {
                yield return text;
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(GeminiClient));
        }
    }

    /// <summary>
    /// Disposes the client and releases all resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _apiClient?.Dispose();
            _disposed = true;
            _logger.LogInformation("GeminiClient disposed");
        }
    }
}