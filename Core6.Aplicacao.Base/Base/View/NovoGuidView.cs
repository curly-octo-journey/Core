using Core6.Dominio.Base.Entidades;

namespace Core6.Aplicacao.Base.Base.View
{
    public class NovoGuidView
    {
        public NovoGuidView()
        {

        }

        public NovoGuidView(Guid id)
        {
            Id = id;
        }

        public NovoGuidView(IdentificadorGuid identificador)
            : this(identificador.Id)
        {

        }

        public Guid Id { get; set; }

        public static List<NovoGuidView> Novo(IEnumerable<IdentificadorGuid> ids)
        {
            return ids.Select(p => new NovoGuidView(p))
                      .ToList();
        }
    }
}
