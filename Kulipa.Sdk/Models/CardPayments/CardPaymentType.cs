namespace Kulipa.Sdk.Models.CardPayments
{
    /// <summary>
    ///     Types of card payment transactions.
    /// </summary>
    public enum CardPaymentType
    {
        /// <summary>
        ///     A regular payment transaction.
        /// </summary>
        Payment,

        /// <summary>
        ///     A refund transaction.
        /// </summary>
        Refund
    }
}