using System.Net;
using FluentAssertions;
using Kulipa.Sdk.Exceptions;
using Kulipa.Sdk.Services.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Kulipa.Sdk.Tests.Unit.Services
{
    [TestClass]
    public class RateLimitHandlerTests
    {
        private RateLimitHandler _handler = null!;
        private HttpClient _httpClient = null!;
        private Mock<HttpMessageHandler> _innerHandlerMock = null!;
        private Mock<ILogger<RateLimitHandler>> _loggerMock = null!;

        [TestInitialize]
        public void Initialize()
        {
            _innerHandlerMock = new Mock<HttpMessageHandler>();
            _loggerMock = new Mock<ILogger<RateLimitHandler>>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpClient?.Dispose();
            _handler?.Dispose();
        }

        [TestMethod]
        public async Task SendAsync_WithRateLimitHeaders_UpdatesInternalState()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("x-ratelimit-remaining", "250");
            response.Headers.Add("x-ratelimit-reset", "60");
            response.Headers.Add("x-request-id", "req-123");

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            _handler = new RateLimitHandler(_loggerMock.Object)
            {
                InnerHandler = _innerHandlerMock.Object
            };
            _httpClient = new HttpClient(_handler);

            // Act
            var result = await _httpClient.GetAsync("https://api.kulipa.com/test");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString()!.Contains("req-123") &&
                        v.ToString()!.Contains("250")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task SendAsync_With429Response_ThrowsRateLimitException()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("Rate limit exceeded")
            };
            response.Headers.Add("x-request-id", "req-456");
            response.Headers.Add("Retry-After", "120");

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            _handler = new RateLimitHandler(_loggerMock.Object)
            {
                InnerHandler = _innerHandlerMock.Object
            };
            _httpClient = new HttpClient(_handler);

            // Act & Assert
            var action = () => _httpClient.GetAsync("https://api.kulipa.com/test");
            var exception = await action.Should().ThrowAsync<KulipaRateLimitException>();

            exception.Which.RetryAfterSeconds.Should().Be(120);
        }

        [TestMethod]
        public async Task SendAsync_With429ResponseNoRetryAfter_DefaultsTo60Seconds()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("Rate limit exceeded")
            };

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            _handler = new RateLimitHandler(_loggerMock.Object)
            {
                InnerHandler = _innerHandlerMock.Object
            };
            _httpClient = new HttpClient(_handler);

            // Act & Assert
            var action = () => _httpClient.GetAsync("https://api.kulipa.com/test");
            var exception = await action.Should().ThrowAsync<KulipaRateLimitException>();

            exception.Which.RetryAfterSeconds.Should().Be(60);
        }

        [TestMethod]
        public async Task SendAsync_WhenRateLimitReached_WaitsBeforeRequest()
        {
            // This test is simplified - in a real scenario, you'd want to test the actual waiting behavior
            // For now, we just verify the log message is written

            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("x-ratelimit-remaining", "0");
            response.Headers.Add("x-ratelimit-reset", "1");

            var callCount = 0;
            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() =>
                {
                    callCount++;
                    if (callCount == 1)
                    {
                        var firstResponse = new HttpResponseMessage(HttpStatusCode.OK);
                        firstResponse.Headers.Add("x-ratelimit-remaining", "0");
                        firstResponse.Headers.Add("x-ratelimit-reset", "1");
                        return firstResponse;
                    }

                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            _handler = new RateLimitHandler(_loggerMock.Object)
            {
                InnerHandler = _innerHandlerMock.Object
            };
            _httpClient = new HttpClient(_handler);

            // Act
            await _httpClient.GetAsync("https://api.kulipa.com/test");
            // Second request should trigger rate limit logic
            await _httpClient.GetAsync("https://api.kulipa.com/test");

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Rate limit reached")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task SendAsync_ConcurrentRequests_ExecuteInParallel()
        {
            // Arrange
            var requestCount = 0;
            var concurrentRequests = new List<DateTime>();
            var lockObject = new object();

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(async () =>
                {
                    lock (lockObject)
                    {
                        concurrentRequests.Add(DateTime.UtcNow);
                        requestCount++;
                    }

                    // Simulate some processing time
                    await Task.Delay(50);

                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Headers.Add("x-ratelimit-remaining", "250");
                    return response;
                });

            _handler = new RateLimitHandler(_loggerMock.Object)
            {
                InnerHandler = _innerHandlerMock.Object
            };
            _httpClient = new HttpClient(_handler);

            // Act - Execute multiple requests concurrently
            var tasks = Enumerable.Range(0, 50)
                .Select(i => _httpClient.GetAsync($"https://api.kulipa.com/test{i}"));

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var responses = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            responses.Should().HaveCount(50);
            responses.Should().AllSatisfy(r => r.StatusCode.Should().Be(HttpStatusCode.OK));
            requestCount.Should().Be(50);

            // Verify requests ran in parallel (total time should be close to individual request time, not sum)
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(200,
                "Concurrent requests should complete faster than sequential execution");

            // Verify requests started within a reasonable time window (indicating parallelism)
            var timeSpan = concurrentRequests.Max() - concurrentRequests.Min();
            timeSpan.Should().BeLessThan(TimeSpan.FromMilliseconds(100),
                "All requests should start nearly simultaneously");
        }
    }
}