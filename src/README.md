# Google Generative AI .NET Client

A professional, fully-featured .NET client library for Google's Generative AI APIs, including Gemini Developer API and Vertex AI API.

## Features

- ? **Complete API Coverage**: Support for all major Gemini API features
- ? **Dual API Support**: Works with both Gemini Developer API and Vertex AI
- ? **Modern .NET**: Built for .NET 9.0 with async/await patterns
- ? **Dependency Injection**: Full support for Microsoft.Extensions.DependencyInjection
- ? **Configuration**: Flexible configuration via appsettings.json or environment variables
- ? **Streaming Support**: Real-time streaming responses
- ? **Chat Sessions**: Stateful conversations with history management
- ? **Error Handling**: Comprehensive exception handling with specific error types
- ? **Logging**: Integrated with Microsoft.Extensions.Logging
- ? **Retry Logic**: Automatic retry with exponential backoff
- ? **Type Safety**: Strongly-typed models and responses

## Installation

```bash
dotnet add package GoogleGenAI.Client
```

## Quick Start

### Using Factory Method (Simple)

```csharp
using GoogleGenAI.Client;
using GoogleGenAI.Client.Extensions;

// Create client with API key
using var client = GeminiClientFactory.Create("your-api-key-here");

// Generate text
var response = await client.GenerateTextAsync("gemini-1.5-flash", "Hello, world!");
Console.WriteLine(response);

// Create a chat session
var chat = client.CreateChatSession("gemini-1.5-flash", "You are a helpful assistant.");
var chatResponse = await chat.SendMessageAsync("Tell me a joke!");
Console.WriteLine(chatResponse.Text);
```

### Using Dependency Injection (Recommended)

#### 1. Configure in appsettings.json

```json
{
  "GeminiClient": {
    "ApiKey": "your-api-key-here",
    "TimeoutSeconds": 30,
    "MaxRetryAttempts": 3
  }
}
```

#### 2. Register Services

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GoogleGenAI.Client.Extensions;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        // Add GeminiClient with configuration
        services.AddGeminiClient(context.Configuration);
        
        // Or add with API key directly:
        // services.AddGeminiClient("your-api-key-here");
        
        // Or add for Vertex AI:
        // services.AddGeminiClientForVertexAI("your-project-id", "us-central1");
    })
    .Build();
```

#### 3. Use in Your Services

```csharp
public class MyService
{
    private readonly IGeminiClient _geminiClient;

    public MyService(IGeminiClient geminiClient)
    {
        _geminiClient = geminiClient;
    }

    public async Task<string> GenerateContentAsync(string prompt)
    {
        return await _geminiClient.GenerateTextAsync("gemini-1.5-flash", prompt);
    }
}
```

## Advanced Usage

### Chat Sessions

```csharp
// Create a chat session with system instruction
var chat = client.CreateChatSession(
    "gemini-1.5-flash", 
    "You are a helpful coding assistant. Always provide clear examples.");

// Send messages and maintain conversation history
var response1 = await chat.SendMessageAsync("How do I create a list in Python?");
var response2 = await chat.SendMessageAsync("What about in C#?");

// Access conversation history
Console.WriteLine($"Conversation has {chat.History.Count} messages");

// Clear history if needed
chat.ClearHistory();
```

### Streaming Responses

```csharp
// Stream text generation
await foreach (var chunk in client.GenerateTextStreamAsync("gemini-1.5-flash", "Write a story"))
{
    Console.Write(chunk);
}

// Stream chat responses
await foreach (var chunk in chat.SendMessageStreamAsync("Tell me a long story"))
{
    Console.Write(chunk.Text);
}
```

### Advanced Content Generation

```csharp
using GoogleGenAI.Client.Models;

// Create complex content with multiple parts
var contents = new[]
{
    new Content
    {
        Role = "user",
        Parts = new List<Part>
        {
            new() { Text = "Analyze this image:" },
            new() { 
                InlineData = new InlineData 
                { 
                    MimeType = "image/jpeg", 
                    Data = Convert.ToBase64String(imageBytes) 
                } 
            }
        }
    }
};

// Configure generation parameters
var config = new GenerateContentConfig
{
    GenerationConfig = new GenerationConfig
    {
        MaxOutputTokens = 1000,
        Temperature = 0.7f,
        TopP = 0.9f
    },
    SafetySettings = new List<SafetySetting>
    {
        new() { Category = "HARM_CATEGORY_HARASSMENT", Threshold = "BLOCK_MEDIUM_AND_ABOVE" }
    }
};

