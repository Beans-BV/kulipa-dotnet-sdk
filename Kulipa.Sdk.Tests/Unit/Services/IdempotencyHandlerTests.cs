using System.Net;
using FluentAssertions;
using Kulipa.Sdk.Configuration;
using Kulipa.Sdk.Services.Http;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Polly;

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
            _httpClient.Dispose();
            _handler.Dispose();
        }

        [TestMethod]
        public async Task SendAsync_PostRequestWithIdempotencyKey_AddsHeader()
        {
            // Arrange
            const string idempotencyKey = "test-idempotency-123";
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
            const string idempotencyKey = "test-idempotency-456";
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
            generatedKey.Length.Should().Be(64);
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
            const string existingKey = "existing-key-789";
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

        [TestMethod]
        public async Task SendAsync_OnRetry_UsesConsistentIdempotencyKey()
        {
            // Arrange
            const string idempotencyKey = "test-idempotency-123";
            string? capturedIdempotencyKey1 = null;
            string? capturedIdempotencyKey2 = null;
            var callCount = 0;

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken _) =>
                {
                    callCount++;

                    // Capture idempotency key from each request
                    if (callCount == 1)
                    {
                        capturedIdempotencyKey1 = request.Headers
                            .GetValues("x-idempotency-key")
                            .FirstOrDefault();

                        // First request returns HTTP 408 to trigger retry
                        var response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
                        return response;
                    }
                    else
                    {
                        capturedIdempotencyKey2 = request.Headers
                            .GetValues("x-idempotency-key")
                            .FirstOrDefault();

                        // Second request succeeds
                        var response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Headers.Add("x-ratelimit-remaining", "250");
                        response.Headers.Add("x-ratelimit-reset", "60");
                        return response;
                    }
                });
            // Create Polly retry handler
            var retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.RequestTimeout)
                .RetryAsync(2);

            var pollyHandler = new PolicyHttpMessageHandler(retryPolicy)
            {
                InnerHandler = _innerHandlerMock.Object
            };

            _handler = new IdempotencyHandler(Options.Create(_options))
            {
                InnerHandler = pollyHandler
            };

            _httpClient = new HttpClient(_handler);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.kulipa.com/cards");
            request.Options.Set(new HttpRequestOptionsKey<string>("IdempotencyKey"), idempotencyKey);
            // Act
            await _httpClient.SendAsync(request);

            // Assert
            callCount.Should().Be(2, "Should retry after receiving 408");

            capturedIdempotencyKey1.Should().NotBeNullOrEmpty();
            capturedIdempotencyKey2.Should().NotBeNullOrEmpty();
            capturedIdempotencyKey1.Should()
                .Be(capturedIdempotencyKey2,
                    "idempotency key should remain the same across retries");

            _innerHandlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Exactly(2),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task SendAsync_WithTooLongKey_ThrowsArgumentException()
        {
            // Arrange
            const string idempotencyKey = "this-is-an-idempotency-key-with-over-64-characters-which-is-invalid";

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _handler = new IdempotencyHandler(Options.Create(_options))
            {
                InnerHandler = _innerHandlerMock.Object
            };
            _httpClient = new HttpClient(_handler);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.kulipa.com/cards");
            request.Options.Set(new HttpRequestOptionsKey<string>("IdempotencyKey"), idempotencyKey);

            // Act && assert
            var action = () => _httpClient.SendAsync(request);

            var exception = await action.Should().ThrowAsync<ArgumentException>();

            exception.Which.Message.Should().Contain("Idempotency key exceeds maximum length");
        }
    }
}