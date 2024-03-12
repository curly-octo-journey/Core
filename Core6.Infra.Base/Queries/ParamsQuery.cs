using Newtonsoft.Json;

namespace Core6.Infra.Base.Queries
{
    public class QueryParams : QueryField
    {
        public int? page { get; set; }
        public int? start { get; set; }
        public int? limit { get; set; }
        public string? sort { get; set; }
        public string? filter { get; set; }
        public string? includes { get; set; }

        public List<string> GetIncludes()
        {
            if (includes == null)
                return new List<string>();

            try
            {
                return JsonConvert.DeserializeObject<List<string>>(includes);
            }
            catch (Exception)
            {
                throw new Exception("Erro ao deserealizar os includes");
            }

        }

        public static QueryParams GetFiltroId(int id)
        {
            return new QueryParams
            {
                limit = 1,
                page = 1,
                start = 0,
                filter = "[{'property':'Id','operator':'equal','value':" + id + "}]"
            };
        }

        public static QueryParams GetFiltroId(Guid id)
        {
            return new QueryParams
            {
                limit = 1,
                page = 1,
                start = 0,
                filter = "[{'property':'Id','operator':'equal','value':'" + id + "'}]"
            };
        }

        public List<GrupoFiltroQuery> GetFiltros()
        {
            if (filter == null) return null;
            try
            {
                var filtros = JsonConvert.DeserializeObject<List<GrupoFiltroQuery>>(filter);
                if (filtros.Count == 0 || filtros.FirstOrDefault().filter == null)
                    throw new InvalidOperationException();

                foreach (var item in filtros)
                {
                    item.Filtros = JsonConvert.DeserializeObject<List<FiltroClass>>(item.filter);
                }

                return filtros;
            }
            catch (InvalidOperationException)
            {
                List<FiltroClass> filtro = JsonConvert.DeserializeObject<List<FiltroClass>>(filter);

                List<GrupoFiltroQuery> grupo = new List<GrupoFiltroQuery>()
                {
                    new GrupoFiltroQuery()
                    {
                        Filtros = filtro
                    }
                };

                return grupo;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Erro ao deserealizar o filtro para consulta '{0}'. Erro: {1}", filter, e.Message));
            }
        }

        public List<OrdemClass> GetOrdem()
        {
            if (sort == null) return null;
            try
            {
                return JsonConvert.DeserializeObject<List<OrdemClass>>(sort);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Erro ao deserealizar o ordenação para consulta '{0}'. Erro: {1}", sort, e.Message));
            }
        }
    }
}