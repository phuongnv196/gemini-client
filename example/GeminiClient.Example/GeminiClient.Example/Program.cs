using GoogleGenAI.Client;
using GoogleGenAI.Client.Extensions;
using GoogleGenAI.Client.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GeminiClient.Example;

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