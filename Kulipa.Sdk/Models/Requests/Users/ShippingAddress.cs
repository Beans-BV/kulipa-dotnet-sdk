using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Requests.Users
{
    /// <summary>
    ///     Represents a shipping address.
    /// </summary>
    public sealed record ShippingAddress
    {
        /// <summary>
        ///     Recipient address.
        /// </summary>
        [Required]
        [JsonPropertyName("address")]
        public required Address Address { get; init; }

        /// <summary>
        ///     Phone number of the recipient. One string of digits only with optional + at the start.
        /// </summary>
        [JsonPropertyName("phone")]
        public string? Phone { get; init; }
    }
}