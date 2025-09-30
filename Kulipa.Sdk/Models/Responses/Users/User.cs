using System.Text.Json.Serialization;
using Kulipa.Sdk.Models.Enums;

namespace Kulipa.Sdk.Models.Responses.Users
{
    /// <summary>
    ///     Represents a Kulipa user.
    /// </summary>
    public sealed record User
    {
        /// <summary>
        ///     User identifier. Begins with 'usr-' followed by a v4 UUID.
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        /// <summary>
        ///     User status.
        /// </summary>
        [JsonPropertyName("status")]
        public UserStatus Status { get; init; }

        /// <summary>
        ///     The first name of the user.
        /// </summary>
        [JsonPropertyName("firstName")]
        public required string FirstName { get; init; }

        /// <summary>
        ///     The last name of the user.
        /// </summary>
        [JsonPropertyName("lastName")]
        public required string LastName { get; init; }

        /// <summary>
        ///     Legitimate email ID of the card user. This entry should be distinct among all users.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; init; }

        /// <summary>
        ///     Phone number with country code.
        /// </summary>
        [JsonPropertyName("phone")]
        public Phone? Phone { get; init; }

        /// <summary>
        ///     Date of birth of the card user in YYYY-MM-DD format.
        /// </summary>
        [JsonPropertyName("dateOfBirth")]
        public DateOnly? DateOfBirth { get; init; }

        /// <summary>
        ///     Place of birth of the card user in ISO 3166-1 alpha-2 country code.
        /// </summary>
        [JsonPropertyName("countryOfBirth")]
        public string? CountryOfBirth { get; init; }

        /// <summary>
        ///     Country of residence of the card user in ISO 3166-1 alpha-2 country code.
        /// </summary>
        [JsonPropertyName("countryOfResidence")]
        public string? CountryOfResidence { get; init; }

        /// <summary>
        ///     Physical address of the user.
        /// </summary>
        [JsonPropertyName("address")]
        public Address? Address { get; init; }

        /// <summary>
        ///     Shipping address of the user.
        /// </summary>
        [JsonPropertyName("shippingAddress")]
        public ShippingAddress? ShippingAddress { get; init; }

        /// <summary>
        ///     The user's legal name as per KYC provider.
        /// </summary>
        [JsonPropertyName("legalName")]
        public LegalName? LegalName { get; init; }

        /// <summary>
        ///     A Company identifier. Begins with 'cmp-' followed by a v4 UUID.
        /// </summary>
        [JsonPropertyName("companyId")]
        public string? CompanyId { get; init; }

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