using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Responses.Users
{
    /// <summary>
    ///     Represents a phone number with country code.
    /// </summary>
    public sealed record Phone
    {
        /// <summary>
        ///     Phone country prefix (for instance, +33). It should begin with a '+'.
        /// </summary>
        [JsonPropertyName("country")]
        public required string Country { get; init; }

        /// <summary>
        ///     Phone number, not including the country prefix, with a maximum of 12 digits.
        /// </summary>
        [JsonPropertyName("number")]
        public required string Number { get; init; }
    }
}