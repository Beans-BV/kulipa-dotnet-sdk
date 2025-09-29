using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Users
{
    /// <summary>
    ///     Represents a shipping address.
    /// </summary>
    public class ShippingAddress
    {
        /// <summary>
        ///     Recipient address.
        /// </summary>
        [JsonPropertyName("address")]
        public Address Address { get; set; } = null!;

        /// <summary>
        ///     Phone number of the recipient. One string of digits only with optional + at the start.
        /// </summary>
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }
    }
}