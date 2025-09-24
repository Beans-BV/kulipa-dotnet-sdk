using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Users
{
    /// <summary>
    ///     Represents a user's legal name as per KYC provider.
    /// </summary>
    public class LegalName
    {
        /// <summary>
        ///     First name.
        /// </summary>
        [JsonPropertyName("firstName")]
        public required string FirstName { get; set; }

        /// <summary>
        ///     Last name.
        /// </summary>
        [JsonPropertyName("lastName")]
        public required string LastName { get; set; }
    }
}