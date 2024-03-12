using System.Collections.Generic;
using System.Linq;

namespace Core6.Infra.Base.Queries
{
    public static class FiltroHelper
    {
        #region RecuperarValorERemover
        public static object RecuperarValorERemover(this List<GrupoFiltroQuery> filtros, string nomeFiltro)
        {
            if (filtros == null || !filtros.Any())
            {
                return null;
            }
            FiltroClass filtro = null;
            foreach (var grupoFiltroQuery in filtros)
            {
                filtro = grupoFiltroQuery.Filtros.FirstOrDefault(x => x.property == nomeFiltro);
                if (filtro != null)
                {
                    grupoFiltroQuery.Filtros.Remove(filtro);
                    break;
                }
            }
            return filtro == null ? null : filtro.value;
        }
        #endregion

        #region RecuperarERemover
        public static FiltroClass RecuperarERemover(this List<GrupoFiltroQuery> filtros, string nomeFiltro)
        {
            if (filtros == null || !filtros.Any())
            {
                return null;
            }
            FiltroClass filtro = null;
            foreach (var grupoFiltroQuery in filtros)
            {
                filtro = grupoFiltroQuery.Filtros.FirstOrDefault(x => x.property == nomeFiltro);
                if (filtro != null)
                {
                    grupoFiltroQuery.Filtros.Remove(filtro);
                    break;
                }
            }
            return filtro == null ? null : filtro;
        }
        #endregion

        #region RecuperarValor
        public static object RecuperarValor(this List<GrupoFiltroQuery> filtros, string nomeFiltro)
        {
            if (filtros == null || !filtros.Any())
            {
                return null;
            }

            FiltroClass filtro = null;
            foreach (var grupoFiltroQuery in filtros)
            {
                filtro = grupoFiltroQuery.Filtros.FirstOrDefault(x => x.property == nomeFiltro);
                if (filtro != null)
                {
                    break;
                }
            }
            return filtro == null ? null : filtro.value;
        }
        #endregion
    }
}