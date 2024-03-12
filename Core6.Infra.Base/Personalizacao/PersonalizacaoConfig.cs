namespace Core6.Infra.Base.Personalizacao
{
    public class PersonalizacaoConfig
    {
        public static int CodigoSistema { get; private set; }
        public static bool ProjetoDePersonalizacao { get; private set; }

        public static void Configure(int codigoSistema, bool projetoDePersonalizacao)
        {
            CodigoSistema = codigoSistema;
            ProjetoDePersonalizacao = projetoDePersonalizacao;
        }

        public static bool CodigoSistemaInformado()
        {
            return CodigoSistema > 0 ? true : false;
        }
    }
}