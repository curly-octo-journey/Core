namespace Core6.Aplicacao.Base.Personalizacao.Atributos
{
    public class PontoAttribute : Attribute
    {
        public string Ponto { get; set; }
        public string Controller { get; set; }
        public bool Antes { get; set; }
        public bool Depois { get; set; }
    }
}
