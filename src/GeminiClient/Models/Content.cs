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
/// Represents content with a role and parts.
/// </summary>
public class Content
{
    /// <summary>
    /// The role of the content (user, model, etc.).
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// The parts that make up this content.
    /// </summary>
    [JsonPropertyName("parts")]
    public List<Part> Parts { get; set; } = new();

    /// <summary>
    /// Validates that this content has valid parts.
    /// </summary>
    public bool IsValid()
    {
        if (!Parts.Any()) return false;
        
        return Parts.All(part => part.IsValid());
    }
}

/// <summary>
/// Represents a part of content (text, image, etc.).
/// </summary>
public class Part
{
    /// <summary>
    /// Text content of this part.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Inline data for this part (images, files, etc.).
    /// </summary>
    [JsonPropertyName("inline_data")]
    public InlineData? InlineData { get; set; }

    /// <summary>
    /// File data reference.
    /// </summary>
    [JsonPropertyName("file_data")]
    public FileData? FileData { get; set; }

    /// <summary>
    /// Function call data.
    /// </summary>
    [JsonPropertyName("function_call")]
    public FunctionCall? FunctionCall { get; set; }

    /// <summary>
    /// Function response data.
    /// </summary>
    [JsonPropertyName("function_response")]
    public FunctionResponse? FunctionResponse { get; set; }

    /// <summary>
    /// Validates that this part has valid content.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Text) ||
               InlineData != null ||
               FileData != null ||
               FunctionCall != null ||
               FunctionResponse != null;
    }
}

/// <summary>
/// Represents inline data (base64 encoded).
/// </summary>
public class InlineData
{
    /// <summary>
    /// MIME type of the data.
    /// </summary>
    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Base64 encoded data.
    /// </summary>
    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;
}

/// <summary>
/// Represents a file data reference.
/// </summary>
public class FileData
{
    /// <summary>
    /// MIME type of the file.
    /// </summary>
    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// URI of the file.
    /// </summary>
    [JsonPropertyName("file_uri")]
    public string FileUri { get; set; } = string.Empty;
}

/// <summary>
/// Represents a function call.
/// </summary>
public class FunctionCall
{
    /// <summary>
    /// Name of the function to call.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Arguments for the function call.
    /// </summary>
    [JsonPropertyName("args")]
    public Dictionary<string, object> Args { get; set; } = new();
}

/// <summary>
/// Represents a function response.
/// </summary>
public class FunctionResponse
{
    /// <summary>
    /// Name of the function that was called.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Response from the function.
    /// </summary>
    [JsonPropertyName("response")]
    public Dictionary<string, object> Response { get; set; } = new();
}
