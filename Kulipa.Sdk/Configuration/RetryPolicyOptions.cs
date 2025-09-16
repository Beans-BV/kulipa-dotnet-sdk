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