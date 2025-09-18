using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Wallets
{
    /// <summary>
    ///     Request to create a new wallet.
    /// </summary>
    public class CreateWalletRequest
    {
        /// <summary>
        ///     The UUID of the cardholder user.
        /// </summary>
        [Required]
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

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
        ///     Address of the non-custodial wallet on the blockchain network. Required for debit accounts.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        ///     User's withdrawal wallet address on the blockchain network. This address is used to withdraw funds from the user's
        ///     kulipa prepaid account. Required for prepaid accounts.
        /// </summary>
        [JsonPropertyName("withdrawalAddress")]
        public string WithdrawalAddress { get; set; }
    }
}