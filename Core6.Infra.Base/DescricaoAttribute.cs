using System;

namespace Core6.Infra.Base
{
    public class DescricaoAttribute : Attribute
    {
        private string description;
        private string cor;
        public string Description { get { return description; } }
        public string Cor { get { return cor; } }

        public DescricaoAttribute(string description)
        {
            this.description = description;
        }

        public DescricaoAttribute(string description, string cor)
        {
            this.description = description;
            this.cor = cor;
        }
    }
}
