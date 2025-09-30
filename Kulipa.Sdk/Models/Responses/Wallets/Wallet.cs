using System.Text.Json.Serialization;
using Kulipa.Sdk.JsonConverters;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Responses.Wallets
{
    /// <summary>
    ///     Represents a Kulipa wallet.
    /// </summary>
    public sealed record Wallet
    {
        /// <summary>
        ///     Wallet identifier. Begins with 'wlt-' followed by a v4 UUID.
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        /// <summary>
        ///     Name for the wallet.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        /// <summary>
        ///     The UUID of the cardholder user.
        /// </summary>
        [JsonPropertyName("userId")]
        public required string UserId { get; init; }

        /// <summary>
        ///     Status of the wallet.
        /// </summary>
        [JsonPropertyName("status")]
        public WalletStatus Status { get; init; }

        /// <summary>
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("address")]
        public required string Address { get; init; }

        /// <summary>
        ///     A blockchain on which a wallet is deployed.
        /// </summary>
        [JsonPropertyName("blockchain")]
        [JsonConverter(typeof(KebabCaseLowerJsonStringEnumConverter))]
        public BlockchainNetwork Blockchain { get; init; }

        /// <summary>
        ///     Verification date.
        /// </summary>
        [JsonPropertyName("verifiedAt")]
        public DateTime VerifiedAt { get; init; }

        /// <summary>
        ///     Kulipa public key created for the wallet.
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [JsonPropertyName("publicKey")]
        public required string PublicKey { get; init; }

        /// <summary>
        ///     Creation date.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; init; }

        /// <summary>
        ///     Last update date.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; init; }
    }
}