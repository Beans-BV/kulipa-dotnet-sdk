using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Kulipa.Sdk.Exceptions;
using Kulipa.Sdk.Models.Enums;
using Kulipa.Sdk.Models.Requests.Cards;
using Kulipa.Sdk.Models.Requests.Common;
using Kulipa.Sdk.Models.Responses.Cards;
using Kulipa.Sdk.Models.Responses.Common;
using Kulipa.Sdk.Resources;
using Moq;
using Moq.Protected;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Kulipa.Sdk.Tests.Unit.Resources
{
    [TestClass]
    public class CardsResourceTests
    {
        private CardsResource _cardsResource = null!;
        private HttpClient _httpClient = null!;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock = null!;

        [TestInitialize]
        public void Initialize()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://api.kulipa.com")
            };

            _cardsResource = new CardsResource(_httpClient);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpClient?.Dispose();
        }

        [TestMethod]
        public async Task CreateAsync_WithValidRequest_ReturnsCard()
        {
            // Arrange
            var request = new CreateCardRequest
            {
                Format = CardFormat.Virtual,
                UserId = "usr-123e4567-e89b-12d3-a456-426614174000",
                Tier = CardTier.Standard,
                CurrencyCode = "USD"
            };

            var expectedCard = new Card
            {
                Id = "crd-987f6543-e21b-43d2-b123-426614174111",
                UserId = request.UserId,
                Format = CardFormat.Virtual,
                Status = CardStatus.Active,
                LastFourDigits = "1234",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var responseJson = JsonSerializer.Serialize(expectedCard);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.PathAndQuery == "/cards"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _cardsResource.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedCard.Id);
            result.UserId.Should().Be(expectedCard.UserId);
            result.Format.Should().Be(expectedCard.Format);
            result.LastFourDigits.Should().Be(expectedCard.LastFourDigits);
        }

        [TestMethod]
        public async Task CreateAsync_WithNullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _cardsResource.CreateAsync(null!));
        }

        [TestMethod]
        public async Task CreateAsync_WithIdempotencyKey_AddsToRequestOptions()
        {
            // Arrange
            var request = new CreateCardRequest
            {
                Format = CardFormat.Physical,
                UserId = "usr-123e4567-e89b-12d3-a456-426614174000",
                Tier = CardTier.Premium,
                DeliveryType = DeliveryType.ShipToUser
            };

            var idempotencyKey = "test-idempotency-key-123";
            HttpRequestMessage? capturedRequest = null;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Created,
                    Content = new StringContent(JsonSerializer.Serialize(new Card()))
                });

            // Act
            await _cardsResource.CreateAsync(request, idempotencyKey);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Options.TryGetValue(
                    new HttpRequestOptionsKey<string>("IdempotencyKey"),
                    out var actualKey)
                .Should()
                .BeTrue();
            actualKey.Should().Be(idempotencyKey);
        }

        [TestMethod]
        public async Task CreateAsync_WithRateLimitError_ThrowsRateLimitException()
        {
            // Arrange
            var request = new CreateCardRequest
            {
                Format = CardFormat.Virtual,
                UserId = "usr-123e4567-e89b-12d3-a456-426614174000",
                Tier = CardTier.Standard
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.TooManyRequests,
                Content = new StringContent("Rate limit exceeded")
            };
            httpResponse.Headers.Add("x-request-id", "req-123");
            httpResponse.Headers.Add("Retry-After", "60");

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var exception =
                await Assert.ThrowsExactlyAsync<KulipaRateLimitException>(() => _cardsResource.CreateAsync(request));

            exception.RetryAfterSeconds.Should().Be(60);
            exception.RequestId.Should().Be(null);
        }

        [TestMethod]
        public async Task CreateAsync_WithAuthenticationError_ThrowsAuthenticationException()
        {
            // Arrange
            var request = new CreateCardRequest
            {
                Format = CardFormat.Virtual,
                UserId = "usr-123",
                Tier = CardTier.Standard
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("Invalid API key")
            };
            httpResponse.Headers.Add("x-request-id", "req-456");

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var exception =
                await Assert.ThrowsExactlyAsync<KulipaAuthenticationException>(() =>
                    _cardsResource.CreateAsync(request));

            exception.RequestId.Should().Be("req-456");
        }

        [TestMethod]
        public async Task GetAsync_WithValidId_ReturnsCard()
        {
            // Arrange
            var cardId = "crd-123e4567-e89b-12d3-a456-426614174000";
            var expectedCard = new Card
            {
                Id = cardId,
                Status = CardStatus.Frozen,
                FrozenBy = FrozenBy.User
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedCard))
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.PathAndQuery == $"/cards/{cardId}"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _cardsResource.GetAsync(cardId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cardId);
            result.Status.Should().Be(CardStatus.Frozen);
            result.FrozenBy.Should().Be(FrozenBy.User);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task GetAsync_WithInvalidId_ThrowsArgumentException(string invalidId)
        {
            // Act & Assert
            await Assert.ThrowsExactlyAsync<ArgumentException>(() => _cardsResource.GetAsync(invalidId));
        }

        [TestMethod]
        public async Task ListAsync_WithPagination_ReturnsPagedResponse()
        {
            // Arrange
            var userId = "usr-123";
            var pagedRequest = new PagedRequest
            {
                Limit = 10,
                FromPage = 0,
                SortBy = "createdAt"
            };

            var expectedResponse = new PagedResponse<Card>
            {
                Page = 0,
                Count = 2,
                HasMore = true,
                Items = new List<Card>
                {
                    new() { Id = "crd-1", Status = CardStatus.Active },
                    new() { Id = "crd-2", Status = CardStatus.Inactive }
                }
            };

            var responseJson = JsonSerializer.Serialize(expectedResponse);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.PathAndQuery.Contains("/cards") &&
                        req.RequestUri.Query.Contains($"userId={userId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _cardsResource.ListAsync(
                userId,
                pagedRequest);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result.HasMore.Should().BeTrue();
            result.Items.Should().HaveCount(2);
            result.Items[0].Id.Should().Be("crd-1");
            result.Items[1].Id.Should().Be("crd-2");
        }

        [TestMethod]
        public async Task ListAsync_WithDefaultParameters_UsesDefaults()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new PagedResponse<Card>()))
                });

            // Act
            await _cardsResource.ListAsync();

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.RequestUri!.Query.Should().Contain("limit=10");
            capturedRequest.RequestUri.Query.Should().Contain("fromPage=0");
            capturedRequest.RequestUri.Query.Should().Contain("sortBy=-createdAt");
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 1)]
        [DataRow(101, 100)]
        [DataRow(50, 50)]
        public async Task ListAsync_WithLimitValues_ClampsProperly(int inputLimit, int expectedLimit)
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            var pagedRequest = new PagedRequest { Limit = inputLimit };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new PagedResponse<Card>()))
                });

            // Act
            await _cardsResource.ListAsync(pagedRequest: pagedRequest);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.RequestUri!.Query.Should().Contain($"limit={expectedLimit}");
        }

        [TestMethod]
        public async Task FreezeAsync_WithValidId_ReturnsUpdatedCard()
        {
            // Arrange
            var cardId = "crd-123";
            var expectedCard = new Card
            {
                Id = cardId,
                Status = CardStatus.Frozen,
                FrozenBy = FrozenBy.User
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedCard))
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri!.PathAndQuery == $"/cards/{cardId}/freeze"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _cardsResource.FreezeAsync(cardId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(CardStatus.Frozen);
            result.FrozenBy.Should().Be(FrozenBy.User);
        }

        [TestMethod]
        public async Task HandleErrorResponse_WithVariousStatusCodes_ThrowsCorrectExceptions()
        {
            // Arrange
            var testCases = new[]
            {
                (HttpStatusCode.BadRequest, typeof(KulipaValidationException)),
                (HttpStatusCode.Unauthorized, typeof(KulipaAuthenticationException)),
                (HttpStatusCode.Forbidden, typeof(KulipaAuthorizationException)),
                (HttpStatusCode.InternalServerError, typeof(KulipaServerException)),
                (HttpStatusCode.BadGateway, typeof(KulipaServerException)),
                (HttpStatusCode.ServiceUnavailable, typeof(KulipaServerException)),
                (HttpStatusCode.GatewayTimeout, typeof(KulipaServerException))
            };

            foreach (var (statusCode, expectedExceptionType) in testCases)
            {
                // Arrange
                var request = new CreateCardRequest
                {
                    Format = CardFormat.Virtual,
                    UserId = "usr-123",
                    Tier = CardTier.Standard
                };

                _httpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = statusCode,
                        Content = new StringContent($"Error for {statusCode}")
                    });

                // Act
                Func<Task> act = async () => await _cardsResource.CreateAsync(request);

                // Assert
                var exception = await act.Should().ThrowAsync<KulipaException>();
                exception.Which.Should().BeOfType(expectedExceptionType);
            }
        }
    }
}