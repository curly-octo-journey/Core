using System.Collections.Generic;

namespace Core6.Infra.Base.Personalizacao.DTOs
{
    public class PersonaCadDTO
    {
        public int CodigoPersonaCad { get; set; }
        public string NomeTabela { get; set; }
        public string Identificacao { get; set; }
        public EnumTipoChaveCadastro TipoChave { get; set; }
        public List<PersonaCadCampoDTO> PersonaCadCampos { get; set; }
    }

    public class PersonaDTO
    {
        public PersonaDTO()
        {
            PersonaCad = new List<PersonaCadDTO>();
            PersonaPontos = new List<PersonaPontoDTO>();
        }

        public int CodigoPersona { get; set; }
        public string Url { get; set; }
        public List<PersonaCadDTO> PersonaCad { get; set; }
        public List<PersonaPontoDTO> PersonaPontos { get; set; }
        public int CodigoTenant { get; set; }
    }

    public class PersonaPontoDTO
    {
        public string Ponto { get; set; }
        public int Antes { get; set; }
        public int Depois { get; set; }
        public string Url { get; set; }
    }

    public class PersonaCadCampoDTO
    {
        public string NomeCampo { get; set; }
        public int Obrigatorio { get; set; }
        public EnumTipoCampo TipoCampo { get; set; }
    }
}

public enum EnumTipoCadastroSistemaPersona
{
    Novo = 1,
    Sistema = 2
}

public enum EnumOperacaoPontoPersonalizacao
{
    Antes = 1,
    Depois = 2,
}

public enum EnumTipoCampo
{
    Texto = 1,
    NumeroInteiro = 2,
    NumeroDecimal = 3,
    Data = 4,
    SimNao = 5,
    DataHora = 6,
    Selecao = 7,
    TextoMultiLinha = 8,
    Guid = 9
}

public enum EnumTipoChaveCadastro
{
    Numero = 1,
    Guid = 2
}

public class IdDto
{
    public int Id { get; set; }
}