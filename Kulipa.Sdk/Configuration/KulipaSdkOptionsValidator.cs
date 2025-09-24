using Microsoft.Extensions.Options;

namespace Kulipa.Sdk.Configuration
{
    /// <summary>
    ///     Validates KulipaSdkOptions configuration.
    /// </summary>
    public class KulipaSdkOptionsValidator : IValidateOptions<KulipaSdkOptions>
    {
        /// <summary>
        ///     Validates the provided options.
        /// </summary>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="options">The options to validate.</param>
        /// <returns>The validation result.</returns>
        public ValidateOptionsResult Validate(string? name, KulipaSdkOptions options)
        {
            if (options == null)
            {
                return ValidateOptionsResult.Fail("Options cannot be null");
            }

            if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return ValidateOptionsResult.Fail($"BaseUrl must be a valid HTTP or HTTPS URL: {options.BaseUrl}");
            }

            // Validate Environment
            if (!Enum.IsDefined(typeof(KulipaEnvironment), options.Environment))
            {
                return ValidateOptionsResult.Fail($"Invalid Environment value: {options.Environment}");
            }

            // Validate Timeout
            if (options.Timeout <= TimeSpan.Zero)
            {
                return ValidateOptionsResult.Fail("Timeout must be greater than zero");
            }

            if (options.Timeout > TimeSpan.FromMinutes(10))
            {
                return ValidateOptionsResult.Fail("Timeout cannot exceed 10 minutes");
            }

            // Validate Retry Policy
            if (options.RetryPolicy != null)
            {
                if (options.RetryPolicy.MaxRetryAttempts is < 0 or > 10)
                {
                    return ValidateOptionsResult.Fail("MaxRetryAttempts must be between 0 and 10");
                }

                if (options.RetryPolicy.BaseDelaySeconds is <= 0 or > 60)
                {
                    return ValidateOptionsResult.Fail("BaseDelaySeconds must be between 1 and 60");
                }
            }

            // Validate Rate Limit
            if (options is { EnableRateLimitHandling: true, MaxRequestsPerMinute: <= 0 })
            {
                return ValidateOptionsResult.Fail("RequestsPerMinute must be greater than zero");
            }

            // Validate Webhook options
            if (options.WebhookTimestampTolerance <= TimeSpan.Zero)
            {
                return ValidateOptionsResult.Fail("WebhookTimestampTolerance must be greater than zero");
            }

            if (options.WebhookTimestampTolerance > TimeSpan.FromHours(1))
            {
                return ValidateOptionsResult.Fail("WebhookTimestampTolerance cannot exceed 1 hour");
            }

            if (options.WebhookKeyCacheExpiration <= TimeSpan.Zero)
            {
                return ValidateOptionsResult.Fail("WebhookKeyCacheExpiration must be greater than zero");
            }

            if (options.WebhookKeyCacheExpiration > TimeSpan.FromDays(1))
            {
                return ValidateOptionsResult.Fail("WebhookKeyCacheExpiration cannot exceed 1 day");
            }

            return ValidateOptionsResult.Success;
        }
    }
}