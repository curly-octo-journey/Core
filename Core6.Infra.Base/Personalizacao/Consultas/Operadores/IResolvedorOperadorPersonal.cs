using Core6.Infra.Base.Personalizacao.Consultas;

namespace Core6.Infra.Base.Personalizacao.Consultas.Operadores
{
    interface IResolvedorOperadorPersonal
    {
        ObjetoResolvidoPersona Resolve(EnumTipoCampo tipo, string nome, object valores);
    }
}
