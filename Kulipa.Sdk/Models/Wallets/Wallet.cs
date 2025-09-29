using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Common;

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
        public string Id { get; set; } = null!;

        /// <summary>
        ///     Name for the wallet.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        ///     The UUID of the cardholder user.
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = null!;

        /// <summary>
        ///     Status of the wallet.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WalletStatus Status { get; set; }

        /// <summary>
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; } = null!;

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
        ///     Kulipa public key created for the wallet.
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("publicKey")]
        public string PublicKey { get; set; } = null!;

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
}