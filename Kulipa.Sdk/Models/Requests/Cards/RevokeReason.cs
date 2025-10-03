namespace Kulipa.Sdk.Models.Requests.Cards
{
    /// <summary>
    ///     Represents the reason for revoking a card.
    /// </summary>
    public enum RevokeReason
    {
        /// <summary>
        ///     Card was lost.
        /// </summary>
        Lost,

        /// <summary>
        ///     Card was stolen from the user.
        /// </summary>
        Stolen,

        /// <summary>
        ///     Card was cancelled by the user.
        /// </summary>
        Cancelled
    }
}