using System.Net;
using FluentAssertions;
using Kulipa.Sdk.Configuration;
using Kulipa.Sdk.Core;
using Kulipa.Sdk.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace Kulipa.Sdk.Tests.Unit.Core
{
    [TestClass]
    public class KulipaClientTests
    {
        private Mock<ICardsResource> _cardsResourceMock = null!;
        private HttpClient _httpClient = null!;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock = null!;
        private Mock<ILogger<KulipaClient>> _loggerMock = null!;
        private IOptions<KulipaSdkOptions> _options = null!;
        private Mock<IWebhooksResource> _webhooksResourceMock = null!;

        [TestInitialize]
        public void Initialize()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _loggerMock = new Mock<ILogger<KulipaClient>>();
            _cardsResourceMock = new Mock<ICardsResource>();
            _webhooksResourceMock = new Mock<IWebhooksResource>();
            _options = Options.Create(new KulipaSdkOptions
            {
                ApiKey = "test-api-key",
                BaseUrl = "https://api.kulipa.com",
                Environment = KulipaEnvironment.Sandbox
            });

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri(_options.Value.BaseUrl)
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpClient?.Dispose();
        }

        [TestMethod]
        public void Constructor_WithValidParameters_InitializesSuccessfully()
        {
            // Act
            var client = new KulipaClient(
                _httpClient,
                _options,
                _loggerMock.Object,
                _cardsResourceMock.Object,
                _webhooksResourceMock.Object);

            // Assert
            client.Should().NotBeNull();
            client.Cards.Should().BeSameAs(_cardsResourceMock.Object);
        }

        [TestMethod]
        public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
        {
            // Act & Assert
            var action = () => new KulipaClient(
                null!,
                _options,
                _loggerMock.Object,
                _cardsResourceMock.Object,
                _webhooksResourceMock.Object);

            action.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("httpClient");
        }

        [TestMethod]
        public void Constructor_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            var action = () => new KulipaClient(
                _httpClient,
                null!,
                _loggerMock.Object,
                _cardsResourceMock.Object,
                _webhooksResourceMock.Object);

            action.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("options");
        }

        [TestMethod]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var action = () => new KulipaClient(
                _httpClient,
                _options,
                null!,
                _cardsResourceMock.Object,
                _webhooksResourceMock.Object);

            action.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("logger");
        }

        [TestMethod]
        public void Constructor_WithNullCardsResource_ThrowsArgumentNullException()
        {
            // Act & Assert
            var action = () => new KulipaClient(
                _httpClient,
                _options,
                _loggerMock.Object,
                null!,
                _webhooksResourceMock.Object);

            action.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("cardsResource");
        }

        [TestMethod]
        public async Task TestConnectionAsync_WithSuccessfulResponse_ReturnsTrue()
        {
            // Arrange
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.PathAndQuery == "/cards?limit=1"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{}")
                });

            var client = new KulipaClient(
                _httpClient,
                _options,
                _loggerMock.Object,
                _cardsResourceMock.Object,
                _webhooksResourceMock.Object);

            // Act
            var result = await client.TestConnectionAsync();

            // Assert
            result.Should().BeTrue();
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("succeeded")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task TestConnectionAsync_WithFailedResponse_ReturnsFalse()
        {
            // Arrange
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized
                });

            var client = new KulipaClient(
                _httpClient,
                _options,
                _loggerMock.Object,
                _cardsResourceMock.Object,
                _webhooksResourceMock.Object);

            // Act
            var result = await client.TestConnectionAsync();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task TestConnectionAsync_WithException_ReturnsFalse()
        {
            // Arrange
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            var client = new KulipaClient(
                _httpClient,
                _options,
                _loggerMock.Object,
                _cardsResourceMock.Object,
                _webhooksResourceMock.Object);

            // Act
            var result = await client.TestConnectionAsync();

            // Assert
            result.Should().BeFalse();
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("exception")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [TestMethod]
        public void Dispose_WhenCalled_DisposesSuccessfully()
        {
            // Arrange
            var client = new KulipaClient(
                _httpClient,
                _options,
                _loggerMock.Object,
                _cardsResourceMock.Object,
                _webhooksResourceMock.Object);

            // Act
            var action = () => client.Dispose();

            // Assert
            action.Should().NotThrow();
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("disposed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [TestMethod]
        public void Dispose_WhenCalledMultipleTimes_DisposesOnlyOnce()
        {
            // Arrange
            var client = new KulipaClient(
                _httpClient,
                _options,
                _loggerMock.Object,
                _cardsResourceMock.Object,
                _webhooksResourceMock.Object);

            // Act
            client.Dispose();
            client.Dispose();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("disposed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}