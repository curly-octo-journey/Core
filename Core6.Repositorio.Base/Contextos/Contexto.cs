using Core6.Repositorio.Base.Helpers;
using Core6.Repositorio.Base.Infra;
using Microsoft.EntityFrameworkCore;

namespace Core6.Repositorio.Base.Contextos
{
    public class Contexto : DbContext
    {
        #region ctor
        public Contexto(DbContextOptions<Contexto> options)
            : base(options)
        {
        }
        #endregion

        #region OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        #endregion

        #region SaveChanges
        public override int SaveChanges()
        {
            AplicarTenant();
            //AplicadorDataHelper.Aplicar(ChangeTracker);
            AplicarVersionador();
            return base.SaveChanges();
        }
        #endregion

        #region AplicarVersionador
        public virtual void AplicarVersionador()
        {
            Versionador.Aplicar(ChangeTracker);
        }
        #endregion

        #region AplicarTenant
        public virtual void AplicarTenant()
        {
            AplicadorTennantHelper.Aplicar(ChangeTracker);
        }
        #endregion

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelCreating(modelBuilder);
            MultiBancoRepositorioBase.OnModelCreating(this);
            base.OnModelCreating(modelBuilder);
            DbContextHelper.ConfigurarCasasDecimais(modelBuilder);
        }

        public virtual void ModelCreating(ModelBuilder modelBuilder)
        {

        }
        #endregion
    }
}
