using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Common;

namespace Kulipa.Sdk.Models.Wallets
{
    /// <summary>
    ///     Request to create a new separate wallet.
    ///     Separate wallet creation is only supported for B2B distributors.
    ///     Consumer wallets are created as a part of user creation API call.
    /// </summary>
    public class CreateWalletRequest
    {
        /// <summary>
        ///     A Company identifier. Begins with 'cmp-' followed by a v4 UUID.
        /// </summary>
        [Required]
        [JsonPropertyName("companyId")]
        public string CompanyId { get; set; } = null!;

        /// <summary>
        ///     Name for the wallet.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        ///     A blockchain on which a wallet is deployed.
        /// </summary>
        [Required]
        [JsonPropertyName("blockchain")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BlockchainNetwork Blockchain { get; set; } = BlockchainNetwork.StellarTestnet;

        /// <summary>
        ///     User's withdrawal wallet address on the blockchain network. This address is used to withdraw funds from the user's
        ///     kulipa prepaid account. Required for prepaid accounts.
        /// </summary>
        [Required]
        [JsonPropertyName("withdrawalAddress")]
        public string WithdrawalAddress { get; set; } = null!;
    }
}