namespace Kulipa.Sdk.Models.Enums
{
    /// <summary>
    ///     Represents the current status of a card.
    /// </summary>
    public enum CardStatus
    {
        /// <summary>
        ///     Card is inactive.
        /// </summary>
        Inactive,

        /// <summary>
        ///     Card is active and can be used for transactions.
        /// </summary>
        Active,

        /// <summary>
        ///     Card has been cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        ///     Card is temporarily frozen and cannot be used.
        /// </summary>
        Frozen,

        /// <summary>
        ///     Card has been reported as lost.
        /// </summary>
        Lost,

        /// <summary>
        ///     Card has been reported as stolen.
        /// </summary>
        Stolen,

        /// <summary>
        ///     Card has expired and is no longer valid.
        /// </summary>
        Expired
    }
}