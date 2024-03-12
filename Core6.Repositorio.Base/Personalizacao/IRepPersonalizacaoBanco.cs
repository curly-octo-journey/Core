using Core6.Infra.Base.Personalizacao.DTOs;

namespace Core6.Repositorio.Base.Personalizacao
{
    public interface IRepPersonalizacaoBanco
    {
        PersonaDTO RecuperarPersonalizacao(int codigoSistema);
        PersonalizacaoTelaDTO RecuperarPersonalizacaoTela(int codigoSistema);
    }
}