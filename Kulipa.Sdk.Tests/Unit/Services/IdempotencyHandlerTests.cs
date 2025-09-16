using System.Net;
using FluentAssertions;
using Kulipa.Sdk.Configuration;
using Kulipa.Sdk.Services.Http;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace Kulipa.Sdk.Tests.Unit.Services
{
    [TestClass]
    public class IdempotencyHandlerTests
    {
        private IdempotencyHandler _handler = null!;
        private HttpClient _httpClient = null!;
        private Mock<HttpMessageHandler> _innerHandlerMock = null!;
        private KulipaSdkOptions _options = null!;

        [TestInitialize]
        public void Initialize()
        {
            _innerHandlerMock = new Mock<HttpMessageHandler>();
            _options = new KulipaSdkOptions
            {
                EnableIdempotency = true,
                AutoGenerateIdempotencyKey = false
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpClient?.Dispose();
            _handler?.Dispose();
        }

        [TestMethod]
        public async Task SendAsync_PostRequestWithIdempotencyKey_AddsHeader()
        {
            // Arrange
            var idempotencyKey = "test-idempotency-123";
            HttpRequestMessage? capturedRequest = null;

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _handler = new IdempotencyHandler(Options.Create(_options))
            {
                InnerHandler = _innerHandlerMock.Object
            };
            _httpClient = new HttpClient(_handler);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.kulipa.com/cards");
            request.Options.Set(new HttpRequestOptionsKey<string>("IdempotencyKey"), idempotencyKey);

            // Act
            await _httpClient.SendAsync(request);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Headers.Should().ContainSingle(h => h.Key == "x-idempotency-key");
            capturedRequest.Headers.GetValues("x-idempotency-key").Should().ContainSingle(idempotencyKey);
        }

        [TestMethod]
        public async Task SendAsync_PutRequestWithIdempotencyKey_AddsHeader()
        {
            // Arrange
            var idempotencyKey = "test-idempotency-456";
            HttpRequestMessage? capturedRequest = null;

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _handler = new IdempotencyHandler(Options.Create(_options))
            {
                InnerHandler = _innerHandlerMock.Object
            };
            _httpClient = new HttpClient(_handler);

            var request = new HttpRequestMessage(HttpMethod.Put, "https://api.kulipa.com/cards/123");
            request.Options.Set(new HttpRequestOptionsKey<string>("IdempotencyKey"), idempotencyKey);

            // Act
            await _httpClient.SendAsync(request);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Headers.Should().ContainSingle(h => h.Key == "x-idempotency-key");
            capturedRequest.Headers.GetValues("x-idempotency-key").Should().ContainSingle(idempotencyKey);
        }

        [TestMethod]
        public async Task SendAsync_GetRequest_DoesNotAddHeader()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _handler = new IdempotencyHandler(Options.Create(_options))
            {
                InnerHandler = _innerHandlerMock.Object
            };
            _httpClient = new HttpClient(_handler);

            // Act
            await _httpClient.GetAsync("https://api.kulipa.com/cards");

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Headers.Should().NotContain(h => h.Key == "x-idempotency-key");
        }

        [TestMethod]
        public async Task SendAsync_DeleteRequest_DoesNotAddHeader()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _handler = new IdempotencyHandler(Options.Create(_options))
            {
                InnerHandler = _innerHandlerMock.Object
            };
            _httpClient = new HttpClient(_handler);

            // Act
            await _httpClient.DeleteAsync("https://api.kulipa.com/cards/123");

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Headers.Should().NotContain(h => h.Key == "x-idempotency-key");
        }

        [TestMethod]
        public async Task SendAsync_WithAutoGenerateEnabled_GeneratesKey()
        {
            // Arrange
            _options.AutoGenerateIdempotencyKey = true;
            HttpRequestMessage? capturedRequest = null;

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _handler = new IdempotencyHandler(Options.Create(_options))
            {
                InnerHandler = _innerHandlerMock.Object
            };
            _httpClient = new HttpClient(_handler);

            // Act
            await _httpClient.PostAsync("https://api.kulipa.com/cards", new StringContent("{}"));

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Headers.Should().ContainSingle(h => h.Key == "x-idempotency-key");
            var generatedKey = capturedRequest.Headers.GetValues("x-idempotency-key").First();
            generatedKey.Should().NotBeNullOrEmpty();
            generatedKey.Should().Contain("POST_");
            generatedKey.Should().Contain("/cards");
        }

        [TestMethod]
        public async Task SendAsync_WithIdempotencyDisabled_DoesNotAddHeader()
        {
            // Arrange
            _options.EnableIdempotency = false;
            HttpRequestMessage? capturedRequest = null;

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _handler = new IdempotencyHandler(Options.Create(_options))
            {
                InnerHandler = _innerHandlerMock.Object
            };
            _httpClient = new HttpClient(_handler);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.kulipa.com/cards");
            request.Options.Set(new HttpRequestOptionsKey<string>("IdempotencyKey"), "should-not-be-added");

            // Act
            await _httpClient.SendAsync(request);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Headers.Should().NotContain(h => h.Key == "x-idempotency-key");
        }

        [TestMethod]
        public async Task SendAsync_WithExistingHeader_DoesNotOverwrite()
        {
            // Arrange
            var existingKey = "existing-key-789";
            HttpRequestMessage? capturedRequest = null;

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _handler = new IdempotencyHandler(Options.Create(_options))
            {
                InnerHandler = _innerHandlerMock.Object
            };
            _httpClient = new HttpClient(_handler);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.kulipa.com/cards");
            request.Headers.Add("x-idempotency-key", existingKey);
            request.Options.Set(new HttpRequestOptionsKey<string>("IdempotencyKey"), "new-key-should-not-be-used");

            // Act
            await _httpClient.SendAsync(request);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Headers.GetValues("x-idempotency-key").Should().ContainSingle(existingKey);
        }
    }
}