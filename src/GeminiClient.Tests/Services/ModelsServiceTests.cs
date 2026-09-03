using GoogleGenAI.Client.Services;
using GoogleGenAI.Client.Configuration;
using GoogleGenAI.Client.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GeminiClient.Tests.Services;

public class ModelsServiceTests
{
    private readonly Mock<IApiClient> _apiClientMock;
    private readonly Mock<ILogger<ModelsService>> _loggerMock;

    public ModelsServiceTests()
    {
        _apiClientMock = new Mock<IApiClient>();
        _loggerMock = new Mock<ILogger<ModelsService>>();
    }

    [Fact]
    public async Task ListAsync_WhenUseVertexAIIsFalse_CallsCorrectEndpointAndReturnsModels()
    {
        // Arrange
        var expectedModels = new List<Model>
        {
            new Model { Name = "models/gemini-1.5-pro", DisplayName = "Gemini 1.5 Pro" },
            new Model { Name = "models/gemini-1.5-flash", DisplayName = "Gemini 1.5 Flash" }
        };
        var expectedResponse = new ModelsListResponse { Models = expectedModels };

        _apiClientMock
            .Setup(c => c.GetAsync<ModelsListResponse>("/v1/models", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var options = new GeminiClientOptions { UseVertexAI = false, ApiVersion = "v1" };
        var optionsMock = new Mock<IOptions<GeminiClientOptions>>();
        optionsMock.Setup(o => o.Value).Returns(options);

        var service = new ModelsService(_apiClientMock.Object, optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await service.ListAsync();

        // Assert
        _apiClientMock.Verify(c => c.GetAsync<ModelsListResponse>("/v1/models", It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(expectedModels.Count, result.Count());
        Assert.Equal(expectedModels[0].Name, result.ElementAt(0).Name);
        Assert.Equal(expectedModels[1].Name, result.ElementAt(1).Name);
    }

    [Fact]
    public async Task ListAsync_WhenUseVertexAIIsTrue_CallsCorrectEndpointAndReturnsModels()
    {
        // Arrange
        var expectedModels = new List<Model>
        {
            new Model { Name = "models/gemini-1.5-pro", DisplayName = "Gemini 1.5 Pro" }
        };
        var expectedResponse = new ModelsListResponse { Models = expectedModels };
        var expectedEndpoint = "/v1/projects/my-project/locations/us-central1/publishers/google/models";

        _apiClientMock
            .Setup(c => c.GetAsync<ModelsListResponse>(expectedEndpoint, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var options = new GeminiClientOptions
        {
            UseVertexAI = true,
            ProjectId = "my-project",
            Location = "us-central1"
        };
        var optionsMock = new Mock<IOptions<GeminiClientOptions>>();
        optionsMock.Setup(o => o.Value).Returns(options);

        var service = new ModelsService(_apiClientMock.Object, optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await service.ListAsync();

        // Assert
        _apiClientMock.Verify(c => c.GetAsync<ModelsListResponse>(expectedEndpoint, It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expectedModels[0].Name, result.ElementAt(0).Name);
    }

    [Fact]
    public async Task ListAsync_WhenModelsIsNull_ReturnsEmptyEnumerable()
    {
        // Arrange
        var expectedResponse = new ModelsListResponse { Models = null };

        _apiClientMock
            .Setup(c => c.GetAsync<ModelsListResponse>("/v1/models", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var options = new GeminiClientOptions { UseVertexAI = false, ApiVersion = "v1" };
        var optionsMock = new Mock<IOptions<GeminiClientOptions>>();
        optionsMock.Setup(o => o.Value).Returns(options);

        var service = new ModelsService(_apiClientMock.Object, optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await service.ListAsync();

        // Assert
        _apiClientMock.Verify(c => c.GetAsync<ModelsListResponse>("/v1/models", It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}