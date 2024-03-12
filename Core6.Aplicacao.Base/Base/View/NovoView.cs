using Core6.Dominio.Base.Entidades;

namespace Core6.Aplicacao.Base.Base.View
{
    public class NovoView
    {
        public NovoView()
        {

        }

        public NovoView(int id)
        {
            Id = id;
        }

        public NovoView(Identificador identificador)
            : this(identificador.Id)
        {

        }

        public int Id { get; set; }

        public static List<NovoView> Novo(IEnumerable<Identificador> ids)
        {
            return ids.Select(p => new NovoView(p))
                      .ToList();
        }
    }
}
