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
/// Response from the generate content API.
/// </summary>
public class GenerateContentResponse
{
    /// <summary>
    /// List of candidate responses.
    /// </summary>
    [JsonPropertyName("candidates")]
    public List<Candidate> Candidates { get; set; } = new();

    /// <summary>
    /// Usage metadata for the request.
    /// </summary>
    [JsonPropertyName("usage_metadata")]
    public UsageMetadata? UsageMetadata { get; set; }

    /// <summary>
    /// Prompt feedback.
    /// </summary>
    [JsonPropertyName("prompt_feedback")]
    public PromptFeedback? PromptFeedback { get; set; }

    /// <summary>
    /// Gets the text from the first candidate if available.
    /// </summary>
    public string? Text => Candidates.FirstOrDefault()?.Content?.Parts
        .FirstOrDefault(p => !string.IsNullOrEmpty(p.Text))?.Text;

    /// <summary>
    /// Validates that this response has valid content.
    /// </summary>
    public bool IsValid()
    {
        if (!Candidates.Any()) return false;
        
        var firstCandidate = Candidates.First();
        return firstCandidate.Content?.IsValid() == true;
    }
}

/// <summary>
/// Represents a candidate response.
/// </summary>
public class Candidate
{
    /// <summary>
    /// Content of the candidate.
    /// </summary>
    [JsonPropertyName("content")]
    public Content? Content { get; set; }

    /// <summary>
    /// Finish reason for this candidate.
    /// </summary>
    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }

    /// <summary>
    /// Safety ratings for this candidate.
    /// </summary>
    [JsonPropertyName("safety_ratings")]
    public List<SafetyRating> SafetyRatings { get; set; } = new();

    /// <summary>
    /// Citation metadata.
    /// </summary>
    [JsonPropertyName("citation_metadata")]
    public CitationMetadata? CitationMetadata { get; set; }

    /// <summary>
    /// Token count for this candidate.
    /// </summary>
    [JsonPropertyName("token_count")]
    public int? TokenCount { get; set; }

    /// <summary>
    /// Index of this candidate.
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }
}

/// <summary>
/// Usage metadata for the request.
/// </summary>
public class UsageMetadata
{
    /// <summary>
    /// Number of tokens in the prompt.
    /// </summary>
    [JsonPropertyName("prompt_token_count")]
    public int PromptTokenCount { get; set; }

    /// <summary>
    /// Number of tokens in the response.
    /// </summary>
    [JsonPropertyName("candidates_token_count")]
    public int CandidatesTokenCount { get; set; }

    /// <summary>
    /// Total number of tokens.
    /// </summary>
    [JsonPropertyName("total_token_count")]
    public int TotalTokenCount { get; set; }
}

/// <summary>
/// Prompt feedback.
/// </summary>
public class PromptFeedback
{
    /// <summary>
    /// Block reason if the prompt was blocked.
    /// </summary>
    [JsonPropertyName("block_reason")]
    public string? BlockReason { get; set; }

    /// <summary>
    /// Safety ratings for the prompt.
    /// </summary>
    [JsonPropertyName("safety_ratings")]
    public List<SafetyRating> SafetyRatings { get; set; } = new();
}

/// <summary>
/// Safety rating for content.
/// </summary>
public class SafetyRating
{
    /// <summary>
    /// Category of the safety rating.
    /// </summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Probability level of the safety concern.
    /// </summary>
    [JsonPropertyName("probability")]
    public string Probability { get; set; } = string.Empty;

    /// <summary>
    /// Whether this content was blocked.
    /// </summary>
    [JsonPropertyName("blocked")]
    public bool Blocked { get; set; }
}

/// <summary>
/// Citation metadata for generated content.
/// </summary>
public class CitationMetadata
{
    /// <summary>
    /// List of citation sources.
    /// </summary>
    [JsonPropertyName("citation_sources")]
    public List<CitationSource> CitationSources { get; set; } = new();
}

/// <summary>
/// Individual citation source.
/// </summary>
public class CitationSource
{
    /// <summary>
    /// Start index of the citation.
    /// </summary>
    [JsonPropertyName("start_index")]
    public int StartIndex { get; set; }

    /// <summary>
    /// End index of the citation.
    /// </summary>
    [JsonPropertyName("end_index")]
    public int EndIndex { get; set; }

    /// <summary>
    /// URI of the source.
    /// </summary>
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    /// <summary>
    /// License of the source.
    /// </summary>
    [JsonPropertyName("license")]
    public string? License { get; set; }
}
