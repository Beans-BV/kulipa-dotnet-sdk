using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kulipa.Sdk.JsonConverters
{
    /// <summary>
    ///     A JSON converter for enumerations that serializes enum values as snake_case lowercase strings.
    /// </summary>
    public class SnakeCaseLowerJsonStringEnumConverter() : JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower);
}