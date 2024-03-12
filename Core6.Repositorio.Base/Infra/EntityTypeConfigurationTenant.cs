using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core6.Dominio.Base.Entidades;

namespace Core6.Repositorio.Base.Infra
{
    public class EntityTypeConfigurationTenant<TClass> : IEntityTypeConfiguration<TClass> where TClass : IdentificadorTenant
    {
        public virtual void Configure(EntityTypeBuilder<TClass> builder)
        {
            builder.Property(x => x.CodigoTenant)
                .HasColumnName("IDTENANT")
                .IsRequired();
        }
    }
}