var response = await client.Models.GenerateContentAsync("gemini-1.5-flash", contents, config);
```

### Working with Models

```csharp
// List available models
var models = await client.Models.ListAsync();
foreach (var model in models)
{
    Console.WriteLine($"{model.Name}: {model.Description}");
}

// Get model information
var modelInfo = await client.Models.GetAsync("gemini-1.5-flash");
Console.WriteLine($"Input token limit: {modelInfo.InputTokenLimit}");
```

### Token Counting

```csharp
var contents = new[]
{
    new Content
    {
        Role = "user",
        Parts = new List<Part> { new() { Text = "Hello, world!" } }
    }
};

var tokenCount = await client.AuthTokens.CountTokensAsync("gemini-1.5-flash", contents);
Console.WriteLine($"Token count: {tokenCount.TotalTokens}");
```

## Configuration

### Environment Variables

```bash
GOOGLE_API_KEY=your-api-key-here
GOOGLE_GENAI_USE_VERTEXAI=true
GOOGLE_CLOUD_PROJECT=your-project-id
GOOGLE_CLOUD_LOCATION=us-central1
```

### Vertex AI Configuration

```csharp
// For Vertex AI
using var client = GeminiClientFactory.CreateForVertexAI(
    "your-project-id", 
    "us-central1",
    options =>
    {
        options.TimeoutSeconds = 60;
        options.MaxRetryAttempts = 5;
    });
```

### Full Configuration Options

```json
{
  "GeminiClient": {
    "ApiKey": "your-api-key",
    "UseVertexAI": false,
    "ProjectId": "your-project-id",
    "Location": "us-central1",
    "BaseUrl": "custom-url",
    "ApiVersion": "v1",
    "TimeoutSeconds": 60,
    "MaxRetryAttempts": 3,
    "RetryDelayMilliseconds": 1000,
    "UseExponentialBackoff": true,
    "MaxRetryDelayMilliseconds": 10000,
    "UserAgent": "CustomApp/1.0",
    "Debug": {
      "ClientMode": "record",
      "ReplaysDirectory": "./replays",
      "ReplayId": "test-scenario-1"
    }
  }
}
```

## Error Handling

```csharp
using GoogleGenAI.Client.Exceptions;

try
{
    var response = await client.GenerateTextAsync("gemini-1.5-flash", "Hello!");
}
catch (GeminiAuthenticationException ex)
{
    Console.WriteLine($"Authentication failed: {ex.Message}");
}
catch (GeminiRateLimitException ex)
{
    Console.WriteLine($"Rate limited. Retry after: {ex.RetryAfter}");
}
catch (GeminiContentFilteredException ex)
{
    Console.WriteLine($"Content blocked: {string.Join(", ", ex.BlockedCategories)}");
}
catch (GeminiException ex)
{
    Console.WriteLine($"API error: {ex.Message} (Code: {ex.ErrorCode})");
}
```

## Service Overview

The client provides access to all major Gemini API services:

- **Models**: List models, get model info, generate content
- **Chats**: Create chat sessions, send messages
- **Files**: Upload, list, and manage files
- **Caches**: Create and manage cached content
- **Batches**: Submit and monitor batch jobs
- **Tunings**: Fine-tune models
- **AuthTokens**: Count tokens
- **Operations**: Monitor long-running operations

## Architecture

This client is designed following .NET best practices:

- **Separation of Concerns**: Each service handles a specific area of functionality
- **Dependency Injection**: All services are registered and can be injected
- **Configuration**: Flexible configuration via Options pattern
- **Logging**: Integrated logging throughout
- **Async/Await**: Full async support with cancellation tokens
- **Error Handling**: Specific exception types for different error scenarios
- **Testing**: Mockable interfaces for easy unit testing

## Contributing

We welcome contributions! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Development Setup

1. Clone the repository
2. Open the solution in Visual Studio 2022 or your preferred IDE
3. Build the solution: `dotnet build`
4. Run tests: `dotnet test`

### Guidelines

- Follow existing code style and conventions
- Add unit tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting PR

## Repository

This project is hosted on GitHub: https://github.com/phuongnv196/gemini-client

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Thanks to Google for providing the Generative AI APIs
- Inspired by the official Python SDK structure
- Built with modern .NET best practices