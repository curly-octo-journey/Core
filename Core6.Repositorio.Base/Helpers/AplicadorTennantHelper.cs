using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Core6.Dominio.Base.Entidades;
using Core6.Infra.Base.Auth;

namespace Core6.Repositorio.Base.Helpers
{
    public static class AplicadorTennantHelper
    {
        public static void Aplicar(ChangeTracker changeTracker)
        {
            var states = new[] { EntityState.Added, EntityState.Modified };
            var listaEntries = changeTracker.Entries().Where(p => states.Contains(p.State)
                                                               && (p.Entity is IdentificadorTenant || p.Entity is IdentificadorTenantGuid));

            foreach (var entry in listaEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (entry.Entity is IdentificadorTenant)
                        {
                            ((IdentificadorTenant)entry.Entity).CodigoTenant = DadosTokenHelperBase.Dados().RecuperarTenant();
                        }
                        else if (entry.Entity is IdentificadorTenantGuid)
                        {
                            ((IdentificadorTenantGuid)entry.Entity).CodigoTenant = DadosTokenHelperBase.Dados().RecuperarTenant();
                        }
                        break;
                    default:
                        entry.Property("CodigoTenant").IsModified = false;
                        break;
                }
            }
        }
    }
}
