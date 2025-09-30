using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Requests.Common
{
    /// <summary>
    ///     Represents a phone number with country code.
    /// </summary>
    public sealed record Phone
    {
        /// <summary>
        ///     Phone country prefix (for instance, +33). It should begin with a '+'.
        /// </summary>
        [Required]
        [JsonPropertyName("country")]
        public required string Country { get; init; }

        /// <summary>
        ///     Phone number, not including the country prefix, with a maximum of 12 digits.
        /// </summary>
        [Required]
        [JsonPropertyName("number")]
        public required string Number { get; init; }
    }
}