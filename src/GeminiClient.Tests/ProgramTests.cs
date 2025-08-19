using GoogleGenAI.Client;
using GoogleGenAI.Client.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace GeminiClient.Tests
{
    public class ProgramTests
    {
        [Fact]
        public async Task RunWithDependencyInjectionExample_ShouldCallGenerateContentAsync()
        {
            // Arrange
            var mockModelsService = new Mock<IModelsService>();
            var expectedResponse = new GenerateContentResponse();
            
            mockModelsService.Setup(s => s.GenerateContentAsync(
                It.IsAny<string>(),
                It.IsAny<IEnumerable<Content>>(),
                It.IsAny<GenerateContentConfig>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton(mockModelsService.Object);
                })
                .Build();

            // Act
            var client = host.Services.GetRequiredService<IModelsService>();
            var response = await client.GenerateContentAsync("gemini-1.5-flash", new List<Content>
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

            // Assert
            mockModelsService.Verify(s => s.GenerateContentAsync(
                "gemini-1.5-flash",
                It.IsAny<IEnumerable<Content>>(),
                null,
                It.IsAny<CancellationToken>()),
                Times.Once);
            
            Assert.Same(expectedResponse, response);
        }
    }
}