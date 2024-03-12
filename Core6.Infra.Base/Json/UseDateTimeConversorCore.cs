using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core6.Infra.Base.Json
{
    public class UseDateTimeConversorCore : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (!string.IsNullOrEmpty(reader.GetString()))
                {
                    return DateTime.Parse(reader.GetString());
                }
                return null;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value != null)
            {
                writer.WriteStringValue(value.Value.ToString("dd/MM/yyyy HH:mm:ss"));
            }
        }
    }
}
