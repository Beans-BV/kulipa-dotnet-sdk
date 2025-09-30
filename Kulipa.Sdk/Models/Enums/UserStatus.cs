namespace Kulipa.Sdk.Models.Enums
{
    /// <summary>
    ///     Possible statuses of a user.
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        ///     User is unverified and cannot be used.
        /// </summary>
        Unverified,

        /// <summary>
        ///     User is active and can be used for operations.
        /// </summary>
        Active,

        /// <summary>
        ///     User is frozen and temporarily disabled.
        /// </summary>
        Frozen,

        /// <summary>
        ///     User is disabled.
        /// </summary>
        Disabled,

        /// <summary>
        ///     User is deleted.
        /// </summary>
        Deleted
    }
}