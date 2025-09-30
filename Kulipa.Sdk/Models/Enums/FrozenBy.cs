namespace Kulipa.Sdk.Models.Enums
{
    /// <summary>
    ///     Represents who initiated the card freeze action.
    /// </summary>
    public enum FrozenBy
    {
        /// <summary>
        ///     Card was frozen by the user.
        /// </summary>
        User,

        /// <summary>
        ///     Card was frozen by Kulipa (administrative action).
        /// </summary>
        Kulipa
    }
}