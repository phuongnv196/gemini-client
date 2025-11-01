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
/// Configuration for generating content.
/// </summary>
public class GenerateContentConfig
{
    /// <summary>
    /// Generation configuration parameters.
    /// </summary>
    [JsonPropertyName("generation_config")]
    public GenerationConfig? GenerationConfig { get; set; }

    /// <summary>
    /// Safety settings.
    /// </summary>
    [JsonPropertyName("safety_settings")]
    public List<SafetySetting> SafetySettings { get; set; } = new();

    /// <summary>
    /// Tool configuration.
    /// </summary>
    [JsonPropertyName("tool_config")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ToolConfig? ToolConfig { get; set; }

    /// <summary>
    /// Tools available to the model.
    /// </summary>
    [JsonPropertyName("tools")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Tool>? Tools { get; set; }

    /// <summary>
    /// System instruction for the model.
    /// </summary>
    [JsonPropertyName("system_instruction")]
    public Content? SystemInstruction { get; set; }

    /// <summary>
    /// Cached content to use.
    /// </summary>
    [JsonPropertyName("cached_content")]
    public string? CachedContent { get; set; }
}

/// <summary>
/// Generation configuration parameters.
/// </summary>
public class GenerationConfig
{
    /// <summary>
    /// Maximum number of tokens to generate.
    /// </summary>
    [JsonPropertyName("max_output_tokens")]
    public int? MaxOutputTokens { get; set; }

    /// <summary>
    /// Temperature for randomness in generation (0.0 to 2.0).
    /// </summary>
    [JsonPropertyName("temperature")]
    public float? Temperature { get; set; }

    /// <summary>
    /// Top-p value for nucleus sampling.
    /// </summary>
    [JsonPropertyName("top_p")]
    public float? TopP { get; set; }

    /// <summary>
    /// Top-k value for top-k sampling.
    /// </summary>
    [JsonPropertyName("top_k")]
    public int? TopK { get; set; }

    /// <summary>
    /// Number of candidate responses to generate.
    /// </summary>
    [JsonPropertyName("candidate_count")]
    public int? CandidateCount { get; set; }

    /// <summary>
    /// Stop sequences to end generation.
    /// </summary>
    [JsonPropertyName("stop_sequences")]
    public List<string> StopSequences { get; set; } = new();

    /// <summary>
    /// Response MIME type.
    /// </summary>
    [JsonPropertyName("response_mime_type")]
    public string? ResponseMimeType { get; set; }

    /// <summary>
    /// Response schema for structured output.
    /// </summary>
    [JsonPropertyName("response_schema")]
    public object? ResponseSchema { get; set; }

    /// <summary>
    /// Presence penalty for token repetition.
    /// </summary>
    [JsonPropertyName("presence_penalty")]
    public float? PresencePenalty { get; set; }

    /// <summary>
    /// Frequency penalty for token repetition.
    /// </summary>
    [JsonPropertyName("frequency_penalty")]
    public float? FrequencyPenalty { get; set; }
}

/// <summary>
/// Safety setting for content filtering.
/// </summary>
public class SafetySetting
{
    /// <summary>
    /// Category of content to filter.
    /// </summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Threshold for blocking content.
    /// </summary>
    [JsonPropertyName("threshold")]
    public string Threshold { get; set; } = string.Empty;
}

/// <summary>
/// Tool configuration.
/// </summary>
public class ToolConfig
{
    /// <summary>
    /// Function calling configuration.
    /// </summary>
    [JsonPropertyName("function_calling_config")]
    public FunctionCallingConfig? FunctionCallingConfig { get; set; }
}

/// <summary>
/// Configuration for function calling.
/// </summary>
public class FunctionCallingConfig
{
    /// <summary>
    /// Mode for function calling (AUTO, ANY, NONE).
    /// </summary>
    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    /// <summary>
    /// Allowed function names.
    /// </summary>
    [JsonPropertyName("allowed_function_names")]
    public List<string> AllowedFunctionNames { get; set; } = new();
}

/// <summary>
/// Tool available to the model.
/// </summary>
public class Tool
{
    /// <summary>
    /// Function declarations.
    /// </summary>
    [JsonPropertyName("function_declarations")]
    public List<FunctionDeclaration> FunctionDeclarations { get; set; } = new();

    /// <summary>
    /// Code execution capability.
    /// </summary>
    [JsonPropertyName("code_execution")]
    public object? CodeExecution { get; set; }

    /// <summary>
    /// Google search capability.
    /// </summary>
    [JsonPropertyName("google_search")]
    public object? GoogleSearch { get; set; }
}

/// <summary>
/// Declaration of a function that can be called.
/// </summary>
public class FunctionDeclaration
{
    /// <summary>
    /// Name of the function.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of what the function does.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Parameters schema for the function.
    /// </summary>
    [JsonPropertyName("parameters")]
    public object? Parameters { get; set; }
}
