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
/// Represents a batch job for processing multiple requests.
/// </summary>
public class BatchJob
{
    /// <summary>
    /// The resource name of the batch job.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the batch job.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The model used for this batch job.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Current state of the batch job.
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// When the batch job was created.
    /// </summary>
    [JsonPropertyName("create_time")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// When the batch job was last updated.
    /// </summary>
    [JsonPropertyName("update_time")]
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// Request counts for the batch job.
    /// </summary>
    [JsonPropertyName("request_counts")]
    public RequestCounts? RequestCounts { get; set; }
}

/// <summary>
/// Request to create a batch job.
/// </summary>
public class CreateBatchRequest
{
    /// <summary>
    /// The model to use for this batch job.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the batch job.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// The requests to process in this batch.
    /// </summary>
    [JsonPropertyName("requests")]
    public List<BatchRequest> Requests { get; set; } = new();
}

/// <summary>
/// Individual request in a batch.
/// </summary>
public class BatchRequest
{
    /// <summary>
    /// Request ID for tracking.
    /// </summary>
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    /// The contents for generation.
    /// </summary>
    [JsonPropertyName("contents")]
    public List<Content> Contents { get; set; } = new();

    /// <summary>
    /// Generation configuration.
    /// </summary>
    [JsonPropertyName("generation_config")]
    public GenerationConfig? GenerationConfig { get; set; }

    /// <summary>
    /// Safety settings.
    /// </summary>
    [JsonPropertyName("safety_settings")]
    public List<SafetySetting> SafetySettings { get; set; } = new();
}

/// <summary>
/// Request counts for a batch job.
/// </summary>
public class RequestCounts
{
    /// <summary>
    /// Total number of requests.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>
    /// Number of completed requests.
    /// </summary>
    [JsonPropertyName("completed")]
    public int Completed { get; set; }

    /// <summary>
    /// Number of failed requests.
    /// </summary>
    [JsonPropertyName("failed")]
    public int Failed { get; set; }

    /// <summary>
    /// Number of pending requests.
    /// </summary>
    [JsonPropertyName("pending")]
    public int Pending { get; set; }
}

/// <summary>
/// Represents a tuning job for fine-tuning models.
/// </summary>
public class TuningJob
{
    /// <summary>
    /// The resource name of the tuning job.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the tuning job.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The base model being tuned.
    /// </summary>
    [JsonPropertyName("base_model")]
    public string BaseModel { get; set; } = string.Empty;

    /// <summary>
    /// Current state of the tuning job.
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// When the tuning job was created.
    /// </summary>
    [JsonPropertyName("create_time")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// When the tuning job was last updated.
    /// </summary>
    [JsonPropertyName("update_time")]
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// When the tuning job completed.
    /// </summary>
    [JsonPropertyName("complete_time")]
    public DateTime? CompleteTime { get; set; }

    /// <summary>
    /// Tuning task configuration.
    /// </summary>
    [JsonPropertyName("tuning_task")]
    public TuningTask? TuningTask { get; set; }

    /// <summary>
    /// The resulting tuned model.
    /// </summary>
    [JsonPropertyName("tuned_model")]
    public TunedModel? TunedModel { get; set; }

    /// <summary>
    /// Error information if tuning failed.
    /// </summary>
    [JsonPropertyName("error")]
    public TuningError? Error { get; set; }
}

/// <summary>
/// Request to create a tuning job.
/// </summary>
public class CreateTuningRequest
{
    /// <summary>
    /// The display name of the tuning job.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// The base model to tune.
    /// </summary>
    [JsonPropertyName("base_model")]
    public string BaseModel { get; set; } = string.Empty;

    /// <summary>
    /// Tuning task configuration.
    /// </summary>
    [JsonPropertyName("tuning_task")]
    public TuningTask TuningTask { get; set; } = new();
}

/// <summary>
/// Configuration for a tuning task.
/// </summary>
public class TuningTask
{
    /// <summary>
    /// Training data for the tuning task.
    /// </summary>
    [JsonPropertyName("training_data")]
    public Dataset? TrainingData { get; set; }

    /// <summary>
    /// Hyperparameters for tuning.
    /// </summary>
    [JsonPropertyName("hyperparameters")]
    public Hyperparameters? Hyperparameters { get; set; }
}

/// <summary>
/// Dataset configuration for tuning.
/// </summary>
public class Dataset
{
    /// <summary>
    /// Examples for training.
    /// </summary>
    [JsonPropertyName("examples")]
    public List<TuningExample> Examples { get; set; } = new();
}

/// <summary>
/// Training example for tuning.
/// </summary>
public class TuningExample
{
    /// <summary>
    /// Input text for the example.
    /// </summary>
    [JsonPropertyName("text_input")]
    public string TextInput { get; set; } = string.Empty;

    /// <summary>
    /// Output text for the example.
    /// </summary>
    [JsonPropertyName("output")]
    public string Output { get; set; } = string.Empty;
}

/// <summary>
/// Hyperparameters for tuning.
/// </summary>
public class Hyperparameters
{
    /// <summary>
    /// Learning rate for training.
    /// </summary>
    [JsonPropertyName("learning_rate")]
    public float? LearningRate { get; set; }

    /// <summary>
    /// Number of training epochs.
    /// </summary>
    [JsonPropertyName("epoch_count")]
    public int? EpochCount { get; set; }

    /// <summary>
    /// Batch size for training.
    /// </summary>
    [JsonPropertyName("batch_size")]
    public int? BatchSize { get; set; }
}

/// <summary>
/// Information about a tuned model.
/// </summary>
public class TunedModel
{
    /// <summary>
    /// The name of the tuned model.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the tuned model.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The base model that was tuned.
    /// </summary>
    [JsonPropertyName("base_model")]
    public string BaseModel { get; set; } = string.Empty;
}

/// <summary>
/// Error information for tuning jobs.
/// </summary>
public class TuningError
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
/// Represents a long-running operation.
/// </summary>
public class Operation
{
    /// <summary>
    /// The resource name of the operation.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Metadata about the operation.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Whether the operation is done.
    /// </summary>
    [JsonPropertyName("done")]
    public bool Done { get; set; }

    /// <summary>
    /// The operation result if successful.
    /// </summary>
    [JsonPropertyName("response")]
    public Dictionary<string, object>? Response { get; set; }

    /// <summary>
    /// Error information if the operation failed.
    /// </summary>
    [JsonPropertyName("error")]
    public OperationError? Error { get; set; }
}

/// <summary>
/// Error information for operations.
/// </summary>
public class OperationError
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

    /// <summary>
    /// Additional error details.
    /// </summary>
    [JsonPropertyName("details")]
    public List<Dictionary<string, object>> Details { get; set; } = new();
}
