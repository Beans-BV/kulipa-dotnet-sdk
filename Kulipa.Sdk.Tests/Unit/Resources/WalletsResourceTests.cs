using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Kulipa.Sdk.Exceptions;
using Kulipa.Sdk.Models.Common;
using Kulipa.Sdk.Models.Wallets;
using Kulipa.Sdk.Resources;
using Moq;
using Moq.Protected;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Kulipa.Sdk.Tests.Unit.Resources
{
    [TestClass]
    public class WalletsResourceTests
    {
        private HttpClient _httpClient = null!;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock = null!;
        private WalletsResource _walletsResource = null!;

        [TestInitialize]
        public void Initialize()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://api.kulipa.com")
            };

            _walletsResource = new WalletsResource(_httpClient);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpClient?.Dispose();
        }

        [TestMethod]
        public async Task CreateAsync_WithValidRequest_ReturnsWallet()
        {
            // Arrange
            var request = new CreateWalletRequest
            {
                Name = "Test Wallet",
                Blockchain = BlockchainNetwork.StellarTestnet,
                WithdrawalAddress = "yyy"
            };

            var expectedWallet = new Wallet
            {
                Id = "wlt-987f6543-e21b-43d2-b123-426614174111",
                UserId = "usr-2eb2681f-8fdb-4270-b753-7bc4852a9142",
                Name = request.Name,
                // Address =  TODO Unconfirmed - Update this when Kulipa updates their docs after adding support for Stellar.
                Status = WalletStatus.Unverified,
                Blockchain = request.Blockchain,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var responseJson = JsonSerializer.Serialize(expectedWallet);
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
                        req.RequestUri!.PathAndQuery == "/wallets"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _walletsResource.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedWallet.Id);
            result.UserId.Should().Be(expectedWallet.UserId);
            result.Name.Should().Be(expectedWallet.Name);
            result.Blockchain.Should().Be(expectedWallet.Blockchain);
            // TODO Unconfirmed - Update this when Kulipa updates their docs after adding support for Stellar.
            // result.Address.Should().Be(expectedWallet.Address); 
            result.Status.Should().Be(expectedWallet.Status);
        }

        [TestMethod]
        public async Task CreateAsync_WithNullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => _walletsResource.CreateAsync(null!));
        }

        [TestMethod]
        public async Task CreateAsync_WithIdempotencyKey_AddsToRequestOptions()
        {
            // Arrange
            var request = new CreateWalletRequest
            {
                Name = "Test Wallet",
                Blockchain = BlockchainNetwork.StellarTestnet,
                WithdrawalAddress = "yyy"
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
                    Content = new StringContent(JsonSerializer.Serialize(
                            new Wallet { Id = "", UserId = "" }
                        )
                    )
                });

            // Act
            await _walletsResource.CreateAsync(request, idempotencyKey);

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
            var request = new CreateWalletRequest
            {
                Name = "Test Wallet",
                Blockchain = BlockchainNetwork.StellarTestnet,
                WithdrawalAddress = "yyy"
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
                await Assert.ThrowsExactlyAsync<KulipaRateLimitException>(() => _walletsResource.CreateAsync(request));

            exception.RetryAfterSeconds.Should().Be(60);
            exception.RequestId.Should().Be(null);
        }

        [TestMethod]
        public async Task CreateAsync_WithAuthenticationError_ThrowsAuthenticationException()
        {
            // Arrange
            var request = new CreateWalletRequest
            {
                Name = "Test Wallet",
                Blockchain = BlockchainNetwork.StellarTestnet,
                WithdrawalAddress = "yyy"
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
                    _walletsResource.CreateAsync(request));

            exception.RequestId.Should().Be("req-456");
        }

        [TestMethod]
        public async Task GetAsync_WithValidId_ReturnsWallet()
        {
            // Arrange
            var walletId = "wlt-123e4567-e89b-12d3-a456-426614174000";
            var expectedWallet = new Wallet
            {
                Id = walletId,
                UserId = "usr-123e4567-e89b-12d3-a456-426614174000",
                Name = "Test wallet",
                Address = "", // TODO Unconfirmed - Update this when Kulipa updates their docs after adding support for Stellar.
                Status = WalletStatus.Frozen,
                Blockchain = BlockchainNetwork.StellarTestnet,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedWallet))
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.PathAndQuery == $"/wallets/{walletId}"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _walletsResource.GetAsync(walletId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(walletId);
            result.Status.Should().Be(expectedWallet.Status);
            result.UserId.Should().Be(expectedWallet.UserId);
            result.Name.Should().Be(expectedWallet.Name);
            result.Blockchain.Should().Be(expectedWallet.Blockchain);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task GetAsync_WithInvalidId_ThrowsArgumentException(string invalidId)
        {
            // Act & Assert
            await Assert.ThrowsExactlyAsync<ArgumentException>(() => _walletsResource.GetAsync(invalidId));
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

            var expectedResponse = new PagedResponse<Wallet>
            {
                Page = 0,
                Count = 2,
                HasMore = false,
                Items =
                [
                    new Wallet { Id = "wlt-1", UserId = "usr-1", Status = WalletStatus.Active },
                    new Wallet { Id = "wlt-2", UserId = "usr-1", Status = WalletStatus.Unverified }
                ]
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
                        req.RequestUri!.PathAndQuery.Contains("/wallets") &&
                        req.RequestUri.Query.Contains($"userId={userId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _walletsResource.ListAsync(
                userId,
                null,
                pagedRequest);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result.HasMore.Should().BeFalse();
            result.Items.Should().HaveCount(2);
            result.Items[0].Id.Should().Be("wlt-1");
            result.Items[1].Id.Should().Be("wlt-2");
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
                    Content = new StringContent(JsonSerializer.Serialize(new PagedResponse<Wallet>()))
                });

            // Act
            await _walletsResource.ListAsync();

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
                    Content = new StringContent(JsonSerializer.Serialize(new PagedResponse<Wallet>()))
                });

            // Act
            await _walletsResource.ListAsync(pagedRequest: pagedRequest);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.RequestUri!.Query.Should().Contain($"limit={expectedLimit}");
        }

        [TestMethod]
        public async Task VerifyAsync_WithValidId_ReturnsUpdatedWallet()
        {
            // Arrange
            var walletId = "wlt-123";
            var expectedWallet = new Wallet
            {
                Id = walletId,
                UserId = "usr-123e4567-e89b-12d3-a456-426614174000",
                Name = "Test wallet",
                Status = WalletStatus.Active
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedWallet))
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri!.PathAndQuery == $"/wallets/{walletId}/verify"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _walletsResource.VerifyAsync(walletId, new { });

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(WalletStatus.Active);
        }

        [TestMethod]
        public async Task ListWithdrawalsAsync_WithPagination_ReturnsPagedResponse()
        {
            // Arrange
            var walletId = "wlt-123";
            var userId = "usr-123";
            var pagedRequest = new PagedRequest
            {
                Limit = 10,
                FromPage = 0,
                SortBy = "createdAt"
            };

            var expectedResponse = new PagedResponse<Withdrawal>
            {
                Page = 0,
                Count = 5,
                HasMore = false,
                Items =
                [
                    new Withdrawal
                    {
                        Id = "wdr-1",
                        WalletId = walletId,
                        UserId = userId,
                        Status = WithdrawalStatus.Pending
                    },
                    new Withdrawal
                    {
                        Id = "wdr-2",
                        WalletId = walletId,
                        UserId = userId,
                        Status = WithdrawalStatus.Confirmed
                    },
                    new Withdrawal
                    {
                        Id = "wdr-3",
                        WalletId = walletId,
                        UserId = userId,
                        Status = WithdrawalStatus.Failed
                    },
                    new Withdrawal
                    {
                        Id = "wdr-4",
                        WalletId = walletId,
                        UserId = userId,
                        Status = WithdrawalStatus.Rejected
                    },
                    new Withdrawal
                    {
                        Id = "wdr-5",
                        WalletId = walletId,
                        UserId = userId,
                        Status = WithdrawalStatus.Draft
                    }
                ]
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
                        req.RequestUri!.PathAndQuery.Contains("/wallets")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _walletsResource.ListWithdrawalsAsync(
                walletId,
                null,
                null,
                pagedRequest);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(5);
            result.HasMore.Should().BeFalse();
            result.Items.Should().HaveCount(5);
            // TODO Add assertion for missing fields
            result.Items[0].Id.Should().Be("wdr-1");
            result.Items[0].WalletId.Should().Be(walletId);
            result.Items[0].UserId.Should().Be(userId);
            result.Items[0].Status.Should().Be(WithdrawalStatus.Pending);
            result.Items[1].Id.Should().Be("wdr-2");
            result.Items[1].WalletId.Should().Be(walletId);
            result.Items[1].UserId.Should().Be(userId);
            result.Items[1].Status.Should().Be(WithdrawalStatus.Confirmed);
            result.Items[2].Id.Should().Be("wdr-3");
            result.Items[2].WalletId.Should().Be(walletId);
            result.Items[2].UserId.Should().Be(userId);
            result.Items[2].Status.Should().Be(WithdrawalStatus.Failed);
            result.Items[3].Id.Should().Be("wdr-4");
            result.Items[3].WalletId.Should().Be(walletId);
            result.Items[3].UserId.Should().Be(userId);
            result.Items[3].Status.Should().Be(WithdrawalStatus.Rejected);
            result.Items[4].Id.Should().Be("wdr-5");
            result.Items[4].WalletId.Should().Be(walletId);
            result.Items[4].UserId.Should().Be(userId);
            result.Items[4].Status.Should().Be(WithdrawalStatus.Draft);
        }

        [TestMethod]
        public async Task ListTopUpsAsync_WithPagination_ReturnsPagedResponse()
        {
            // Arrange
            var walletId = "wlt-123";
            var userId = "usr-123";
            var pagedRequest = new PagedRequest
            {
                Limit = 10,
                FromPage = 0,
                SortBy = "createdAt"
            };

            var expectedResponse = new PagedResponse<TopUp>
            {
                Page = 0,
                Count = 3,
                HasMore = false,
                Items =
                [
                    new TopUp
                    {
                        Id = "btn-1",
                        WalletId = walletId,
                        UserId = userId,
                        Status = TopUpStatus.Confirmed
                    },
                    new TopUp
                    {
                        Id = "btn-2",
                        WalletId = walletId,
                        UserId = userId,
                        Status = TopUpStatus.Failed
                    },
                    new TopUp
                    {
                        Id = "btn-3",
                        WalletId = walletId,
                        UserId = userId,
                        Status = TopUpStatus.Confirmed
                    }
                ]
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
                        req.RequestUri!.PathAndQuery.Contains("/wallets")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _walletsResource.ListTopUpsAsync(
                walletId,
                pagedRequest);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(3);
            result.HasMore.Should().BeFalse();
            result.Items.Should().HaveCount(3);
            // TODO Add assertion for missing fields
            result.Items[0].Id.Should().Be("btn-1");
            result.Items[0].WalletId.Should().Be(walletId);
            result.Items[0].UserId.Should().Be(userId);
            result.Items[0].Status.Should().Be(TopUpStatus.Confirmed);
            result.Items[1].Id.Should().Be("btn-2");
            result.Items[1].WalletId.Should().Be(walletId);
            result.Items[1].UserId.Should().Be(userId);
            result.Items[1].Status.Should().Be(TopUpStatus.Failed);
            result.Items[2].Id.Should().Be("btn-3");
            result.Items[2].WalletId.Should().Be(walletId);
            result.Items[2].UserId.Should().Be(userId);
            result.Items[2].Status.Should().Be(TopUpStatus.Confirmed);
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
                var request = new CreateWalletRequest
                {
                    Name = "Test Wallet",
                    Blockchain = BlockchainNetwork.StellarTestnet,
                    WithdrawalAddress = "yyy"
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
                Func<Task> act = async () => await _walletsResource.CreateAsync(request);

                // Assert
                var exception = await act.Should().ThrowAsync<KulipaException>();
                exception.Which.Should().BeOfType(expectedExceptionType);
            }
        }
    }
}