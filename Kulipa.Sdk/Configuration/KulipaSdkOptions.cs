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
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the base URL for the Kulipa API.
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

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
}