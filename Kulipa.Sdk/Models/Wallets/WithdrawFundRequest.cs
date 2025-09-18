using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Wallets
{
    public class WithdrawFundRequest
    {
        /// <summary>
        ///     TODO
        /// </summary>
        [Required]
        [JsonPropertyName("tokenId")]
        public required string TokenId { get; set; }

        /// <summary>
        ///     Amount to withdraw in token decimal precision. (e.g. 1000000 for 1 USDC)
        /// </summary>
        [Required]
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}