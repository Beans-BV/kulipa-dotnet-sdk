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
                Type = CardType.Virtual,
                UserId = "usr-123e4567-e89b-12d3-a456-426614174000",
                Tier = CardTier.Standard,
                CurrencyCode = "USD"
            };

            var expectedCard = new Card
            {
                Id = "crd-987f6543-e21b-43d2-b123-426614174111",
                UserId = request.UserId,
                Type = CardType.Virtual,
                Status = CardStatus.Active,
                LastFourDigits = "1234",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Expiration = null!,
                EmbossedName = null!
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
            result.Type.Should().Be(expectedCard.Type);
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
                Type = CardType.Physical,
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
                    Content = new StringContent(JsonSerializer.Serialize(new Card
                    {
                        Id = null!,
                        UserId = null!,
                        LastFourDigits = null!,
                        Expiration = null!,
                        EmbossedName = null!
                    }))
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
                Type = CardType.Virtual,
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
                Type = CardType.Virtual,
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
                FrozenBy = FrozenBy.User,
                UserId = null!,
                LastFourDigits = null!,
                Expiration = null!,
                EmbossedName = null!
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
                    new()
                    {
                        Id = "crd-1",
                        Status = CardStatus.Active,
                        UserId = null!,
                        LastFourDigits = null!,
                        Expiration = null!,
                        EmbossedName = null!
                    },
                    new()
                    {
                        Id = "crd-2",
                        Status = CardStatus.Inactive,
                        UserId = null!,
                        LastFourDigits = null!,
                        Expiration = null!,
                        EmbossedName = null!
                    }
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
                FrozenBy = FrozenBy.User,
                UserId = null!,
                LastFourDigits = null!,
                Expiration = null!,
                EmbossedName = null!
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
        public async Task ReissueAsync_WithValidRequest_ReturnsNewCard()
        {
            // Arrange
            var cardId = "crd-123e4567-e89b-12d3-a456-426614174000";
            var request = new ReissueCardRequest
            {
                Reason = ReissueReason.Lost
            };

            var expectedCard = new Card
            {
                Id = "crd-987f6543-e21b-43d2-b123-426614174222",
                UserId = "usr-123e4567-e89b-12d3-a456-426614174000",
                Type = CardType.Virtual,
                Status = CardStatus.Active,
                LastFourDigits = "5678",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Expiration = null!,
                EmbossedName = null!
            };

            var responseJson = JsonSerializer.Serialize(expectedCard);
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
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.PathAndQuery == $"/cards/{cardId}/reissue"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _cardsResource.ReissueAsync(cardId, request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedCard.Id);
            result.UserId.Should().Be(expectedCard.UserId);
            result.Type.Should().Be(expectedCard.Type);
            result.Status.Should().Be(CardStatus.Active);
            result.LastFourDigits.Should().Be("5678");
        }

        [TestMethod]
        public async Task ReissueAsync_WithStolenReason_ReturnsNewCard()
        {
            // Arrange
            var cardId = "crd-123e4567-e89b-12d3-a456-426614174000";
            var request = new ReissueCardRequest
            {
                Reason = ReissueReason.Stolen
            };

            var expectedCard = new Card
            {
                Id = "crd-987f6543-e21b-43d2-b123-426614174333",
                UserId = "usr-123e4567-e89b-12d3-a456-426614174000",
                Type = CardType.Physical,
                Status = CardStatus.Inactive,
                LastFourDigits = "9999",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Expiration = null!,
                EmbossedName = null!
            };

            var responseJson = JsonSerializer.Serialize(expectedCard);
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
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.PathAndQuery == $"/cards/{cardId}/reissue"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _cardsResource.ReissueAsync(cardId, request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedCard.Id);
            result.Type.Should().Be(CardType.Physical);
            result.Status.Should().Be(CardStatus.Inactive);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task ReissueAsync_WithInvalidCardId_ThrowsArgumentException(string invalidId)
        {
            // Arrange
            var request = new ReissueCardRequest
            {
                Reason = ReissueReason.Lost
            };

            // Act & Assert
            await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
                _cardsResource.ReissueAsync(invalidId, request));
        }

        [TestMethod]
        public async Task ReissueAsync_WithNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            var cardId = "crd-123e4567-e89b-12d3-a456-426614174000";

            // Act & Assert
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() =>
                _cardsResource.ReissueAsync(cardId, null!));
        }

        [TestMethod]
        public async Task ReissueAsync_WithIdempotencyKey_AddsToRequestOptions()
        {
            // Arrange
            var cardId = "crd-123e4567-e89b-12d3-a456-426614174000";
            var request = new ReissueCardRequest
            {
                Reason = ReissueReason.Lost
            };
            var idempotencyKey = "test-idempotency-key-reissue";
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
                    Content = new StringContent(JsonSerializer.Serialize(new Card
                    {
                        Id = "crd-new",
                        UserId = null!,
                        LastFourDigits = null!,
                        Expiration = null!,
                        EmbossedName = null!
                    }))
                });

            // Act
            await _cardsResource.ReissueAsync(cardId, request, idempotencyKey);

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
        public async Task ReissueAsync_WithValidationError_ThrowsValidationException()
        {
            // Arrange
            var cardId = "crd-123e4567-e89b-12d3-a456-426614174000";
            var request = new ReissueCardRequest
            {
                Reason = ReissueReason.Lost
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Digital cards cannot be reissued")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act & Assert
            await Assert.ThrowsExactlyAsync<KulipaValidationException>(() =>
                _cardsResource.ReissueAsync(cardId, request));
        }

        [TestMethod]
        public async Task GetSpendingControlsAsync_WithValidId_ReturnsSpendingControls()
        {
            // Arrange
            var cardId = "crd-123e4567-e89b-12d3-a456-426614174000";
            var expectedControls = new List<SpendingControlUsage>
            {
                new()
                {
                    Id = "scl-111e4567-e89b-12d3-a456-426614174000",
                    Description = "Daily spending limit",
                    Type = SpendingControlType.Purchase,
                    Config = new SpendingControlConfigResponse
                    {
                        Period = SpendingControlPeriod.Daily,
                        Limit = 10000
                    },
                    AvailableAmount = 7500
                },
                new()
                {
                    Id = "scl-222e4567-e89b-12d3-a456-426614174000",
                    Description = "Block gambling merchants",
                    Type = SpendingControlType.BlockedMcc,
                    Config = new SpendingControlConfigResponse
                    {
                        Values = new List<string> { "7995", "7800" }
                    }
                }
            };

            var responseJson = JsonSerializer.Serialize(expectedControls);
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
                        req.RequestUri!.PathAndQuery == $"/cards/{cardId}/spending-controls"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _cardsResource.GetSpendingControlsAsync(cardId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            result[0].Id.Should().Be("scl-111e4567-e89b-12d3-a456-426614174000");
            result[0].Type.Should().Be(SpendingControlType.Purchase);
            result[0].Config.Period.Should().Be(SpendingControlPeriod.Daily);
            result[0].Config.Limit.Should().Be(10000);
            result[0].AvailableAmount.Should().Be(7500);

            result[1].Id.Should().Be("scl-222e4567-e89b-12d3-a456-426614174000");
            result[1].Type.Should().Be(SpendingControlType.BlockedMcc);
            result[1].Config.Values.Should().BeEquivalentTo(new[] { "7995", "7800" });
            result[1].AvailableAmount.Should().BeNull();
        }

        [TestMethod]
        public async Task GetSpendingControlsAsync_WithEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var cardId = "crd-123e4567-e89b-12d3-a456-426614174000";
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.PathAndQuery == $"/cards/{cardId}/spending-controls"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _cardsResource.GetSpendingControlsAsync(cardId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task GetSpendingControlsAsync_WithInvalidId_ThrowsArgumentException(string invalidId)
        {
            // Act & Assert
            await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
                _cardsResource.GetSpendingControlsAsync(invalidId));
        }

        [TestMethod]
        public async Task CreateSpendingControlAsync_WithPurchaseLimit_ReturnsSpendingControlSetting()
        {
            // Arrange
            var cardId = "crd-123e4567-e89b-12d3-a456-426614174000";
            var request = new CreateSpendingControlRequest
            {
                Description = "Daily spending limit",
                Type = SpendingControlType.Purchase,
                Config = SpendingControlConfig.ForPurchaseLimit(SpendingControlPeriod.Daily, 10000)
            };

            var expectedResponse = new SpendingControlSetting
            {
                Id = "scl-111e4567-e89b-12d3-a456-426614174000",
                Description = request.Description,
                Type = SpendingControlType.Purchase,
                Config = new SpendingControlConfigResponse
                {
                    Period = SpendingControlPeriod.Daily,
                    Limit = 10000
                }
            };

            var responseJson = JsonSerializer.Serialize(expectedResponse);
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
                        req.RequestUri!.PathAndQuery == $"/cards/{cardId}/spending-controls"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _cardsResource.CreateSpendingControlAsync(cardId, request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedResponse.Id);
            result.Description.Should().Be(request.Description);
            result.Type.Should().Be(SpendingControlType.Purchase);
            result.Config.Period.Should().Be(SpendingControlPeriod.Daily);
            result.Config.Limit.Should().Be(10000);
        }

        [TestMethod]
        public async Task CreateSpendingControlAsync_WithBlockedMcc_ReturnsSpendingControlSetting()
        {
            // Arrange
            var cardId = "crd-123e4567-e89b-12d3-a456-426614174000";
            var mccCodes = new[] { "7995", "7800", "7801" };
            var request = new CreateSpendingControlRequest
            {
                Description = "Block gambling merchants",
                Type = SpendingControlType.BlockedMcc,
                Config = SpendingControlConfig.ForBlockedMcc(mccCodes)
            };

            var expectedResponse = new SpendingControlSetting
            {
                Id = "scl-222e4567-e89b-12d3-a456-426614174000",
                Description = request.Description,
                Type = SpendingControlType.BlockedMcc,
                Config = new SpendingControlConfigResponse
                {
                    Values = mccCodes.ToList()
                }
            };

            var responseJson = JsonSerializer.Serialize(expectedResponse);
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
                        req.RequestUri!.PathAndQuery == $"/cards/{cardId}/spending-controls"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _cardsResource.CreateSpendingControlAsync(cardId, request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedResponse.Id);
            result.Description.Should().Be(request.Description);
            result.Type.Should().Be(SpendingControlType.BlockedMcc);
            result.Config.Values.Should().BeEquivalentTo(mccCodes);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task CreateSpendingControlAsync_WithInvalidCardId_ThrowsArgumentException(string invalidId)
        {
            // Arrange
            var request = new CreateSpendingControlRequest
            {
                Description = "Test",
                Type = SpendingControlType.Purchase,
                Config = SpendingControlConfig.ForPurchaseLimit(SpendingControlPeriod.Daily, 1000)
            };

            // Act & Assert
            await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
                _cardsResource.CreateSpendingControlAsync(invalidId, request));
        }

        [TestMethod]
        public async Task CreateSpendingControlAsync_WithNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            var cardId = "crd-123e4567-e89b-12d3-a456-426614174000";

            // Act & Assert
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() =>
                _cardsResource.CreateSpendingControlAsync(cardId, null!));
        }

        [TestMethod]
        public async Task CreateSpendingControlAsync_WithIdempotencyKey_AddsToRequestOptions()
        {
            // Arrange
            var cardId = "crd-123e4567-e89b-12d3-a456-426614174000";
            var request = new CreateSpendingControlRequest
            {
                Description = "Daily limit",
                Type = SpendingControlType.Purchase,
                Config = SpendingControlConfig.ForPurchaseLimit(SpendingControlPeriod.Daily, 5000)
            };
            var idempotencyKey = "test-idempotency-key-spending";
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
                    Content = new StringContent(JsonSerializer.Serialize(new SpendingControlSetting
                    {
                        Id = "scl-123",
                        Description = "Test",
                        Config = new SpendingControlConfigResponse()
                    }))
                });

            // Act
            await _cardsResource.CreateSpendingControlAsync(cardId, request, idempotencyKey);

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
                    Type = CardType.Virtual,
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