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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using GoogleGenAI.Client.Configuration;
using GoogleGenAI.Client.Services;

namespace GoogleGenAI.Client.Extensions;

/// <summary>
/// Extension methods for registering Google Generative AI client services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Google Generative AI client services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="configurationSection">The configuration section name (defaults to "GeminiClient").</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGeminiClient(
        this IServiceCollection services,
        IConfiguration configuration,
        string configurationSection = GeminiClientOptions.SectionName)
    {
        // Configure options
        services.Configure<GeminiClientOptions>(configuration.GetSection(configurationSection));

        // Register HTTP client
        services.AddHttpClient<IApiClient, ApiClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<GeminiClientOptions>>().Value;
            options.Validate();

            client.BaseAddress = new Uri(options.GetBaseUrl());
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);

            // Configure authentication for Gemini Developer API
            if (!options.GetUseVertexAI())
            {
                var apiKey = options.GetApiKey();
                if (!string.IsNullOrEmpty(apiKey))
                {
                    //client.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);
                }
            }
        });

        // Register services
        services.AddScoped<IModelsService, ModelsService>();
        services.AddScoped<IChatsService, ChatsService>();
        services.AddScoped<IFilesService, FilesService>();
        services.AddScoped<ICachesService, CachesService>();
        services.AddScoped<IBatchesService, BatchesService>();
        services.AddScoped<ITuningsService, TuningsService>();
        services.AddScoped<ITokensService, TokensService>();
        services.AddScoped<IOperationsService, OperationsService>();

        // Register main client
        services.AddScoped<IGeminiClient, GeminiClient>();

        // Add logging
        services.AddLogging();

        return services;
    }

    /// <summary>
    /// Adds Google Generative AI client services with custom configuration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">Action to configure the client options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGeminiClient(
        this IServiceCollection services,
        Action<GeminiClientOptions> configureOptions)
    {
        // Configure options
        services.Configure(configureOptions);

        return services.AddGeminiClientCore();
    }

    /// <summary>
    /// Adds Google Generative AI client services with API key configuration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="apiKey">The API key for Gemini Developer API.</param>
    /// <param name="configureOptions">Optional action to configure additional options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGeminiClient(
        this IServiceCollection services,
        string apiKey,
        Action<GeminiClientOptions>? configureOptions = null)
    {
        services.Configure<GeminiClientOptions>(options =>
        {
            options.ApiKey = apiKey;
            configureOptions?.Invoke(options);
        });

        return services.AddGeminiClientCore();
    }

    /// <summary>
    /// Adds Google Generative AI client services for Vertex AI.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="projectId">The Google Cloud project ID.</param>
    /// <param name="location">The location for API requests (e.g., "us-central1").</param>
    /// <param name="configureOptions">Optional action to configure additional options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGeminiClientForVertexAI(
        this IServiceCollection services,
        string projectId,
        string location,
        Action<GeminiClientOptions>? configureOptions = null)
    {
        services.Configure<GeminiClientOptions>(options =>
        {
            options.UseVertexAI = true;
            options.ProjectId = projectId;
            options.Location = location;
            configureOptions?.Invoke(options);
        });

        return services.AddGeminiClientCore();
    }

    private static IServiceCollection AddGeminiClientCore(this IServiceCollection services)
    {
        // Register HTTP client
        services.AddHttpClient<IApiClient, ApiClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<GeminiClientOptions>>().Value;
            options.Validate();

            client.BaseAddress = new Uri(options.GetBaseUrl());
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);

            // Configure authentication for Gemini Developer API
            if (!options.GetUseVertexAI())
            {
                var apiKey = options.GetApiKey();
                if (!string.IsNullOrEmpty(apiKey))
                {
                    //client.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);
                }
            }
        });

        // Register services
        services.AddScoped<IModelsService, ModelsService>();
        services.AddScoped<IChatsService, ChatsService>();
        services.AddScoped<IFilesService, FilesService>();
        services.AddScoped<ICachesService, CachesService>();
        services.AddScoped<IBatchesService, BatchesService>();
        services.AddScoped<ITuningsService, TuningsService>();
        services.AddScoped<ITokensService, TokensService>();
        services.AddScoped<IOperationsService, OperationsService>();

        // Register main client
        services.AddScoped<IGeminiClient, GeminiClient>();

        // Add logging
        services.AddLogging();

        return services;
    }
}

/// <summary>
/// Extension methods for creating GeminiClient instances without dependency injection.
/// </summary>
public static class GeminiClientFactory
{
    /// <summary>
    /// Creates a new GeminiClient instance with API key for Gemini Developer API.
    /// </summary>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="configureOptions">Optional action to configure additional options.</param>
    /// <returns>A new GeminiClient instance.</returns>
    public static IGeminiClient Create(string apiKey, Action<GeminiClientOptions>? configureOptions = null)
    {
        var options = new GeminiClientOptions
        {
            ApiKey = apiKey
        };

        configureOptions?.Invoke(options);

        return CreateClient(options);
    }

    /// <summary>
    /// Creates a new GeminiClient instance for Vertex AI.
    /// </summary>
    /// <param name="projectId">The Google Cloud project ID.</param>
    /// <param name="location">The location for API requests.</param>
    /// <param name="configureOptions">Optional action to configure additional options.</param>
    /// <returns>A new GeminiClient instance.</returns>
    public static IGeminiClient CreateForVertexAI(
        string projectId,
        string location,
        Action<GeminiClientOptions>? configureOptions = null)
    {
        var options = new GeminiClientOptions
        {
            UseVertexAI = true,
            ProjectId = projectId,
            Location = location
        };

        configureOptions?.Invoke(options);

        return CreateClient(options);
    }

    /// <summary>
    /// Creates a new GeminiClient instance with custom options.
    /// </summary>
    /// <param name="options">The client configuration options.</param>
    /// <returns>A new GeminiClient instance.</returns>
    public static IGeminiClient Create(GeminiClientOptions options)
    {
        return CreateClient(options);
    }

    private static IGeminiClient CreateClient(GeminiClientOptions options)
    {
        // Create HTTP client
        var httpClient = new HttpClient();

        // Create simple loggers
        var loggerFactory = new SimpleLoggerFactory();
        var apiClientLogger = new SimpleLogger<ApiClient>();
        var geminiClientLogger = new SimpleLogger<GeminiClient>();

        // Create API client
        var apiClient = new ApiClient(httpClient, Microsoft.Extensions.Options.Options.Create(options), apiClientLogger);

        // Create main client
        return new GeminiClient(apiClient, Microsoft.Extensions.Options.Options.Create(options), geminiClientLogger, null);
    }
}

/// <summary>
/// Simple logger implementation for factory scenarios.
/// </summary>
internal class SimpleLogger<T> : Microsoft.Extensions.Logging.ILogger<T>
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => false;
    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
}

/// <summary>
/// Simple logger factory for factory scenarios.
/// </summary>
internal class SimpleLoggerFactory : Microsoft.Extensions.Logging.ILoggerFactory
{
    public void AddProvider(Microsoft.Extensions.Logging.ILoggerProvider provider) { }
    public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName) => new SimpleLogger();
    public void Dispose() { }
}

/// <summary>
/// Simple logger implementation.
/// </summary>
internal class SimpleLogger : Microsoft.Extensions.Logging.ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => false;
    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
}
