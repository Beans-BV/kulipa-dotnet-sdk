using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Wallets;

namespace Kulipa.Sdk.Models.Users
{
    /// <summary>
    ///     Request to create a new user with a wallet.
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        ///     The first name of the user - Minimum of 2 up to 64 characters in length as a string. In case the user have several
        ///     first names, they can be concatenated with a whitespace.
        /// </summary>
        [Required]
        [StringLength(64, MinimumLength = 2)]
        [JsonPropertyName("firstName")]
        public required string FirstName { get; set; }

        /// <summary>
        ///     The last name of the user - Minimum of 2 up to 64 characters in length as a string. In case the user have several
        ///     last names, they can be concatenated with a whitespace.
        /// </summary>
        [Required]
        [StringLength(64, MinimumLength = 2)]
        [JsonPropertyName("lastName")]
        public required string LastName { get; set; }

        /// <summary>
        ///     Non-custodial primary wallet information of the user.
        /// </summary>
        [Required]
        [JsonPropertyName("wallet")]
        public required Wallet Wallet { get; set; }

        /// <summary>
        ///     Legitimate email ID of the card user. This entry should be distinct among all users.
        /// </summary>
        [EmailAddress]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        ///     Phone number.
        /// </summary>
        [JsonPropertyName("phone")]
        public Phone? Phone { get; set; }

        /// <summary>
        ///     Date of birth of the card user in YYYY-MM-DD format.
        /// </summary>
        [JsonPropertyName("dateOfBirth")]
        public DateOnly? DateOfBirth { get; set; }

        /// <summary>
        ///     Place of birth of the card user in ISO 3166-1 alpha-2 country code.
        /// </summary>
        [StringLength(2, MinimumLength = 2)]
        [JsonPropertyName("countryOfBirth")]
        public string? CountryOfBirth { get; set; }

        /// <summary>
        ///     Country of residence of the card user in ISO 3166-1 alpha-2 country code.
        /// </summary>
        [StringLength(2, MinimumLength = 2)]
        [JsonPropertyName("countryOfResidence")]
        public string? CountryOfResidence { get; set; }

        /// <summary>
        ///     Physical address of the user.
        /// </summary>
        [JsonPropertyName("address")]
        public Address? Address { get; set; }

        /// <summary>
        ///     Shipping address of the user.
        /// </summary>
        [JsonPropertyName("shippingAddress")]
        public ShippingAddress? ShippingAddress { get; set; }

        /// <summary>
        ///     The company id of the user (for b2b cards)
        /// </summary>
        [JsonPropertyName("companyId")]
        public string? CompanyId { get; set; }
    }

    public class Wallet
    {
        /// <summary>
        ///     A blockchain on which a wallet is deployed.
        /// </summary>
        [Required]
        [JsonPropertyName("blockchain")]
        public required BlockchainNetwork Blockchain { get; set; } = BlockchainNetwork.StellarTestnet;

        /// <summary>
        ///     User's withdrawal wallet address on the blockchain network.
        ///     This address is used to withdraw funds from the user's
        ///     Kulipa prepaid account.
        /// </summary>
        [Required]
        [JsonPropertyName("withdrawalAddress")]
        public required string WithdrawalAddress { get; set; }
    }
}