using Core6.Infra.Base.Queries;
using Core6.Infra.Base.Personalizacao.Consultas.Operadores;

namespace Core6.Infra.Base.Personalizacao.Consultas
{
    public class ResolvedorOperadorPersona
    {
        public ObjetoResolvidoPersona Resolver(EnumTipoCampo tipo, string nome, OperadorFiltros? operador, object valores)
        {
            return new FabricaResolvedorOperador().Cria(operador).Resolve(tipo, nome, valores);
        }
    }
}
