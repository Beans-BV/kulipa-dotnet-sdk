using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Wallets
{
    /// <summary>
    ///     Represents a request to withdraw funds from a wallet.
    /// </summary>
    public class WithdrawFundRequest
    {
        /// <summary>
        ///     TODO: Update this when Kulipa updates their docs after adding support for Stellar.
        /// </summary>
        [Required]
        [JsonPropertyName("tokenId")]
        public string TokenId { get; set; } = null!;

        /// <summary>
        ///     Amount to withdraw in token decimal precision (e.g. 1000000 for 1 USDC).
        /// </summary>
        [Required]
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}