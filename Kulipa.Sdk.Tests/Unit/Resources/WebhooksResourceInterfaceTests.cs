using Kulipa.Sdk.Configuration;
using Kulipa.Sdk.Core;
using Kulipa.Sdk.Models.Webhooks;
using Kulipa.Sdk.Resources;
using Kulipa.Sdk.Webhooks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Kulipa.Sdk.Tests.Unit.Resources
{
    [TestClass]
    public class WebhooksResourceInterfaceTests
    {
        [TestMethod]
        public void WebhooksResource_ImplementsIWebhooksResource()
        {
            // Arrange
            var httpClient = new HttpClient();
            var mockVerifier = new Mock<IWebhookVerifier>();
            var mockCache = new Mock<IPublicKeyCache>();
            var logger = new Mock<ILogger<WebhooksResource>>().Object;

            // Act
            var resource = new WebhooksResource(httpClient, mockVerifier.Object, mockCache.Object, logger);

            // Assert
            Assert.IsInstanceOfType(resource, typeof(IWebhooksResource));
        }

        [TestMethod]
        public void WebhooksResource_CanBeUsedViaInterface()
        {
            // Arrange
            var httpClient = new HttpClient { BaseAddress = new Uri("https://api.kulipa.com") };
            var mockVerifier = new Mock<IWebhookVerifier>();
            var mockCache = new Mock<IPublicKeyCache>();
            var logger = new Mock<ILogger<WebhooksResource>>().Object;

            mockVerifier.Setup(x => x.VerifyWebhookAsync(
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<string>(),
                    CancellationToken.None))
                .ReturnsAsync(WebhookVerificationResult.Success());

            IWebhooksResource resource =
                new WebhooksResource(httpClient, mockVerifier.Object, mockCache.Object, logger);

            // Act & Assert - Verify all interface methods are accessible
            Assert.IsNotNull(resource);

            // These would throw if not properly implemented
            var headers = new Dictionary<string, string>();
            var rawBody = "{}";

            // Verify methods can be called (they'll fail due to no setup, but that's ok for this test)
            Assert.IsNotNull(resource.VerifyWebhookAsync(headers, rawBody));

            // Verify cache management methods
            resource.InvalidateCachedKey("test-key");
            resource.ClearKeyCache();
        }

        [TestMethod]
        public async Task WebhooksResource_MockImplementation_Works()
        {
            // This test demonstrates that the interface can be mocked for testing

            // Arrange
            var mockResource = new Mock<IWebhooksResource>();
            var expectedResult = WebhookVerificationResult.Success();

            mockResource
                .Setup(x => x.VerifyWebhookAsync(
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<string>(),
                    CancellationToken.None))
                .ReturnsAsync(expectedResult);

            var resource = mockResource.Object;

            // Act
            var result = await resource.VerifyWebhookAsync(
                new Dictionary<string, string>(),
                "test-body");

            // Assert
            Assert.IsTrue(result.IsValid);
            mockResource.Verify(x => x.VerifyWebhookAsync(
                It.IsAny<IDictionary<string, string>>(),
                It.IsAny<string>(),
                CancellationToken.None), Times.Once);
        }

        [TestMethod]
        public void WebhooksResource_InterfaceExposesAllNecessaryMethods()
        {
            // This test ensures the interface has all the methods we need
            var interfaceType = typeof(IWebhooksResource);

            // Check for VerifySignatureAsync
            var verifySignatureMethod = interfaceType.GetMethod("VerifySignatureAsync");
            Assert.IsNotNull(verifySignatureMethod, "VerifySignatureAsync method should exist");

            // Check for VerifyWebhookAsync overloads
            var verifyWebhookMethods = interfaceType.GetMethods()
                .Where(m => m.Name == "VerifyWebhookAsync")
                .ToList();
            Assert.AreEqual(2, verifyWebhookMethods.Count, "Should have 2 VerifyWebhookAsync overloads");

            // Check for cache management methods
            var invalidateMethod = interfaceType.GetMethod("InvalidateCachedKey");
            Assert.IsNotNull(invalidateMethod, "InvalidateCachedKey method should exist");

            var clearMethod = interfaceType.GetMethod("ClearKeyCache");
            Assert.IsNotNull(clearMethod, "ClearKeyCache method should exist");
        }

        [TestMethod]
        public void KulipaClient_ExposesWebhooksResourceViaInterface()
        {
            // Arrange
            var mockHttpClient = new HttpClient();
            var mockOptions = new Mock<IOptions<KulipaSdkOptions>>();
            var mockLogger = new Mock<ILogger<KulipaClient>>();
            var mockCards = new Mock<ICardsResource>();
            var mockWebhooks = new Mock<IWebhooksResource>();

            mockOptions.Setup(x => x.Value)
                .Returns(new KulipaSdkOptions
                {
                    ApiKey = "test-key",
                    BaseUrl = "https://api.kulipa.com",
                    Environment = KulipaEnvironment.Production
                });

            var client = new KulipaClient(
                mockHttpClient,
                mockOptions.Object,
                mockLogger.Object,
                mockCards.Object,
                mockWebhooks.Object);

            // Act
            var webhooks = client.Webhooks;

            // Assert
            Assert.IsNotNull(webhooks);
            Assert.IsInstanceOfType(webhooks, typeof(IWebhooksResource));
        }
    }
}