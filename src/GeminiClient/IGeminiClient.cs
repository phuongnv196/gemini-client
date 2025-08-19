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

using GoogleGenAI.Client.Models;

namespace GoogleGenAI.Client;

/// <summary>
/// Interface for Google Generative AI client operations.
/// </summary>
public interface IGeminiClient : IDisposable
{
    /// <summary>
    /// Access to model operations (list models, get model info).
    /// </summary>
    IModelsService Models { get; }

    /// <summary>
    /// Access to chat operations (generate content, chat sessions).
    /// </summary>
    IChatsService Chats { get; }

    /// <summary>
    /// Access to file operations (upload, list, delete files).
    /// </summary>
    IFilesService Files { get; }

    /// <summary>
    /// Access to cache operations (create, list, delete caches).
    /// </summary>
    ICachesService Caches { get; }

    /// <summary>
    /// Access to batch operations (create and manage batch jobs).
    /// </summary>
    IBatchesService Batches { get; }

    /// <summary>
    /// Access to tuning operations (fine-tuning models).
    /// </summary>
    ITuningsService Tunings { get; }

    /// <summary>
    /// Access to authentication token operations.
    /// </summary>
    ITokensService AuthTokens { get; }

    /// <summary>
    /// Access to long-running operations.
    /// </summary>
    IOperationsService Operations { get; }

    /// <summary>
    /// Indicates whether this client is using Vertex AI API endpoints.
    /// </summary>
    bool IsVertexAI { get; }

    /// <summary>
    /// Creates a quick chat session for simple conversations.
    /// </summary>
    /// <param name="modelName">Name of the model to use (e.g., "gemini-1.5-flash").</param>
    /// <param name="systemInstruction">Optional system instruction to guide the model's behavior.</param>
    /// <returns>A new chat session.</returns>
    IChatSession CreateChatSession(string modelName, string? systemInstruction = null);

