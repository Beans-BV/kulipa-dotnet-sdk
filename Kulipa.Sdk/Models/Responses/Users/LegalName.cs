using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Responses.Users
{
    /// <summary>
    ///     Represents a user's legal name as per KYC provider.
    /// </summary>
    public sealed record LegalName
    {
        /// <summary>
        ///     First name.
        /// </summary>
        [JsonPropertyName("firstName")]
        public required string FirstName { get; init; }

        /// <summary>
        ///     Last name.
        /// </summary>
        [JsonPropertyName("lastName")]
        public required string LastName { get; init; }
    }
}