using Core6.Repositorio.Base.Contextos;
using Devart.Data.Oracle;
using Core6.Infra.Base.Auth;
using Core6.Infra.Base.MetodosExtensao;
using Core6.Infra.Base.Personalizacao.DTOs;

namespace Core6.Repositorio.Base.Personalizacao
{
    public class RepPersonalizacaoBanco : IRepPersonalizacaoBanco
    {
        #region ctor
        private readonly Contexto _contexto;

        public RepPersonalizacaoBanco(Contexto contexto)
        {
            _contexto = contexto;
            //_contexto.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }
        #endregion

        #region Recuperar
        public PersonaDTO RecuperarPersonalizacao(int codigoSistema)
        {
            try
            {
                var personalizacao = BuscarPersona(codigoSistema);
                personalizacao.PersonaPontos = BuscarPersonaPontos(personalizacao);
                personalizacao.PersonaCad = BuscarPersonaCad(personalizacao);

                foreach (var confcad in personalizacao.PersonaCad)
                {
                    confcad.PersonaCadCampos = BuscarPersonaCadCampos(confcad);
                }

                return personalizacao;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.uTratar());
            }
        }
        #endregion

        #region RecuperarPersonalizacaoTela
        public PersonalizacaoTelaDTO RecuperarPersonalizacaoTela(int codigoSistema)
        {
            try
            {
                var codigoTenant = DadosTokenHelperBase.Dados().RecuperarTenant();
                var parametroCodigoSistema = new OracleParameter("codigosistema", OracleDbType.Number)
                {
                    Value = codigoSistema
                };
                var parametroCodigoTenant = new OracleParameter("codigotenant", OracleDbType.Number)
                {
                    Value = codigoTenant
                };
                return _contexto.SqlQuery<PersonalizacaoTelaDTO>("select javascripttela, urlpersonalizacao as url from use_persona where idsistema = :codigosistema and idtenant = :codigotenant", parametroCodigoSistema, parametroCodigoTenant).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao recuperar personalização tela. Mais detalhes:" + ex.uTratar());
            }
        }
        #endregion

        #region BuscarPersonaCadCampos
        private List<PersonaCadCampoDTO> BuscarPersonaCadCampos(PersonaCadDTO confcad)
        {
            var parametroCodigoPersona = new OracleParameter("codigopersonacad", OracleDbType.Number)
            {
                Value = confcad.CodigoPersonaCad
            };
            var sqlUsePersonaCadCampo = @"SELECT personacampo.nomecampo,
                                                 personacampo.tipocampo,
                                                 personacampo.obrigatorio
                                            FROM USE_PERSONA_CAD_CAMPO personacampo
                                           WHERE personacampo.idpersonacad  = :codigopersonacad";

            return _contexto.SqlQuery<PersonaCadCampoDTO>(sqlUsePersonaCadCampo, parametroCodigoPersona).ToList();
        }
        #endregion

        #region BuscarPersonaCad
        private List<PersonaCadDTO> BuscarPersonaCad(PersonaDTO configPersona)
        {
            var parametroCodigoPersona = new OracleParameter("codigopersona", OracleDbType.Number)
            {
                Value = configPersona.CodigoPersona
            };
            var sqlUsePersonaCad = @"  SELECT personacad.idpersonacad as codigopersonacad,
                                              personacad.nometabela,
                                              personacad.tipochave,
                                              personacad.nome as identificacao
                                         FROM USE_PERSONA_CAD personacad
                                        WHERE personacad.idpersona = :codigopersona";

            var configPersonaCad = _contexto.SqlQuery<PersonaCadDTO>(sqlUsePersonaCad, parametroCodigoPersona).ToList();
            return configPersonaCad;
        }
        #endregion

        #region BuscarPersonaPontos
        private List<PersonaPontoDTO> BuscarPersonaPontos(PersonaDTO configPersona)
        {
            var parametroCodigoPersona = new OracleParameter("codigopersona", OracleDbType.Number)
            {
                Value = configPersona.CodigoPersona
            };
            var sqlPontos = @"select use_tarefasistema.identificacao as ponto,
                                     use_tarefasistemaper.antes,
                                     use_tarefasistemaper.depois,
                                     use_persona.urlpersonalizacao as url
                                from use_persona,use_tarefasistemaper, use_tarefasistema
                               where use_persona.idpersona = use_tarefasistemaper.idpersona
                                 and use_tarefasistema.idtarefasistema = use_tarefasistemaper.idtarefasistema
                                 and use_persona.idtenant = use_tarefasistemaper.idtenant
                                 and use_persona.idpersona = :codigopersona";
            var configPersonaPontos = _contexto.SqlQuery<PersonaPontoDTO>(sqlPontos, parametroCodigoPersona).ToList();
            return configPersonaPontos;
        }
        #endregion

        #region BuscarPersona
        private PersonaDTO BuscarPersona(int codigoSistema)
        {
            var parametroCodigoSistema = new OracleParameter("codigosistema", OracleDbType.Number)
            {
                Value = codigoSistema
            };
            var sqlUsePersona = @"select idtenant as codigotenant,
                                         idpersona as codigopersona,
                                         urlpersonalizacao as url
                                    from use_persona where idsistema = :codigosistema";

            var personalizacao = _contexto.SqlQuery<PersonaDTO>(sqlUsePersona, parametroCodigoSistema).FirstOrDefault();
            if (personalizacao == null)
            {
                personalizacao = new PersonaDTO
                {
                    CodigoTenant = DadosTokenHelperBase.Dados().RecuperarTenant()
                };
            }
            return personalizacao;
        }
        #endregion
    }
}
