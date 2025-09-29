using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Requests.Users
{
    /// <summary>
    ///     Represents a user's wallet information during card creation.
    /// </summary>
    public class UserWallet
    {
        /// <summary>
        ///     A blockchain on which a wallet is deployed.
        /// </summary>
        [Required]
        [JsonPropertyName("blockchain")]
        public BlockchainNetwork Blockchain { get; set; } = BlockchainNetwork.StellarTestnet;

        /// <summary>
        ///     User's withdrawal wallet address on the blockchain network.
        ///     This address is used to withdraw funds from the user's
        ///     Kulipa prepaid account.
        /// </summary>
        [Required]
        [JsonPropertyName("withdrawalAddress")]
        public string WithdrawalAddress { get; set; } = null!;
    }
}