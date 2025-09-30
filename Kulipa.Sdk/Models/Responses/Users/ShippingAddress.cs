using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Responses.Users
{
    /// <summary>
    ///     Represents a shipping address.
    /// </summary>
    public sealed record ShippingAddress
    {
        /// <summary>
        ///     Recipient address.
        /// </summary>
        [JsonPropertyName("address")]
        public Address Address { get; init; } = null!;

        /// <summary>
        ///     Phone number of the recipient. One string of digits only with optional + at the start.
        /// </summary>
        [JsonPropertyName("phone")]
        public string? Phone { get; init; }
    }
}