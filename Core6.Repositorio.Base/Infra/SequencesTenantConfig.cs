namespace Core6.Repositorio.Base.Infra
{
    public static class SequencesTenantConfig
    {
        public static Dictionary<string, string> Sequences { get; private set; }

        public static void Adicionar(string chave, string valor)
        {
            if (Sequences == null)
            {
                Sequences = new Dictionary<string, string>();
            }

            Sequences.Add(chave, valor);
        }

        public static object Recuperar(string chave)
        {
            if (Sequences == null)
            {
                Sequences = new Dictionary<string, string>();
            }

            string valor;
            if (Sequences.TryGetValue(chave, out valor))
            {
                return valor;
            }
            return string.Empty;
        }
    }
}