    /// <summary>
    /// Quickly generates content without maintaining conversation history.
    /// </summary>
    /// <param name="modelName">Name of the model to use.</param>
    /// <param name="prompt">The text prompt to send to the model.</param>
    /// <param name="maxTokens">Maximum number of tokens to generate.</param>
    /// <param name="temperature">Temperature for randomness (0.0 to 2.0).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated response.</returns>
    Task<string> GenerateTextAsync(
        string modelName, 
        string prompt, 
        int? maxTokens = null, 
        float? temperature = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Quickly generates content with streaming response.
    /// </summary>
    /// <param name="modelName">Name of the model to use.</param>
    /// <param name="prompt">The text prompt to send to the model.</param>
    /// <param name="maxTokens">Maximum number of tokens to generate.</param>
    /// <param name="temperature">Temperature for randomness (0.0 to 2.0).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of response chunks.</returns>
    IAsyncEnumerable<string> GenerateTextStreamAsync(
        string modelName, 
        string prompt, 
        int? maxTokens = null, 
        float? temperature = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for model operations.
/// </summary>
public interface IModelsService
{
    /// <summary>
    /// Lists available models.
    /// </summary>
    Task<IEnumerable<Model>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about a specific model.
    /// </summary>
    Task<Model> GetAsync(string modelName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates content using the specified model.
    /// </summary>
    Task<GenerateContentResponse> GenerateContentAsync(
        string modelName,
        IEnumerable<Content> contents,
        GenerateContentConfig? config = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates content with streaming response.
    /// </summary>
    IAsyncEnumerable<GenerateContentResponse> GenerateContentStreamAsync(
        string modelName,
        IEnumerable<Content> contents,
        GenerateContentConfig? config = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for chat operations.
/// </summary>
public interface IChatsService
{
    /// <summary>
    /// Creates a new chat session.
    /// </summary>
    IChatSession CreateSession(string modelName, GenerateContentConfig? config = null);

    /// <summary>
    /// Sends a single message and gets a response.
    /// </summary>
    Task<GenerateContentResponse> SendMessageAsync(
        string modelName,
        string message,
        GenerateContentConfig? config = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a single message with streaming response.
    /// </summary>
    IAsyncEnumerable<GenerateContentResponse> SendMessageStreamAsync(
        string modelName,
        string message,
        GenerateContentConfig? config = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for a chat session that maintains conversation history.
/// </summary>
public interface IChatSession
{
    /// <summary>
    /// The model name used for this session.
    /// </summary>
    string ModelName { get; }

    /// <summary>
    /// The conversation history.
    /// </summary>
    IReadOnlyList<Content> History { get; }

    /// <summary>
    /// Sends a message and gets a response.
    /// </summary>
    Task<GenerateContentResponse> SendMessageAsync(string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a message with streaming response.
    /// </summary>
    IAsyncEnumerable<GenerateContentResponse> SendMessageStreamAsync(string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends content (can include images, files, etc.) and gets a response.
    /// </summary>
    Task<GenerateContentResponse> SendContentAsync(Content content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends content with streaming response.
    /// </summary>
    IAsyncEnumerable<GenerateContentResponse> SendContentStreamAsync(Content content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the conversation history.
    /// </summary>
    void ClearHistory();
}

/// <summary>
/// Interface for file operations.
/// </summary>
public interface IFilesService
{
    /// <summary>
    /// Uploads a file.
    /// </summary>
    Task<FileMetadata> UploadAsync(Stream fileStream, string fileName, string? mimeType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists uploaded files.
    /// </summary>
    Task<IEnumerable<FileMetadata>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about a specific file.
    /// </summary>
    Task<FileMetadata> GetAsync(string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file.
    /// </summary>
    Task DeleteAsync(string fileName, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for cache operations.
/// </summary>
public interface ICachesService
{
    /// <summary>
    /// Creates a new cache.
    /// </summary>
    Task<CacheInfo> CreateAsync(CreateCacheRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists existing caches.
    /// </summary>
    Task<IEnumerable<CacheInfo>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about a specific cache.
    /// </summary>
    Task<CacheInfo> GetAsync(string cacheName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a cache.
    /// </summary>
    Task<CacheInfo> UpdateAsync(string cacheName, UpdateCacheRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a cache.
    /// </summary>
    Task DeleteAsync(string cacheName, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for batch operations.
/// </summary>
public interface IBatchesService
{
    /// <summary>
    /// Creates a new batch job.
    /// </summary>
    Task<BatchJob> CreateAsync(CreateBatchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists batch jobs.
    /// </summary>
    Task<IEnumerable<BatchJob>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about a specific batch job.
    /// </summary>
    Task<BatchJob> GetAsync(string batchName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a batch job.
    /// </summary>
    Task DeleteAsync(string batchName, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for tuning operations.
/// </summary>
public interface ITuningsService
{
    /// <summary>
    /// Creates a new tuning job.
    /// </summary>
    Task<TuningJob> CreateAsync(CreateTuningRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists tuning jobs.
    /// </summary>
    Task<IEnumerable<TuningJob>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about a specific tuning job.
    /// </summary>
    Task<TuningJob> GetAsync(string tuningName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a tuning job.
    /// </summary>
    Task DeleteAsync(string tuningName, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for authentication token operations.
/// </summary>
public interface ITokensService
{
    /// <summary>
    /// Counts tokens in the given content.
    /// </summary>
    Task<TokenCountResponse> CountTokensAsync(string modelName, IEnumerable<Content> contents, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for long-running operations.
/// </summary>
public interface IOperationsService
{
    /// <summary>
    /// Lists operations.
    /// </summary>
    Task<IEnumerable<Operation>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about a specific operation.
    /// </summary>
    Task<Operation> GetAsync(string operationName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels an operation.
    /// </summary>
    Task DeleteAsync(string operationName, CancellationToken cancellationToken = default);
}
