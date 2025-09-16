using System.Net;
using System.Text;
using System.Text.Json;
using Kulipa.Sdk.Configuration;
using Kulipa.Sdk.Models.Webhooks;
using Kulipa.Sdk.Webhooks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Kulipa.Sdk.Tests.Webhooks
{
    [TestClass]
    public class MemoryPublicKeyCacheTests
    {
        private MemoryPublicKeyCache _cache;
        private HttpClient _httpClient;
        private Mock<HttpMessageHandler> _mockHttpHandler;
        private Mock<ILogger<MemoryPublicKeyCache>> _mockLogger;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<MemoryPublicKeyCache>>();
            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpHandler.Object)
            {
                BaseAddress = new Uri("https://api.kulipa.com")
            };
            var mockOptions = new Mock<IOptions<KulipaSdkOptions>>();
            mockOptions.Setup(x => x.Value).Returns(new KulipaSdkOptions());

            _cache = new MemoryPublicKeyCache(_httpClient, _mockLogger.Object, mockOptions.Object);
        }

        [TestMethod]
        public async Task GetKeyAsync_FetchesFromApi_AndCaches()
        {
            // Arrange
            var keyId = "test-key-id";
            var webhookKey = new WebhookKey
            {
                Id = keyId,
                Algorithm = "ECDSA_SHA_256",
                PublicKey = "test-public-key",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var json = JsonSerializer.Serialize(webhookKey);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act - First call should fetch from API
            var result1 = await _cache.GetKeyAsync(keyId);

            // Act - Second call should use cache
            var result2 = await _cache.GetKeyAsync(keyId);

            // Assert
            Assert.IsNotNull(result1);
            Assert.AreEqual(keyId, result1.Id);
            Assert.AreEqual("ECDSA_SHA_256", result1.Algorithm);
            Assert.AreEqual("test-public-key", result1.PublicKey);

            Assert.IsNotNull(result2);
            Assert.AreEqual(result1.Id, result2.Id);

            // Verify HTTP was called only once (second call used cache)
            _mockHttpHandler.Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString().Contains($"/v1/webhooks/keys/{keyId}")),
                    ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task GetKeyAsync_ReturnsNull_WhenApiFails()
        {
            // Arrange
            var keyId = "test-key-id";
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await _cache.GetKeyAsync(keyId);

            // Assert
            Assert.IsNull(result);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to fetch public key")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetKeyAsync_ReturnsNull_ForEmptyKeyId()
        {
            // Act
            var result = await _cache.GetKeyAsync("");

            // Assert
            Assert.IsNull(result);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("null or empty ID")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetKeyAsync_RefetchesAfterExpiration()
        {
            // Arrange
            var keyId = "test-key-id";
            var shortExpirationOptions = new KulipaSdkOptions
            {
                WebhookKeyCacheExpiration = TimeSpan.FromMilliseconds(100)
            };
            var mockOptions = new Mock<IOptions<KulipaSdkOptions>>();
            mockOptions.Setup(x => x.Value).Returns(shortExpirationOptions);

            var cache = new MemoryPublicKeyCache(_httpClient, _mockLogger.Object, mockOptions.Object);

            var webhookKey = new WebhookKey
            {
                Id = keyId,
                Algorithm = "ECDSA_SHA_256",
                PublicKey = "test-public-key",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var json = JsonSerializer.Serialize(webhookKey);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result1 = await cache.GetKeyAsync(keyId);

            // Wait for cache to expire
            await Task.Delay(150);

            var result2 = await cache.GetKeyAsync(keyId);

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);

            // Verify HTTP was called twice (cache expired)
            _mockHttpHandler.Protected()
                .Verify(
                    "SendAsync",
                    Times.Exactly(2),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString().Contains($"/v1/webhooks/keys/{keyId}")),
                    ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task InvalidateKey_RemovesFromCache()
        {
            // Arrange
            var keyId = "test-key-id";
            var webhookKey = new WebhookKey
            {
                Id = keyId,
                Algorithm = "ECDSA_SHA_256",
                PublicKey = "test-public-key",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var json = JsonSerializer.Serialize(webhookKey);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result1 = await _cache.GetKeyAsync(keyId);
            _cache.InvalidateKey(keyId);
            var result2 = await _cache.GetKeyAsync(keyId);

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);

            // Verify HTTP was called twice (cache was invalidated)
            _mockHttpHandler.Protected()
                .Verify(
                    "SendAsync",
                    Times.Exactly(2),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString().Contains($"/v1/webhooks/keys/{keyId}")),
                    ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task Clear_RemovesAllCachedKeys()
        {
            // Arrange
            var keyId1 = "test-key-id-1";
            var keyId2 = "test-key-id-2";

            var webhookKey1 = new WebhookKey { Id = keyId1, Algorithm = "ECDSA_SHA_256", PublicKey = "key1" };
            var webhookKey2 = new WebhookKey { Id = keyId2, Algorithm = "ECDSA_SHA_256", PublicKey = "key2" };

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains(keyId1)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(webhookKey1), Encoding.UTF8,
                        "application/json")
                });

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains(keyId2)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(webhookKey2), Encoding.UTF8,
                        "application/json")
                });

            // Act
            await _cache.GetKeyAsync(keyId1);
            await _cache.GetKeyAsync(keyId2);
            _cache.Clear();
            await _cache.GetKeyAsync(keyId1);
            await _cache.GetKeyAsync(keyId2);

            // Assert - Each key should be fetched twice (once before clear, once after)
            _mockHttpHandler.Protected()
                .Verify(
                    "SendAsync",
                    Times.Exactly(4),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task GetKeyAsync_HandlesHttpRequestException()
        {
            // Arrange
            var keyId = "test-key-id";

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _cache.GetKeyAsync(keyId);

            // Assert
            Assert.IsNull(result);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("HTTP error")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetKeyAsync_HandlesTaskCanceledException()
        {
            // Arrange
            var keyId = "test-key-id";

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new TaskCanceledException("Request timeout"));

            // Act
            var result = await _cache.GetKeyAsync(keyId);

            // Assert
            Assert.IsNull(result);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Request timeout")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}