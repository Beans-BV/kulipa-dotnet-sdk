namespace Kulipa.Sdk.Models.Enums
{
    /// <summary>
    ///     Possible statuses of a card payment.
    /// </summary>
    public enum CardPaymentStatus
    {
        /// <summary>
        ///     Payment has been confirmed and is being processed.
        /// </summary>
        Confirmed,

        /// <summary>
        ///     Payment has been completed successfully.
        /// </summary>
        Complete,

        /// <summary>
        ///     Payment has been rejected.
        /// </summary>
        Rejected,

        /// <summary>
        ///     Payment has failed due to an error.
        /// </summary>
        Failed
    }
}