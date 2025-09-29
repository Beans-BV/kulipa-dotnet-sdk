namespace Kulipa.Sdk.Models.Enums
{
    /// <summary>
    ///     Reasons why a card payment might be declined.
    /// </summary>
    public enum CardPaymentDeclineReason
    {
        /// <summary>
        ///     Insufficient funds in the account.
        /// </summary>
        InsufficientFund,

        /// <summary>
        ///     Payment failed fraud or security controls.
        /// </summary>
        FailedControl,

        /// <summary>
        ///     Card or account is not active.
        /// </summary>
        NotActive,

        /// <summary>
        ///     Eager transfer timed out.
        /// </summary>
        EagerTransferTimeout,

        /// <summary>
        ///     Eager transfer failed.
        /// </summary>
        EagerTransferFailed,

        /// <summary>
        ///     Other unspecified reason.
        /// </summary>
        Other
    }
}