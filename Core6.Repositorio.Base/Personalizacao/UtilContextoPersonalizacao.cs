using JsonSoft.Json;
using Core6.Infra.Base.Auth;
using Core6.Infra.Base.Personalizacao.DTOs;

namespace Core6.Repositorio.Base.Personalizacao
{
    public static class UtilContextoPersonalizacao
    {
        #region Conversão Metadata
        public static Metadata ConverteConfigMetaData(PersonaCadDTO metadataDTO)
        {
            var metadata = new Metadata
            {
                NomeTabela = metadataDTO.NomeTabela,
                TipoChave = ConverteTipoChaveCadastro(metadataDTO.TipoChave)
            };

            foreach (var metadataCampo in metadataDTO.PersonaCadCampos)
            {
                metadata.MetadataCampos.Add(new MetadataCampos
                {
                    Descricao = metadataCampo.NomeCampo,
                    NomeCampo = metadataCampo.NomeCampo,
                    Obrigatorio = metadataCampo.Obrigatorio == 1,
                    TipoCampo = ConverteTipoCampo(metadataCampo.TipoCampo)
                });
            }

            return metadata;
        }

        private static EnumTipoChaveTabela ConverteTipoChaveCadastro(EnumTipoChaveCadastro tipoChave)
        {
            switch (tipoChave)
            {
                case EnumTipoChaveCadastro.Numero:
                    return EnumTipoChaveTabela.Numero;
                case EnumTipoChaveCadastro.Guid:
                    return EnumTipoChaveTabela.Guid;
                default:
                    return EnumTipoChaveTabela.Numero;
            }
        }

        public static TipoCampo ConverteTipoCampo(EnumTipoCampo tipoCampoDTO)
        {
            switch (tipoCampoDTO)
            {
                case EnumTipoCampo.Texto:
                    return TipoCampo.Texto;
                case EnumTipoCampo.NumeroInteiro:
                    return TipoCampo.NumeroInteiro;
                case EnumTipoCampo.NumeroDecimal:
                    return TipoCampo.NumeroDecimal;
                case EnumTipoCampo.Data:
                    return TipoCampo.Data;
                case EnumTipoCampo.SimNao:
                    return TipoCampo.SimNao;
                case EnumTipoCampo.DataHora:
                    return TipoCampo.DataHora;
                case EnumTipoCampo.Selecao:
                    return TipoCampo.Selecao;
                case EnumTipoCampo.TextoMultiLinha:
                    return TipoCampo.TextoMultiLinha;
                case EnumTipoCampo.Guid:
                    return TipoCampo.Guid;
                default:
                    throw new Exception("Tipo de campo não suportado. TipoCampo=" + tipoCampoDTO);
            }
        }
        #endregion

        #region ValidarNomeTabelaPersonalizada
        public static void ValidarNomeTabelaPersonalizada(string nomeTabela)
        {
            var inicioDeTabelasPermitidas = new List<string> { "UPC", "UPS" };
            if (!inicioDeTabelasPermitidas.Contains(nomeTabela.ToUpper().Substring(0, 3)))
            {
                throw new Exception(string.Format("Não é possível realizar operações de Inserir/Alterar/Remover na tabela {0} em projeto de personalização.", nomeTabela));
            }
        }
        #endregion

        #region ValidarTabelaTenant
        public static void ValidarTabelaTenant(string nomeTabela)
        {
            if (nomeTabela.Length < 11)
            {
                throw new Exception(string.Format("Nome de tabela personalizada deve ter no mínimo 11 caracteres. Atual = {0}", nomeTabela));
            }
            var nomeTabelaOriginal = nomeTabela;
            nomeTabela = nomeTabela.Remove(0, 3);
            var tenantString = nomeTabela.Substring(0, 8);
            var codigoTenant = 0;
            if (int.TryParse(tenantString, out codigoTenant))
            {
                if (codigoTenant != DadosTokenHelperBase.Dados().RecuperarTenant())
                {
                    throw new Exception(string.Format("A tabela com nome {0} é inválida para o tenant {1}", nomeTabelaOriginal, DadosTokenHelperBase.Dados().RecuperarTenant()));
                }
            }
            else
            {
                throw new Exception(string.Format("Não foi possível extrair tenant da tabela {0} ", nomeTabela));
            }
        }
        #endregion
    }
}