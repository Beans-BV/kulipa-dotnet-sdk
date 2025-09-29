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
        public string FirstName { get; set; } = null!;

        /// <summary>
        ///     Last name.
        /// </summary>
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = null!;
    }
}