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

namespace GoogleGenAI.Client.Configuration;

/// <summary>
/// Configuration options for the Google Generative AI client.
/// </summary>
public class GeminiClientOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "GeminiClient";

    /// <summary>
    /// API key for Gemini Developer API authentication.
    /// Can also be set via GOOGLE_API_KEY environment variable.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Indicates whether to use Vertex AI API endpoints.
    /// Defaults to false (uses Gemini Developer API endpoints).
    /// Can also be set via GOOGLE_GENAI_USE_VERTEXAI environment variable.
    /// </summary>
    public bool UseVertexAI { get; set; } = false;

    /// <summary>
    /// Google Cloud project ID for Vertex AI API.
    /// Can also be set via GOOGLE_CLOUD_PROJECT environment variable.
    /// </summary>
    public string? ProjectId { get; set; }

    /// <summary>
    /// Location for Vertex AI API requests (e.g., "us-central1").
    /// Can also be set via GOOGLE_CLOUD_LOCATION environment variable.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Base URL for API requests. If not specified, will be determined 
    /// based on UseVertexAI setting.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// API version to use. Defaults to "v1".
    /// </summary>
    public string ApiVersion { get; set; } = "v1";

    /// <summary>
    /// Default timeout for HTTP requests in seconds. Defaults to 60.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Maximum number of retry attempts for failed requests. Defaults to 3.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Base delay between retry attempts in milliseconds. Defaults to 1000.
    /// </summary>
    public int RetryDelayMilliseconds { get; set; } = 1000;

    /// <summary>
    /// Whether to use exponential backoff for retries. Defaults to true.
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;

    /// <summary>
    /// Maximum delay between retry attempts in milliseconds. Defaults to 10000.
    /// </summary>
    public int MaxRetryDelayMilliseconds { get; set; } = 10000;

    /// <summary>
    /// User agent string for HTTP requests.
    /// </summary>
    public string UserAgent { get; set; } = "GoogleGenAI-DotNet/1.0.0";

    /// <summary>
    /// Debug configuration options.
    /// </summary>
    public DebugConfiguration Debug { get; set; } = new();

    /// <summary>
    /// Gets the effective base URL based on configuration.
    /// </summary>
    public string GetBaseUrl()
    {
        if (!string.IsNullOrEmpty(BaseUrl))
            return BaseUrl;

        return UseVertexAI
            ? $"https://{Location ?? "us-central1"}-aiplatform.googleapis.com"
            : "https://generativelanguage.googleapis.com";
    }

    /// <summary>
    /// Gets the API key from configuration or environment variables.
    /// </summary>
    public string? GetApiKey()
    {
        return ApiKey ?? Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
    }

    /// <summary>
    /// Gets the project ID from configuration or environment variables.
    /// </summary>
    public string? GetProjectId()
    {
        return ProjectId ?? Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT");
    }

    /// <summary>
    /// Gets the location from configuration or environment variables.
    /// </summary>
    public string? GetLocation()
    {
        return Location ?? Environment.GetEnvironmentVariable("GOOGLE_CLOUD_LOCATION");
    }

    /// <summary>
    /// Gets whether to use Vertex AI from configuration or environment variables.
    /// </summary>
    public bool GetUseVertexAI()
    {
        var envVar = Environment.GetEnvironmentVariable("GOOGLE_GENAI_USE_VERTEXAI");
        return UseVertexAI || (envVar?.ToLowerInvariant() == "true");
    }

    /// <summary>
    /// Validates the configuration settings.
    /// </summary>
    public void Validate()
    {
        var useVertexAI = GetUseVertexAI();
        
        if (!useVertexAI)
        {
            // Gemini Developer API requires API key
            if (string.IsNullOrEmpty(GetApiKey()))
            {
                throw new InvalidOperationException(
                    "API key is required for Gemini Developer API. " +
                    "Set the ApiKey property or GOOGLE_API_KEY environment variable.");
            }
        }
        else
        {
            // Vertex AI requires project ID and location
            if (string.IsNullOrEmpty(GetProjectId()))
            {
                throw new InvalidOperationException(
                    "Project ID is required for Vertex AI API. " +
                    "Set the ProjectId property or GOOGLE_CLOUD_PROJECT environment variable.");
            }

            if (string.IsNullOrEmpty(GetLocation()))
            {
                throw new InvalidOperationException(
                    "Location is required for Vertex AI API. " +
                    "Set the Location property or GOOGLE_CLOUD_LOCATION environment variable.");
            }
        }

        if (TimeoutSeconds <= 0)
        {
            throw new InvalidOperationException("TimeoutSeconds must be greater than 0.");
        }

        if (MaxRetryAttempts < 0)
        {
            throw new InvalidOperationException("MaxRetryAttempts must be non-negative.");
        }

        if (RetryDelayMilliseconds <= 0)
        {
            throw new InvalidOperationException("RetryDelayMilliseconds must be greater than 0.");
        }
    }
}

/// <summary>
/// Debug configuration options for testing and development.
/// </summary>
public class DebugConfiguration
{
    /// <summary>
    /// Client mode for testing (record, replay, auto).
    /// Can also be set via GOOGLE_GENAI_CLIENT_MODE environment variable.
    /// </summary>
    public string? ClientMode { get; set; }

    /// <summary>
    /// Directory for storing replay files.
    /// Can also be set via GOOGLE_GENAI_REPLAYS_DIRECTORY environment variable.
    /// </summary>
    public string? ReplaysDirectory { get; set; }

    /// <summary>
    /// Replay ID for test scenarios.
    /// Can also be set via GOOGLE_GENAI_REPLAY_ID environment variable.
    /// </summary>
    public string? ReplayId { get; set; }

    /// <summary>
    /// Gets the client mode from configuration or environment variables.
    /// </summary>
    public string? GetClientMode()
    {
        return ClientMode ?? Environment.GetEnvironmentVariable("GOOGLE_GENAI_CLIENT_MODE");
    }

    /// <summary>
    /// Gets the replays directory from configuration or environment variables.
    /// </summary>
    public string? GetReplaysDirectory()
    {
        return ReplaysDirectory ?? Environment.GetEnvironmentVariable("GOOGLE_GENAI_REPLAYS_DIRECTORY");
    }

    /// <summary>
    /// Gets the replay ID from configuration or environment variables.
    /// </summary>
    public string? GetReplayId()
    {
        return ReplayId ?? Environment.GetEnvironmentVariable("GOOGLE_GENAI_REPLAY_ID");
    }
}
