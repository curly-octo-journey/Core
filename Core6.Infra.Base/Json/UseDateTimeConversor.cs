using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core6.Infra.Base.Json
{
    public class UseDateTimeConversor : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.Value != null)
                {
                    return DateTime.Parse(reader.Value.ToString());
                }
                return null;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString("dd/MM/yyyy HH:mm:ss"));
        }
    }
}
