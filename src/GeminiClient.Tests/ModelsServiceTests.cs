using GoogleGenAI.Client.Configuration;
using GoogleGenAI.Client.Models;
using GoogleGenAI.Client.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace GeminiClient.Tests
{
    public class ModelsServiceTests
    {
        private readonly Mock<IApiClient> _mockApiClient;
        private readonly Mock<IOptions<GeminiClientOptions>> _mockOptions;
        private readonly Mock<ILogger<ModelsService>> _mockLogger;

        public ModelsServiceTests()
        {
            _mockApiClient = new Mock<IApiClient>();
            _mockOptions = new Mock<IOptions<GeminiClientOptions>>();
            _mockLogger = new Mock<ILogger<ModelsService>>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetAsync_ShouldThrowArgumentException_WhenModelNameIsNullOrWhiteSpace(string? modelName)
        {
            // Arrange
            var options = new GeminiClientOptions { ApiVersion = "v1" };
            _mockOptions.Setup(o => o.Value).Returns(options);
            var service = new ModelsService(_mockApiClient.Object, _mockOptions.Object, _mockLogger.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.GetAsync(modelName!));
            Assert.Contains("Model name cannot be null or empty", exception.Message);
            Assert.Equal("modelName", exception.ParamName);
        }

        [Fact]
        public async Task GetAsync_ShouldCallApiClientWithCorrectEndpoint_WhenUseVertexAIIsFalse()
        {
            // Arrange
            var options = new GeminiClientOptions { ApiVersion = "v1", UseVertexAI = false, ApiKey = "fake_key" };
            _mockOptions.Setup(o => o.Value).Returns(options);
            var service = new ModelsService(_mockApiClient.Object, _mockOptions.Object, _mockLogger.Object);

            var expectedModel = new Model { Name = "models/gemini-1.5-flash", DisplayName = "Gemini 1.5 Flash" };
            _mockApiClient.Setup(client => client.GetAsync<Model>("/v1/models/gemini-1.5-flash", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedModel);

            // Act
            var result = await service.GetAsync("gemini-1.5-flash");

            // Assert
            Assert.Same(expectedModel, result);
            _mockApiClient.Verify(client => client.GetAsync<Model>("/v1/models/gemini-1.5-flash", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldCallApiClientWithCorrectEndpoint_WhenUseVertexAIIsTrue()
        {
            // Arrange
            var options = new GeminiClientOptions
            {
                UseVertexAI = true,
                ProjectId = "my-project",
                Location = "us-central1"
            };
            _mockOptions.Setup(o => o.Value).Returns(options);
            var service = new ModelsService(_mockApiClient.Object, _mockOptions.Object, _mockLogger.Object);

            var expectedModel = new Model { Name = "models/gemini-1.5-flash", DisplayName = "Gemini 1.5 Flash" };
            var expectedEndpoint = "/v1/projects/my-project/locations/us-central1/publishers/google/models/gemini-1.5-flash";

            _mockApiClient.Setup(client => client.GetAsync<Model>(expectedEndpoint, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedModel);

            // Act
            var result = await service.GetAsync("gemini-1.5-flash");

            // Assert
            Assert.Same(expectedModel, result);
            _mockApiClient.Verify(client => client.GetAsync<Model>(expectedEndpoint, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
