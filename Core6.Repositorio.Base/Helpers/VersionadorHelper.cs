using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Core6.Dominio.Base.Interfaces;

namespace Core6.Repositorio.Base.Helpers
{
    public static class Versionador
    {
        public static void Aplicar(ChangeTracker changeTracker)
        {
            var listaEntries = changeTracker.Entries().Where(p => p.State == EntityState.Modified &&
                                                                  p.Entity is IVersionamento);

            foreach (var entry in listaEntries)
            {
                var versao = (IVersionamento)entry.Entity;
                versao.RowVersion++;
            }
        }
    }
}
