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

namespace GoogleGenAI.Client.Models;

/// <summary>
/// Represents a Generative AI model.
/// </summary>
public class Model
{
    /// <summary>
    /// The resource name of the model.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The human-readable name of the model.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// A description of the model.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of input tokens for this model.
    /// </summary>
    [JsonPropertyName("input_token_limit")]
    public int InputTokenLimit { get; set; }

    /// <summary>
    /// Maximum number of output tokens for this model.
    /// </summary>
    [JsonPropertyName("output_token_limit")]
    public int OutputTokenLimit { get; set; }

    /// <summary>
    /// The model's supported generation methods.
    /// </summary>
    [JsonPropertyName("supported_generation_methods")]
    public List<string> SupportedGenerationMethods { get; set; } = new();

    /// <summary>
    /// Temperature range supported by this model.
    /// </summary>
    [JsonPropertyName("temperature")]
    public Range? Temperature { get; set; }

    /// <summary>
    /// Top-p range supported by this model.
    /// </summary>
    [JsonPropertyName("top_p")]
    public Range? TopP { get; set; }

    /// <summary>
    /// Top-k range supported by this model.
    /// </summary>
    [JsonPropertyName("top_k")]
    public Range? TopK { get; set; }
}

/// <summary>
/// Represents a numeric range.
/// </summary>
public class Range
{
    /// <summary>
    /// Minimum value.
    /// </summary>
    [JsonPropertyName("min")]
    public float Min { get; set; }

    /// <summary>
    /// Maximum value.
    /// </summary>
    [JsonPropertyName("max")]
    public float Max { get; set; }
}

/// <summary>
/// Metadata for an uploaded file.
/// </summary>
public class FileMetadata
{
    /// <summary>
    /// The resource name of the file.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the file.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// MIME type of the file.
    /// </summary>
    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Size of the file in bytes.
    /// </summary>
    [JsonPropertyName("size_bytes")]
    public long SizeBytes { get; set; }

    /// <summary>
    /// When the file was created.
    /// </summary>
    [JsonPropertyName("create_time")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// When the file was last updated.
    /// </summary>
    [JsonPropertyName("update_time")]
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// When the file will expire.
    /// </summary>
    [JsonPropertyName("expiration_time")]
    public DateTime? ExpirationTime { get; set; }

    /// <summary>
    /// SHA-256 hash of the file content.
    /// </summary>
    [JsonPropertyName("sha256_hash")]
    public string? Sha256Hash { get; set; }

    /// <summary>
    /// URI to access the file.
    /// </summary>
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    /// <summary>
    /// Current state of the file.
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Error information if file processing failed.
    /// </summary>
    [JsonPropertyName("error")]
    public FileError? Error { get; set; }

    /// <summary>
    /// Video metadata if this is a video file.
    /// </summary>
    [JsonPropertyName("video_metadata")]
    public VideoMetadata? VideoMetadata { get; set; }
}

/// <summary>
/// Error information for file processing.
/// </summary>
public class FileError
{
    /// <summary>
    /// Error code.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Metadata for video files.
/// </summary>
public class VideoMetadata
{
    /// <summary>
    /// Video duration.
    /// </summary>
    [JsonPropertyName("video_duration")]
    public string? VideoDuration { get; set; }
}

/// <summary>
/// Information about a cached content.
/// </summary>
public class CacheInfo
{
    /// <summary>
    /// The resource name of the cache.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the cache.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The model used for this cache.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// The system instruction used.
    /// </summary>
    [JsonPropertyName("system_instruction")]
    public Content? SystemInstruction { get; set; }

    /// <summary>
    /// The cached contents.
    /// </summary>
    [JsonPropertyName("contents")]
    public List<Content> Contents { get; set; } = new();

    /// <summary>
    /// Tools configuration.
    /// </summary>
    [JsonPropertyName("tools")]
    public List<Tool> Tools { get; set; } = new();

    /// <summary>
    /// Tool configuration.
    /// </summary>
    [JsonPropertyName("tool_config")]
    public ToolConfig? ToolConfig { get; set; }

    /// <summary>
    /// When the cache was created.
    /// </summary>
    [JsonPropertyName("create_time")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// When the cache was last updated.
    /// </summary>
    [JsonPropertyName("update_time")]
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// Usage metadata for the cache.
    /// </summary>
    [JsonPropertyName("usage_metadata")]
    public CacheUsageMetadata? UsageMetadata { get; set; }

    /// <summary>
    /// When the cache will expire.
    /// </summary>
    [JsonPropertyName("expire_time")]
    public DateTime? ExpireTime { get; set; }
}

/// <summary>
/// Request to create a cache.
/// </summary>
public class CreateCacheRequest
{
    /// <summary>
    /// The model to use for this cache.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the cache.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// The system instruction to use.
    /// </summary>
    [JsonPropertyName("system_instruction")]
    public Content? SystemInstruction { get; set; }

    /// <summary>
    /// The contents to cache.
    /// </summary>
    [JsonPropertyName("contents")]
    public List<Content> Contents { get; set; } = new();

    /// <summary>
    /// Tools configuration.
    /// </summary>
    [JsonPropertyName("tools")]
    public List<Tool> Tools { get; set; } = new();

    /// <summary>
    /// Tool configuration.
    /// </summary>
    [JsonPropertyName("tool_config")]
    public ToolConfig? ToolConfig { get; set; }

    /// <summary>
    /// Time to live for the cache.
    /// </summary>
    [JsonPropertyName("ttl")]
    public string? Ttl { get; set; }
}

/// <summary>
/// Request to update a cache.
/// </summary>
public class UpdateCacheRequest
{
    /// <summary>
    /// Time to live for the cache.
    /// </summary>
    [JsonPropertyName("ttl")]
    public string? Ttl { get; set; }

    /// <summary>
    /// When the cache should expire.
    /// </summary>
    [JsonPropertyName("expire_time")]
    public DateTime? ExpireTime { get; set; }
}

/// <summary>
/// Usage metadata for a cache.
/// </summary>
public class CacheUsageMetadata
{
    /// <summary>
    /// Total token count in the cache.
    /// </summary>
    [JsonPropertyName("total_token_count")]
    public int TotalTokenCount { get; set; }
}

/// <summary>
/// Response for token counting requests.
/// </summary>
public class TokenCountResponse
{
    /// <summary>
    /// Number of tokens in the content.
    /// </summary>
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}

/// <summary>
/// Request for token counting operations.
/// </summary>
public class CountTokensRequest
{
    /// <summary>
    /// Contents to count tokens for.
    /// </summary>
    [JsonPropertyName("contents")]
    public List<Content> Contents { get; set; } = new();
}
