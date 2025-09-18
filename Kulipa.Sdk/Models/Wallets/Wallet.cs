using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Wallets
{
    /// <summary>
    ///     Represents a Kulipa wallet.
    /// </summary>
    public class Wallet
    {
        /// <summary>
        ///     Wallet identifier. Begins with 'wlt-' followed by a v4 UUID.
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        /// <summary>
        ///     Name for the wallet.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///     The UUID of the cardholder user.
        /// </summary>
        [JsonPropertyName("userId")]
        public required string UserId { get; set; }

        /// <summary>
        ///     Status of the wallet.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WalletStatus Status { get; set; }

        /// <summary>
        ///     TODO
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        ///     A blockchain on which a wallet is deployed.
        /// </summary>
        [JsonPropertyName("blockchain")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BlockchainNetwork Blockchain { get; set; }

        /// <summary>
        ///     Verification date.
        /// </summary>
        [JsonPropertyName("verifiedAt")]
        public DateTime VerifiedAt { get; set; }

        /// <summary>
        ///     Kulipa public key created for the wallet. TODO
        /// </summary>
        [JsonPropertyName("publicKey")]
        public string PublicKey { get; set; }

        /// <summary>
        ///     Creation date.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     Last update date.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    ///     Supported blockchain networks.
    /// </summary>
    public enum BlockchainNetwork
    {
        /// <summary>
        ///     Stellar testnet network.
        /// </summary>
        StellarTestnet,

        /// <summary>
        ///     Stellar mainnet network.
        /// </summary>
        StellarMainnet
    }

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