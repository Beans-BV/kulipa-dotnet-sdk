namespace Kulipa.Sdk.Models.Enums
{
    /// <summary>
    ///     Represents the reason for reissuing a card.
    /// </summary>
    public enum ReissueReason
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
        ///     Card has expired and needs replacement.
        /// </summary>
        Expired
    }
}