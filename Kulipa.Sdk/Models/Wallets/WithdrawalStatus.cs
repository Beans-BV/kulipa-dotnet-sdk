namespace Kulipa.Sdk.Models.Wallets
{
    /// <summary>
    ///     Possible statuses of a withdrawal transaction.
    /// </summary>
    public enum WithdrawalStatus
    {
        /// <summary>
        ///     Withdrawal is in draft state.
        /// </summary>
        Draft,

        /// <summary>
        ///     Withdrawal is pending confirmation.
        /// </summary>
        Pending,

        /// <summary>
        ///     Withdrawal has been confirmed and processed.
        /// </summary>
        Confirmed,

        /// <summary>
        ///     Withdrawal has been rejected.
        /// </summary>
        Rejected,

        /// <summary>
        ///     Withdrawal has failed to process.
        /// </summary>
        Failed
    }
}