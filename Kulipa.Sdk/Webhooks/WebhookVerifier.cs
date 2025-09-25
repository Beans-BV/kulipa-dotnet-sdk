using System.Security.Cryptography;
using System.Text;
using Kulipa.Sdk.Configuration;
using Kulipa.Sdk.Models.Webhooks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kulipa.Sdk.Webhooks
{
    /// <summary>
    ///     Implements webhook signature verification for Kulipa webhooks.
    /// </summary>
    public class WebhookVerifier : IWebhookVerifier
    {
        private const string SignatureHeader = "x-kulipa-signature";
        private const string TimestampHeader = "x-kulipa-signature-ts";
        private const string KeyIdHeader = "x-kulipa-key-id";
        private const string SupportedAlgorithm = "ECDSA_SHA_256";
        private readonly IPublicKeyCache _keyCache;
        private readonly ILogger<WebhookVerifier> _logger;
        private readonly TimeSpan _timestampTolerance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WebhookVerifier" /> class.
        /// </summary>
        /// <param name="keyCache">The public key cache.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The SDK options.</param>
        public WebhookVerifier(
            IPublicKeyCache keyCache,
            ILogger<WebhookVerifier> logger,
            IOptions<KulipaSdkOptions> options)
        {
            _keyCache = keyCache ?? throw new ArgumentNullException(nameof(keyCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _timestampTolerance = options?.Value?.WebhookTimestampTolerance ?? TimeSpan.FromMinutes(5);
        }

        /// <inheritdoc />
        public async Task<WebhookVerificationResult> VerifySignatureAsync(
            string signature,
            string timestamp,
            string keyId,
            string rawBody,
            CancellationToken cancellationToken = default)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(signature))
            {
                _logger.LogWarning("Webhook verification failed: missing signature");
                return WebhookVerificationResult.Failure(
                    "Missing signature",
                    VerificationFailureReason.MissingHeaders);
            }

            if (string.IsNullOrWhiteSpace(timestamp))
            {
                _logger.LogWarning("Webhook verification failed: missing timestamp");
                return WebhookVerificationResult.Failure(
                    "Missing timestamp",
                    VerificationFailureReason.MissingHeaders);
            }

            if (string.IsNullOrWhiteSpace(keyId))
            {
                _logger.LogWarning("Webhook verification failed: missing key ID");
                return WebhookVerificationResult.Failure(
                    "Missing key ID",
                    VerificationFailureReason.MissingHeaders);
            }

            // Validate timestamp
            if (!long.TryParse(timestamp, out var timestampValue))
            {
                _logger.LogWarning("Webhook verification failed: invalid timestamp format: {Timestamp}", timestamp);
                return WebhookVerificationResult.Failure(
                    "Invalid timestamp format",
                    VerificationFailureReason.InvalidTimestampFormat);
            }

            var webhookTime = DateTimeOffset.FromUnixTimeMilliseconds(timestampValue).UtcDateTime;
            var now = DateTime.UtcNow;

            if (webhookTime > now.AddMinutes(1)) // Allow 1-minute clock skew for future
            {
                _logger.LogWarning("Webhook verification failed: timestamp in future: {WebhookTime}", webhookTime);
                return WebhookVerificationResult.Failure(
                    "Timestamp is in the future",
                    VerificationFailureReason.TimestampInFuture);
            }

            if (webhookTime < now.Subtract(_timestampTolerance))
            {
                _logger.LogWarning("Webhook verification failed: timestamp too old: {WebhookTime}", webhookTime);
                return WebhookVerificationResult.Failure(
                    $"Timestamp is older than {_timestampTolerance.TotalMinutes} minutes",
                    VerificationFailureReason.TimestampTooOld);
            }

            // Fetch public key
            var webhookKey = await _keyCache.GetKeyAsync(keyId, cancellationToken);
            if (webhookKey == null)
            {
                _logger.LogWarning("Webhook verification failed: could not fetch key: {KeyId}", keyId);
                return WebhookVerificationResult.Failure(
                    "Failed to fetch public key",
                    VerificationFailureReason.KeyFetchFailed);
            }

            // Validate algorithm
            if (!string.Equals(webhookKey.Algorithm, SupportedAlgorithm, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Webhook verification failed: unsupported algorithm: {Algorithm}",
                    webhookKey.Algorithm);
                return WebhookVerificationResult.Failure(
                    $"Unsupported algorithm: {webhookKey.Algorithm}",
                    VerificationFailureReason.UnsupportedAlgorithm);
            }

            // Construct signed payload
            var signedPayload = $"{timestamp}.{rawBody}";

            // Verify signature
            try
            {
                var isValid = VerifyEcdsaSignature(
                    webhookKey.PublicKey,
                    signedPayload,
                    signature);

                if (isValid)
                {
                    _logger.LogDebug("Webhook signature verified successfully for key: {KeyId}", keyId);
                    return WebhookVerificationResult.Success();
                }

                _logger.LogWarning("Webhook signature verification failed for key: {KeyId}", keyId);
                return WebhookVerificationResult.Failure(
                    "Signature verification failed",
                    VerificationFailureReason.SignatureVerificationFailed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during signature verification for key: {KeyId}", keyId);
                return WebhookVerificationResult.Failure(
                    $"Signature verification error: {ex.Message}",
                    VerificationFailureReason.SignatureVerificationFailed);
            }
        }

        /// <inheritdoc />
        public async Task<WebhookVerificationResult> VerifyWebhookAsync(
            IDictionary<string, string> headers,
            string rawBody,
            CancellationToken cancellationToken = default)
        {
            if (headers == null)
            {
                return WebhookVerificationResult.Failure(
                    "Headers are null",
                    VerificationFailureReason.MissingHeaders);
            }

            // Create case-insensitive dictionary if needed
            var caseInsensitiveHeaders = headers as Dictionary<string, string> ??
                                         new Dictionary<string, string>(headers, StringComparer.OrdinalIgnoreCase);

            // Extract headers
            if (!TryGetHeader(caseInsensitiveHeaders, SignatureHeader, out var signature) ||
                !TryGetHeader(caseInsensitiveHeaders, TimestampHeader, out var timestamp) ||
                !TryGetHeader(caseInsensitiveHeaders, KeyIdHeader, out var keyId))
            {
                _logger.LogWarning("Webhook verification failed: missing required headers");
                return WebhookVerificationResult.Failure(
                    "Missing required webhook headers",
                    VerificationFailureReason.MissingHeaders);
            }

            return await VerifySignatureAsync(signature!, timestamp!, keyId!, rawBody, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<WebhookVerificationResult> VerifyWebhookAsync(
            IEnumerable<KeyValuePair<string, string>> headers,
            string rawBody,
            CancellationToken cancellationToken = default)
        {
            var headerDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var header in headers)
            {
                // Take the last value if duplicate headers exist
                headerDict[header.Key] = header.Value;
            }

            return await VerifyWebhookAsync(headerDict, rawBody, cancellationToken);
        }

        private static bool TryGetHeader(IDictionary<string, string> headers, string headerName, out string? value)
        {
            // Try exact match first
            if (headers.TryGetValue(headerName, out value))
            {
                return !string.IsNullOrWhiteSpace(value);
            }

            // Try case-insensitive match
            var key = headers.Keys.FirstOrDefault(k =>
                string.Equals(k, headerName, StringComparison.OrdinalIgnoreCase));

            if (key != null && headers.TryGetValue(key, out value))
            {
                return !string.IsNullOrWhiteSpace(value);
            }

            value = null;
            return false;
        }

        private bool VerifyEcdsaSignature(string publicKey, string message, string signature)
        {
            try
            {
                // Decode the hex signature
                var signatureBytes = Convert.FromHexString(signature);

                // Convert message to bytes
                var messageBytes = Encoding.UTF8.GetBytes(message);

                // Import the PEM public key
                using var ecdsa = ECDsa.Create();
                ecdsa.ImportFromPem(publicKey);

                // Verify the signature
                return ecdsa.VerifyData(messageBytes,
                    signatureBytes,
                    HashAlgorithmName.SHA256,
                    DSASignatureFormat.Rfc3279DerSequence);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing public key or verifying signature");
                return false;
            }
        }
    }
}