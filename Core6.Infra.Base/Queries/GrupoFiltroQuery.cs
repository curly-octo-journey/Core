using System.Collections.Generic;

namespace Core6.Infra.Base.Queries
{
    public class GrupoFiltroQuery
    {
        public GrupoFiltroQuery()
        {
            And = true;
        }

        public string filter { get; set; }
        public List<FiltroClass> Filtros { get; set; }
        public bool And { get; set; }
    }
}
