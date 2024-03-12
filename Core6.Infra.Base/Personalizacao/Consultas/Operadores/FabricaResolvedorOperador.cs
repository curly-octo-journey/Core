using Core6.Infra.Base.Queries;
using Core6.Infra.Base.Personalizacao.Consultas.Operadores;

namespace Core6.Infra.Base.Personalizacao.Consultas.Operadores
{
    class FabricaResolvedorOperador
    {
        internal IResolvedorOperadorPersonal Cria(OperadorFiltros? operador)
        {
            switch (operador)
            {
                case OperadorFiltros.Igual: return new ResolvedorOperadorIgualPer();
                case OperadorFiltros.Maior: return new ResolvedorOperadorMaiorPer();
                case OperadorFiltros.Menor: return new ResolvedorOperadorMenorPer();
                case OperadorFiltros.Contem: return new ResolvedorOperadorContemPer();
                case OperadorFiltros.ContemTodos: return new ResolvedorOperadorContemTodosPer();
                case OperadorFiltros.In: return new ResolvedorOperadorInPer();
                case OperadorFiltros.MaiorOuIgual: return new ResolvedorOperadorMaiorOuIgualPer();
                case OperadorFiltros.MenorOuIgual: return new ResolvedorOperadorMenorOuIgualPer();
                case OperadorFiltros.IniciaCom: return new ResolvedorOperadorIniciaComPer();
                case OperadorFiltros.TerminaCom: return new ResolvedorOperadorTerminaComPer();
                case OperadorFiltros.InOuNull: return new ResolvedorOperadorInOuNullPer();
                default: throw new ArgumentOutOfRangeException("operador", operador, null);
            }
        }
    }
}
