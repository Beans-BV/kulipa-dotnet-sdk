using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Cards
{
    /// <summary>
    ///     Represents the delivery method for physical cards.
    /// </summary>
    public enum DeliveryType
    {
        /// <summary>
        ///     Card will be shipped directly to the user's address.
        /// </summary>
        [JsonPropertyName("ship_to_user")] ShipToUser
    }
}