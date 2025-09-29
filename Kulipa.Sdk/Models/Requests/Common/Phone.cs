using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Requests.Common
{
    /// <summary>
    ///     Represents a phone number with country code.
    /// </summary>
    public class Phone
    {
        /// <summary>
        ///     Phone country prefix (for instance, +33). It should begin with a '+'.
        /// </summary>
        [Required]
        [JsonPropertyName("country")]
        public string Country { get; set; } = null!;

        /// <summary>
        ///     Phone number, not including the country prefix, with a maximum of 12 digits.
        /// </summary>
        [Required]
        [JsonPropertyName("number")]
        public string Number { get; set; } = null!;
    }
}