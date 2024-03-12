namespace Core6.Infra.Base.Auth
{
    public static class DadosConexaoBase
    {
        public static Dictionary<string, object> Dados { get; set; }

        public static void Adicionar(string chave, object valor)
        {
            if (Dados == null)
            {
                Dados = new Dictionary<string, object>();
            }

            Dados.Add(chave, valor);
        }

        public static object Recuperar(string chave)
        {
            if (Dados == null)
            {
                Dados = new Dictionary<string, object>();
            }

            object valor;
            if (Dados.TryGetValue(chave, out valor))
            {
                return valor;
            }
            return null;
        }
    }
}
