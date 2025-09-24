using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Kulipa.Sdk.Configuration;
using Kulipa.Sdk.Core;
using Kulipa.Sdk.Extensions;
using Kulipa.Sdk.Models.Webhooks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Kulipa.Sdk.Tests.Integration
{
    /// <summary>
    ///     Example integration test showing how webhook verification works end-to-end.
    ///     This would typically be in a separate integration test project.
    /// </summary>
    [TestClass]
    [TestCategory("Integration")]
    public class WebhookIntegrationExample
    {
        private IKulipaClient _client;
        private MockWebhookSender _webhookSender;

        [TestInitialize]
        public void Setup()
        {
            // In a real integration test, you would use DI container
            // For this example, we'll set up manually
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());

            // Configure options
            services.AddKulipaSdk(options =>
            {
                options.ApiKey = "test-api-key"; // Need to replace with actual API key before running the tests
                options.BaseUrl = "https://api.testing.kulipa.dev/v1/";
                options.Environment = KulipaEnvironment.Sandbox;
                options.WebhookTimestampTolerance = TimeSpan.FromMinutes(5);
            });

            var serviceProvider = services.BuildServiceProvider();

            // Get client from DI container - this is how it should be used
            _client = serviceProvider.GetRequiredService<IKulipaClient>();

            // Initialize mock webhook sender for testing
            _webhookSender = new MockWebhookSender();
        }

        [TestMethod]
        public async Task VerifyWebhook_RealScenario_Success()
        {
            // Arrange - Simulate a webhook from Kulipa
            var webhookPayload = new
            {
                userId = "usr-b036777a-d790-4cd3-a3d5-3bc1982eccc2",
                eventId = "bd4624fd-ca50-4698-8558-354e522f4416",
                eventName = "card.created",
                eventTargetId = "crd-3d3bb4df-280d-45f1-807a-ae1f54506a35"
            };

            var rawBody = JsonSerializer.Serialize(webhookPayload);
            var timestamp = "1758011225399";
            var keyId = "dwk-d5418756-4908-4077-b053-3bc662820f85";

            // In a real scenario, Kulipa would sign this with their private key
            var signature =
                "304402202c1365b6bf4aadd370592c6dcd0623c5ea2f18ef22f2d1799816c3c23fa7914c022005c2dbff467214357461ba178ff9a405a525c70867346a029bba0173bb47479e";

            var headers = new Dictionary<string, string>
            {
                ["x-kulipa-signature"] = signature,
                ["x-kulipa-signature-ts"] = timestamp,
                ["x-kulipa-key-id"] = keyId
            };

            // Act - Verify the webhook
            var result = await _client.Webhooks.VerifyWebhookAsync(headers, rawBody);

            // Assert
            Assert.IsTrue(result.IsValid, "Webhook verification should succeed");

            // Process the webhook
            if (result.IsValid)
            {
                var webhook = JsonSerializer.Deserialize<dynamic>(rawBody);
                Console.WriteLine($"Successfully verified webhook: {webhook}");
            }
        }

        [TestMethod]
        public async Task VerifyWebhook_TamperedBody_FailsVerification()
        {
            // Arrange
            var originalPayload = new
            {
                userId = "usr-b036777a-d790-4cd3-a3d5-3bc1982eccc2",
                eventId = "bd4624fd-ca50-4698-8558-354e522f4416",
                eventName = "card.created",
                eventTargetId = "crd-3d3bb4df-280d-45f1-807a-ae1f54506a35"
            };
            var tamperedPayload = new
            {
                userId = "usr-b036777a-d790-4cd3-a3d5-3bc1982eccc2",
                eventId = "bd4624fd-ca50-4698-8558-354e522f4416",
                eventName = "card.created",
                eventTargetId = "crd-3d3bb4df-280d-45f1-807a-ae1f54506a34"
            };

            var originalBody = JsonSerializer.Serialize(originalPayload);
            var tamperedBody = JsonSerializer.Serialize(tamperedPayload);
            var timestamp = "1758011225399";
            var keyId = "dwk-d5418756-4908-4077-b053-3bc662820f85";

            // Sign the original body
            var signature =
                "304402202c1365b6bf4aadd370592c6dcd0623c5ea2f18ef22f2d1799816c3c23fa7914c022005c2dbff467214357461ba178ff9a405a525c70867346a029bba0173bb47479e";

            var headers = new Dictionary<string, string>
            {
                ["x-kulipa-signature"] = signature,
                ["x-kulipa-signature-ts"] = timestamp,
                ["x-kulipa-key-id"] = keyId
            };

            // Act - Try to verify with tampered body
            var result = await _client.Webhooks.VerifyWebhookAsync(headers, tamperedBody);

            // Assert
            Assert.IsFalse(result.IsValid, "Webhook verification should fail for tampered body");
            Assert.AreEqual(
                VerificationFailureReason.SignatureVerificationFailed,
                result.FailureReason,
                "Should indicate signature verification failure"
            );
        }

        [TestMethod]
        public async Task VerifyWebhook_ReplayAttack_FailsForOldTimestamp()
        {
            // Arrange
            var payload = new { eventType = "card.created" };
            var rawBody = JsonSerializer.Serialize(payload);

            // Use a timestamp from 10 minutes ago (replay attack)
            var oldTimestamp = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds().ToString();
            var keyId = "test-key-id";
            var signature = _webhookSender.GenerateSignature(rawBody, oldTimestamp);

            var headers = new Dictionary<string, string>
            {
                ["x-kulipa-signature"] = signature,
                ["x-kulipa-signature-ts"] = oldTimestamp,
                ["x-kulipa-key-id"] = keyId
            };

            // Act
            var result = await _client.Webhooks.VerifyWebhookAsync(headers, rawBody);

            // Assert
            Assert.IsFalse(result.IsValid, "Should reject old timestamps");
            Assert.AreEqual(
                VerificationFailureReason.TimestampTooOld,
                result.FailureReason,
                "Should indicate timestamp is too old"
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
        }
    }

    /// <summary>
    ///     Mock webhook sender for testing.
    ///     In production, Kulipa would use their private key to sign webhooks.
    /// </summary>
    internal class MockWebhookSender
    {
        private readonly ECDsa _ecdsa;

        public MockWebhookSender()
        {
            _ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        }

        public string GenerateSignature(string rawBody, string timestamp)
        {
            var signedPayload = $"{timestamp}.{rawBody}";
            var messageBytes = Encoding.UTF8.GetBytes(signedPayload);
            var signature = _ecdsa.SignData(messageBytes, HashAlgorithmName.SHA256);
            return Convert.ToBase64String(signature);
        }

        public string GetPublicKey()
        {
            var publicKey = _ecdsa.ExportSubjectPublicKeyInfo();
            return Convert.ToBase64String(publicKey);
        }

        public void Dispose()
        {
            _ecdsa?.Dispose();
        }
    }
}

