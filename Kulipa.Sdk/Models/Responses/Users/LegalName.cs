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
        public string FirstName { get; init; } = null!;

        /// <summary>
        ///     Last name.
        /// </summary>
        [JsonPropertyName("lastName")]
        public string LastName { get; init; } = null!;
    }
}