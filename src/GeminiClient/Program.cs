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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GoogleGenAI.Client;
using GoogleGenAI.Client.Extensions;
using GoogleGenAI.Client.Models;

namespace GoogleGenAI.Client.Demo;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Google Generative AI .NET Client Demo");
        Console.WriteLine("=====================================");

        try
        {
            // Example 1: Using dependency injection
            await RunWithDependencyInjectionExample();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static async Task RunWithDependencyInjectionExample()
    {
        Console.WriteLine("Example 1: Using Dependency Injection");
        Console.WriteLine("Note: This example requires proper API key configuration");

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Build host with dependency injection
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Add GeminiClient with configuration
                services.AddGeminiClient(configuration);
                
                // Or add with API key directly:
                // services.AddGeminiClient("your-api-key-here");
                
                // Or add for Vertex AI:
                // services.AddGeminiClientForVertexAI("your-project-id", "us-central1");
            })
            .Build();

        // Get the client from DI container
        var client = host.Services.GetRequiredService<IModelsService>();
        var a = await client.GenerateContentAsync("gemini-1.5-flash", new List<Content>
        {
           new Content
           {
                Parts = new List<Part>
                {
                    new Part
                    {
                        Text = "What is the capital of France?",
                    }
                },
                Role = "user"
           }
        });
    }


    static IGeminiClient CreateMockClient()
    {
        // This creates a client that will fail on actual API calls but demonstrates the API structure
        return GeminiClientFactory.Create("mock-api-key");
    }
}

// Example configuration class for appsettings.json
public class AppSettings
{
    public GeminiClientConfig GeminiClient { get; set; } = new();
}

public class GeminiClientConfig
{
    public string? ApiKey { get; set; }
    public bool UseVertexAI { get; set; } = false;
    public string? ProjectId { get; set; }
    public string? Location { get; set; }
    public int TimeoutSeconds { get; set; } = 60;
    public int MaxRetryAttempts { get; set; } = 3;
}