namespace Kulipa.Sdk.Models.Wallets
{
    /// <summary>
    ///     Possible statuses of a wallet.
    /// </summary>
    public enum WalletStatus
    {
        /// <summary>
        ///     Wallet is unverified and cannot be used.
        /// </summary>
        Unverified,

        /// <summary>
        ///     Wallet is active and can be used for transactions.
        /// </summary>
        Active,

        /// <summary>
        ///     Wallet is frozen and temporarily disabled.
        /// </summary>
        Frozen
    }
}