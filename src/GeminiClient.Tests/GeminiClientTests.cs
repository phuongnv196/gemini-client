using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoogleGenAI.Client;
using GoogleGenAI.Client.Configuration;
using GoogleGenAI.Client.Models;
using GoogleGenAI.Client.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GeminiClient.Tests
{
    public class GeminiClientTests
    {
        private readonly Mock<IApiClient> _mockApiClient;
        private readonly Mock<IOptions<GeminiClientOptions>> _mockOptions;
        private readonly Mock<ILogger<GoogleGenAI.Client.GeminiClient>> _mockLogger;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IModelsService> _mockModelsService;
        private readonly GeminiClientOptions _options;

        public GeminiClientTests()
        {
            _mockApiClient = new Mock<IApiClient>();
            _mockOptions = new Mock<IOptions<GeminiClientOptions>>();
            _mockLogger = new Mock<ILogger<GoogleGenAI.Client.GeminiClient>>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockModelsService = new Mock<IModelsService>();

            _options = new GeminiClientOptions
            {
                ApiKey = "test-api-key"
            };
            _mockOptions.Setup(o => o.Value).Returns(_options);

            _mockServiceProvider
                .Setup(p => p.GetService(typeof(IModelsService)))
                .Returns(_mockModelsService.Object);
        }

        private GoogleGenAI.Client.GeminiClient CreateClient()
        {
            return new GoogleGenAI.Client.GeminiClient(
                _mockApiClient.Object,
                _mockOptions.Object,
                _mockLogger.Object,
                _mockServiceProvider.Object);
        }

        [Fact]
        public async Task GenerateTextAsync_HappyPath_ReturnsText()
        {
            // Arrange
            var client = CreateClient();
            var modelName = "gemini-1.5-flash";
            var prompt = "Hello, world!";
            var expectedText = "Hi there!";

            var response = new GenerateContentResponse
            {
                Candidates = new List<Candidate>
                {
                    new Candidate
                    {
                        Content = new Content
                        {
                            Parts = new List<Part>
                            {
                                new Part { Text = expectedText }
                            }
                        }
                    }
                }
            };

            _mockModelsService
                .Setup(m => m.GenerateContentAsync(
                    modelName,
                    It.Is<IEnumerable<Content>>(c => c.First().Parts.First().Text == prompt),
                    It.Is<GenerateContentConfig>(c => c.GenerationConfig == null),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await client.GenerateTextAsync(modelName, prompt);

            // Assert
            Assert.Equal(expectedText, result);
            _mockModelsService.Verify(m => m.GenerateContentAsync(
                modelName,
                It.Is<IEnumerable<Content>>(c => c.First().Parts.First().Text == prompt),
                It.Is<GenerateContentConfig>(c => c.GenerationConfig == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateTextAsync_WithMaxTokensAndTemperature_SetsConfig()
        {
            // Arrange
            var client = CreateClient();
            var modelName = "gemini-1.5-flash";
            var prompt = "Hello, world!";
            var expectedText = "Hi there!";
            int maxTokens = 100;
            float temperature = 0.7f;

            var response = new GenerateContentResponse
            {
                Candidates = new List<Candidate>
                {
                    new Candidate
                    {
                        Content = new Content
                        {
                            Parts = new List<Part>
                            {
                                new Part { Text = expectedText }
                            }
                        }
                    }
                }
            };

            _mockModelsService
                .Setup(m => m.GenerateContentAsync(
                    modelName,
                    It.IsAny<IEnumerable<Content>>(),
                    It.Is<GenerateContentConfig>(c =>
                        c.GenerationConfig != null &&
                        c.GenerationConfig.MaxOutputTokens == maxTokens &&
                        c.GenerationConfig.Temperature == temperature),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await client.GenerateTextAsync(modelName, prompt, maxTokens, temperature);

            // Assert
            Assert.Equal(expectedText, result);
            _mockModelsService.Verify(m => m.GenerateContentAsync(
                modelName,
                It.IsAny<IEnumerable<Content>>(),
                It.Is<GenerateContentConfig>(c =>
                    c.GenerationConfig != null &&
                    c.GenerationConfig.MaxOutputTokens == maxTokens &&
                    c.GenerationConfig.Temperature == temperature),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateTextAsync_EmptyResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var client = CreateClient();
            var modelName = "gemini-1.5-flash";
            var prompt = "Hello, world!";

            var response = new GenerateContentResponse
            {
                Candidates = new List<Candidate>() // Empty candidates
            };

            _mockModelsService
                .Setup(m => m.GenerateContentAsync(
                    modelName,
                    It.IsAny<IEnumerable<Content>>(),
                    It.IsAny<GenerateContentConfig>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                client.GenerateTextAsync(modelName, prompt));

            Assert.Equal("No text content in response", ex.Message);
        }

        [Fact]
        public async Task GenerateTextAsync_WhenDisposed_ThrowsObjectDisposedException()
        {
            // Arrange
            var client = CreateClient();
            client.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() =>
                client.GenerateTextAsync("gemini-1.5-flash", "Hello"));
        }
    }
}
