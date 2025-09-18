using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Users
{
    /// <summary>
    ///     Represents a Kulipa user.
    /// </summary>
    public class User
    {
        /// <summary>
        ///     User identifier. Begins with 'usr-' followed by a v4 UUID.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        ///     User status.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserStatus Status { get; set; }

        /// <summary>
        ///     The first name of the user.
        /// </summary>
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        ///     The last name of the user.
        /// </summary>
        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        /// <summary>
        ///     Legitimate email ID of the card user. This entry should be distinct among all users.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        ///     Phone number with country code.
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
        [JsonPropertyName("countryOfBirth")]
        public string? CountryOfBirth { get; set; }

        /// <summary>
        ///     Country of residence of the card user in ISO 3166-1 alpha-2 country code.
        /// </summary>
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
        ///     The user's legal name as per KYC provider.
        /// </summary>
        [JsonPropertyName("legalName")]
        public LegalName? LegalName { get; set; }

        /// <summary>
        ///     A Company identifier. Begins with 'cmp-' followed by a v4 UUID.
        /// </summary>
        [JsonPropertyName("companyId")]
        public string? CompanyId { get; set; }

        /// <summary>
        ///     Creation date.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     Last update date.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    ///     Possible statuses of a user.
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        ///     User is unverified and cannot be used.
        /// </summary>
        Unverified,

        /// <summary>
        ///     User is active and can be used for operations.
        /// </summary>
        Active,

        /// <summary>
        ///     User is frozen and temporarily disabled.
        /// </summary>
        Frozen,

        /// <summary>
        ///     User is disabled.
        /// </summary>
        Disabled,

        /// <summary>
        ///     User is deleted.
        /// </summary>
        Deleted
    }
}