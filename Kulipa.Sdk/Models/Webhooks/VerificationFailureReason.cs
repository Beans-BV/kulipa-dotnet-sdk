namespace Kulipa.Sdk.Models.Webhooks
{
    /// <summary>
    ///     Reasons for webhook verification failure.
    /// </summary>
    public enum VerificationFailureReason
    {
        /// <summary>
        ///     Required headers are missing.
        /// </summary>
        MissingHeaders,

        /// <summary>
        ///     The signature format is invalid.
        /// </summary>
        InvalidSignatureFormat,

        /// <summary>
        ///     The timestamp format is invalid.
        /// </summary>
        InvalidTimestampFormat,

        /// <summary>
        ///     The timestamp is too old (replay attack prevention).
        /// </summary>
        TimestampTooOld,

        /// <summary>
        ///     The timestamp is in the future.
        /// </summary>
        TimestampInFuture,

        /// <summary>
        ///     Failed to fetch the public key.
        /// </summary>
        KeyFetchFailed,

        /// <summary>
        ///     The public key format is invalid.
        /// </summary>
        InvalidPublicKeyFormat,

        /// <summary>
        ///     The signature verification failed.
        /// </summary>
        SignatureVerificationFailed,

        /// <summary>
        ///     The algorithm is not supported.
        /// </summary>
        UnsupportedAlgorithm
    }
}