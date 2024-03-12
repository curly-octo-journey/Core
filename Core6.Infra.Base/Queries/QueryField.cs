using Newtonsoft.Json;

namespace Core6.Infra.Base.Queries
{
    public class QueryField
    {
        public string? fields { get; set; }

        public List<string> GetFields()
        {
            try
            {
                if (fields != null)
                    return JsonConvert.DeserializeObject<List<string>>(fields);
                return new List<string>();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
    }
}
