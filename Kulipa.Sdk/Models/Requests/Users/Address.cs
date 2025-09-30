using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.Models.Requests.Users
{
    /// <summary>
    ///     Represents a physical address.
    /// </summary>
    public sealed record Address
    {
        /// <summary>
        ///     Number and street of the address (e.g., street or company name).
        ///     Maximum length: 150 characters.
        /// </summary>
        [Required]
        [StringLength(150)]
        [JsonPropertyName("address1")]
        public required string Address1 { get; init; }

        /// <summary>
        ///     Address complementary information (e.g., apartment, place, or building).
        ///     Maximum length: 50 characters.
        /// </summary>
        [StringLength(50)]
        [JsonPropertyName("address2")]
        public string? Address2 { get; init; }

        /// <summary>
        ///     Postal code or ZIP of the address.
        ///     Maximum length: 16 characters.
        /// </summary>
        [Required]
        [StringLength(16)]
        [JsonPropertyName("postalCode")]
        public required string PostalCode { get; init; }

        /// <summary>
        ///     City of the address.
        ///     Maximum length: 50 characters.
        /// </summary>
        [Required]
        [StringLength(50)]
        [JsonPropertyName("city")]
        public required string City { get; init; }

        /// <summary>
        ///     Country code of the address in ISO 3166-1 alpha-2.
        ///     Maximum length: 2 characters.
        /// </summary>
        [Required]
        [StringLength(2)]
        [JsonPropertyName("country")]
        public required string Country { get; init; }

        /// <summary>
        ///     State, county, province, or region.
        ///     Maximum length: 2 characters.
        /// </summary>
        [StringLength(2, MinimumLength = 2)]
        [JsonPropertyName("state")]
        public string? State { get; init; }
    }
}