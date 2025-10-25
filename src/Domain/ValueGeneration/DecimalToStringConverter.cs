using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueGeneration;

public class DecimalToStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetDecimal(out var number))
            {
                return number.ToString("0.##");
            }
            if (reader.TryGetInt32(out var intNumber))
            {
                return intNumber.ToString();
            }
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            string? stringValue = reader.GetString();
            if (stringValue == null)
            {
                return "0"; // Or handle as appropriate for a null string value
            }
            if (decimal.TryParse(stringValue, out var number))
            {
                return number.ToString("0.##");
            }
            return stringValue; // Return as is if not a valid decimal string
        }
        return "0"; // Default or error handling
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (decimal.TryParse(value, out var number))
        {
            var formattedValue = number.ToString("0.##");
            writer.WriteStringValue(formattedValue);
        }
        else
        {
            writer.WriteStringValue(value);
        }
    }
}
