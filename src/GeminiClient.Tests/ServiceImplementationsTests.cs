using GoogleGenAI.Client;
using GoogleGenAI.Client.Configuration;
using GoogleGenAI.Client.Models;
using GoogleGenAI.Client.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq;

namespace GeminiClient.Tests;

public class ServiceImplementationsTests
{
    private readonly Mock<IApiClient> _mockApiClient;
    private readonly Mock<IOptions<GeminiClientOptions>> _mockOptions;
    private readonly Mock<ILogger<ChatsService>> _mockLogger;
    private readonly Mock<IModelsService> _mockModelsService;
    private readonly ChatsService _chatsService;

    public ServiceImplementationsTests()
    {
        _mockApiClient = new Mock<IApiClient>();

        var options = new GeminiClientOptions { ApiKey = "test-key" };
        _mockOptions = new Mock<IOptions<GeminiClientOptions>>();
        _mockOptions.Setup(o => o.Value).Returns(options);

        _mockLogger = new Mock<ILogger<ChatsService>>();
        _mockModelsService = new Mock<IModelsService>();

        _chatsService = new ChatsService(
            _mockApiClient.Object,
            _mockOptions.Object,
            _mockLogger.Object,
            _mockModelsService.Object);
    }

    [Fact]
    public async Task SendMessageAsync_ValidInput_CallsModelsServiceWithCorrectParameters()
    {
        // Arrange
        var modelName = "gemini-pro";
        var message = "Hello, world!";
        var config = new GenerateContentConfig
        {
            GenerationConfig = new GenerationConfig { Temperature = 0.7f }
        };
        var cancellationToken = new CancellationToken();

        var expectedResponse = new GenerateContentResponse();

        IEnumerable<Content>? capturedContents = null;

        _mockModelsService.Setup(s => s.GenerateContentAsync(
            modelName,
            It.IsAny<IEnumerable<Content>>(),
            config,
            cancellationToken))
            .Callback<string, IEnumerable<Content>, GenerateContentConfig?, CancellationToken>((_, contents, _, _) =>
            {
                capturedContents = contents;
            })
            .ReturnsAsync(expectedResponse);

        // Act
        await _chatsService.SendMessageAsync(modelName, message, config, cancellationToken);

        // Assert
        _mockModelsService.Verify(s => s.GenerateContentAsync(
            modelName,
            It.IsAny<IEnumerable<Content>>(),
            config,
            cancellationToken),
            Times.Once);

        Assert.NotNull(capturedContents);
        var contentList = capturedContents.ToList();
        Assert.Single(contentList);
        var content = contentList[0];
        Assert.Equal("user", content.Role);
        Assert.NotNull(content.Parts);
        Assert.Single(content.Parts);
        Assert.Equal(message, content.Parts[0].Text);
    }

    [Fact]
    public async Task SendMessageAsync_ReturnsExpectedResponse()
    {
        // Arrange
        var modelName = "gemini-pro";
        var message = "Hello, world!";
        var expectedResponse = new GenerateContentResponse();

        _mockModelsService.Setup(s => s.GenerateContentAsync(
            It.IsAny<string>(),
            It.IsAny<IEnumerable<Content>>(),
            It.IsAny<GenerateContentConfig>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _chatsService.SendMessageAsync(modelName, message);

        // Assert
        Assert.Same(expectedResponse, result);
    }
}
