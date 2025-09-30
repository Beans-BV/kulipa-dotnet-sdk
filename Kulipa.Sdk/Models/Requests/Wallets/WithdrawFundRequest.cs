using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Requests.Wallets
{
    /// <summary>
    ///     Represents a request to withdraw funds from a wallet.
    /// </summary>
    public sealed record WithdrawFundRequest
    {
        /// <summary>
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [Required]
        [JsonPropertyName("tokenId")]
        public required string TokenId { get; set; }

        /// <summary>
        ///     Amount to withdraw in token decimal precision (e.g. 1000000 for 1 USDC).
        /// </summary>
        [Required]
        [JsonPropertyName("amount")]
        public required decimal Amount { get; set; }
    }
}