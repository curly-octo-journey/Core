namespace Core6.Repositorio.Base.Helpers
{
    public static class AplicadorDataHelper
    {
        //public static void Aplicar(ChangeTracker changeTracker)
        //{
        //    var states = new[] { EntityState.Added, EntityState.Modified };
        //    var listaEntries = changeTracker.Entries().Where(p => states.Contains(p.State)
        //                                                       && p.Entity is Identificador);

        //    foreach (var entry in listaEntries)
        //    {
        //        switch (entry.State)
        //        {
        //            case EntityState.Added:
        //                ((Identificador)entry.Entity).DataCriacao = DateTime.Now;
        //                ((Identificador)entry.Entity).DataAlteracao = new DateTime(1900, 01, 01);
        //                break;
        //            default:
        //                ((Identificador)entry.Entity).DataAlteracao = DateTime.Now;
        //                entry.Property("DataCriacao").IsModified = false;
        //                break;
        //        }
        //    }
        //}
    }
}