/// <summary>
///     Example of how to use webhook verification in an ASP.NET Core application.
/// </summary>
public class WebhookUsageExample
{
    private readonly IKulipaClient _kulipaClient;
    private readonly ILogger<WebhookUsageExample> _logger;

    public WebhookUsageExample(IKulipaClient kulipaClient, ILogger<WebhookUsageExample> logger)
    {
        _kulipaClient = kulipaClient;
        _logger = logger;
    }

    public async Task<bool> ProcessWebhookAsync(
        IDictionary<string, string> headers,
        string rawBody)
    {
        // Step 1: Verify the webhook
        var verificationResult = await _kulipaClient.Webhooks.VerifyWebhookAsync(headers, rawBody);

        if (!verificationResult.IsValid)
        {
            _logger.LogWarning(
                "Webhook verification failed: {Reason} - {Error}",
                verificationResult.FailureReason,
                verificationResult.ErrorMessage
            );
            return false;
        }

        // Step 2: Parse the webhook payload
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var webhook = JsonSerializer.Deserialize<WebhookEvent>(rawBody, options);

        if (webhook == null)
        {
            _logger.LogError("Failed to deserialize webhook payload");
            return false;
        }

        // Step 3: Process based on event type
        try
        {
            switch (webhook.EventType)
            {
                case "card.created":
                    await HandleCardCreated(webhook);
                    break;

                case "card.frozen":
                    await HandleCardFrozen(webhook);
                    break;

                case "transaction.completed":
                    await HandleTransactionCompleted(webhook);
                    break;

                default:
                    _logger.LogInformation("Unhandled webhook event type: {EventType}", webhook.EventType);
                    break;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook event: {EventType}", webhook.EventType);
            throw;
        }
    }

    private async Task HandleCardCreated(WebhookEvent webhook)
    {
        _logger.LogInformation("Processing card.created event");
        // Your business logic here
        await Task.CompletedTask;
    }

    private async Task HandleCardFrozen(WebhookEvent webhook)
    {
        _logger.LogInformation("Processing card.frozen event");
        // Your business logic here
        await Task.CompletedTask;
    }

    private async Task HandleTransactionCompleted(WebhookEvent webhook)
    {
        _logger.LogInformation("Processing transaction.completed event");
        // Your business logic here
        await Task.CompletedTask;
    }

    public class WebhookEvent
    {
        public string EventType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public JsonElement Data { get; set; }
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}