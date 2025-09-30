using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Requests.Wallets
{
    /// <summary>
    ///     Request to create a new separate wallet.
    ///     Separate wallet creation is only supported for B2B distributors.
    ///     Consumer wallets are created as a part of user creation API call.
    /// </summary>
    public sealed record CreateWalletRequest
    {
        /// <summary>
        ///     A Company identifier. Begins with 'cmp-' followed by a v4 UUID.
        /// </summary>
        [Required]
        [JsonPropertyName("companyId")]
        public required string CompanyId { get; set; }

        /// <summary>
        ///     Name for the wallet.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; }

        /// <summary>
        ///     A blockchain on which a wallet is deployed.
        /// </summary>
        [Required]
        [JsonPropertyName("blockchain")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required BlockchainNetwork Blockchain { get; init; }

        /// <summary>
        ///     User's withdrawal wallet address on the blockchain network. This address is used to withdraw funds from the user's
        ///     kulipa prepaid account. Required for prepaid accounts.
        /// </summary>
        [Required]
        [JsonPropertyName("withdrawalAddress")]
        public required string WithdrawalAddress { get; init; }
    }
}