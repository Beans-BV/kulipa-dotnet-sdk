using System.ComponentModel.DataAnnotations;

namespace Kulipa.Sdk.Configuration
{
    /// <summary>
    ///     Configuration options for the Kulipa SDK.
    /// </summary>
    public class KulipaSdkOptions
    {
        /// <summary>
        ///     Gets or sets the API key for authentication.
        /// </summary>
        [Required(ErrorMessage = "API Key is required")]
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the base URL for the Kulipa API.
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.kulipa.com";

        /// <summary>
        ///     Gets or sets the environment (Sandbox/Production).
        /// </summary>
        public KulipaEnvironment Environment { get; set; } = KulipaEnvironment.Sandbox;

        /// <summary>
        ///     Gets or sets the timeout for HTTP requests.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        ///     Gets or sets the retry policy options.
        /// </summary>
        public RetryPolicyOptions RetryPolicy { get; set; } = new();

        /// <summary>
        ///     Gets or sets whether to enable idempotency for POST/PUT requests.
        /// </summary>
        public bool EnableIdempotency { get; set; } = true;

        /// <summary>
        ///     Gets or sets whether to auto-generate idempotency keys.
        /// </summary>
        public bool AutoGenerateIdempotencyKey { get; set; } = false;

        /// <summary>
        ///     Gets or sets whether to enable automatic rate limit handling.
        /// </summary>
        public bool EnableRateLimitHandling { get; set; } = true;

        /// <summary>
        ///     Gets or sets the maximum requests per minute (default: 300).
        /// </summary>
        public int MaxRequestsPerMinute { get; set; } = 300;

        /// <summary>
        ///     Gets or sets whether to enable request/response logging.
        /// </summary>
        public bool EnableLogging { get; set; } = true;

        /// <summary>
        ///     Gets or sets the maximum age of webhook timestamps.
        ///     Default is 5 minutes.
        /// </summary>
        public TimeSpan WebhookTimestampTolerance { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        ///     Gets or sets the webhook public key cache expiration.
        ///     Default is 1 hour.
        /// </summary>
        public TimeSpan WebhookKeyCacheExpiration { get; set; } = TimeSpan.FromHours(1);
    }

    public class RetryPolicyOptions
    {
        /// <summary>
        ///     Maximum number of retry attempts.
        /// </summary>
        public int MaxRetryAttempts { get; set; } = 3;

        /// <summary>
        ///     Base delay between retries in seconds.
        /// </summary>
        public int BaseDelaySeconds { get; set; } = 2;

        /// <summary>
        ///     Whether to use exponential backoff.
        /// </summary>
        public bool UseExponentialBackoff { get; set; } = true;
    }

    public enum KulipaEnvironment
    {
        Sandbox,
        Production
    }
}