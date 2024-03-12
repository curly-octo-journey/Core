using System.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using JsonSoft.Json;
using Core6.Infra.Base.Personalizacao;
using Core6.Dominio.Base.Entidades;
using Core6.Infra.Base.MetodosExtensao;
using Core6.Infra.Base.Personalizacao.DTOs;
using Core6.Repositorio.Base.Contextos;
using Core6.Repositorio.Base;

namespace Core6.Repositorio.Base.Personalizacao
{
    public class ContextoPersonalizacao : IContextoPersonalizacao
    {
        #region ctor
        private readonly Contexto _contexto;
        private readonly IRepPersonalizacaoBanco _repPersonalizacaoBanco;

        public ContextoPersonalizacao(Contexto contexto, IRepPersonalizacaoBanco repPersonalizacaoBanco)
        {
            _contexto = contexto;
            _repPersonalizacaoBanco = repPersonalizacaoBanco;
            IniciarCache();
        }
        #endregion

        #region IniciarCache
        private void IniciarCache()
        {
            PersonalizacaoCache.Iniciar();
            if (!PersonalizacaoConfig.CodigoSistemaInformado()) return;

            var existeCache = PersonalizacaoCache.ExisteCachePersonalizacao();
            if (!existeCache)
            {
                var codigoSistema = PersonalizacaoConfig.CodigoSistema;
                var personalizacao = _repPersonalizacaoBanco.RecuperarPersonalizacao(codigoSistema);
                PersonalizacaoCache.Inserir(personalizacao);
            }
        }
        #endregion

        #region Inserir
        public void Inserir<TEntidade>(int id, Dictionary<string, object> obj) where TEntidade : Identificador, new()
        {
            if (!PersonalizacaoConfig.CodigoSistemaInformado()) return;
            if (obj == null || obj.Count == 0) return;

            var tipo = new TEntidade() as Identificador;
            var identificacao = tipo.GetIdentificacaoPersonalizacao();

            if (string.IsNullOrEmpty(identificacao)) return;

            var personaCad = PersonalizacaoCache.RecuperarPersonaCadIdentificacao(identificacao);
            if (personaCad == null) return;

            UtilContextoPersonalizacao.ValidarTabelaTenant(personaCad.NomeTabela);

            if (obj.ContainsKey("Id"))
            {
                obj["Id"] = id;
            }
            else
            {
                obj.Add("Id", id);
            }

            var json = JsonConvert.SerializeObject(obj);
            var sqlInsert = new JsonToSql().SqlCriarInsert(json, UtilContextoPersonalizacao.ConverteConfigMetaData(personaCad));
            _contexto.Database.ExecuteSqlRaw(sqlInsert);
        }

        public void Inserir<TEntidade>(Guid id, Dictionary<string, object> obj) where TEntidade : IdentificadorGuid, new()
        {
            if (!PersonalizacaoConfig.CodigoSistemaInformado()) return;
            if (obj == null || obj.Count == 0) return;

            var tipo = new TEntidade() as IdentificadorGuid;
            var identificacao = tipo.GetIdentificacaoPersonalizacao();

            if (string.IsNullOrEmpty(identificacao)) return;

            var personaCad = PersonalizacaoCache.RecuperarPersonaCadIdentificacao(identificacao);
            if (personaCad == null) return;

            UtilContextoPersonalizacao.ValidarTabelaTenant(personaCad.NomeTabela);

            if (obj.ContainsKey("Id"))
            {
                obj["Id"] = id.uToOracle();
            }
            else
            {
                obj.Add("Id", id.uToOracle());
            }

            var json = JsonConvert.SerializeObject(obj);
            var sqlInsert = new JsonToSql().SqlCriarInsert(json, UtilContextoPersonalizacao.ConverteConfigMetaData(personaCad));
            _contexto.Database.ExecuteSqlRaw(sqlInsert);
        }
        #endregion

        #region Alterar
        public void Alterar<TEntidade>(int id, Dictionary<string, object> obj) where TEntidade : Identificador, new()
        {
            if (!PersonalizacaoConfig.CodigoSistemaInformado()) return;
            if (obj == null || obj.Count == 0) return;

            var tipo = new TEntidade() as Identificador;
            var identificacao = tipo.GetIdentificacaoPersonalizacao();
            PersonaCadDTO personaCad = PersonalizacaoCache.RecuperarPersonaCadIdentificacao(identificacao);
            if (personaCad == null) return;
            UtilContextoPersonalizacao.ValidarTabelaTenant(personaCad.NomeTabela);

            if (obj.ContainsKey("Id"))
            {
                obj["Id"] = id;
            }
            else
            {
                obj.Add("Id", id);
            }
            var json = JsonConvert.SerializeObject(obj);

            var registroExisteObject = _contexto.Executar(string.Format("select count(*) from {0} where id = {1}", personaCad.NomeTabela, id)).First();
            var registroExiste = (int)registroExisteObject;

            if (registroExiste > 0)
            {
                var sqlUpdate = new JsonToSql().SqlCriarUpdate(json, UtilContextoPersonalizacao.ConverteConfigMetaData(personaCad));
                _contexto.Database.ExecuteSqlRaw(sqlUpdate);
            }
            else
            {
                var sqlInsert = new JsonToSql().SqlCriarInsert(json, UtilContextoPersonalizacao.ConverteConfigMetaData(personaCad));
                _contexto.Database.ExecuteSqlRaw(sqlInsert);
            }
        }

        public void Alterar<TEntidade>(Guid id, Dictionary<string, object> obj) where TEntidade : IdentificadorGuid, new()
        {
            if (!PersonalizacaoConfig.CodigoSistemaInformado()) return;
            if (obj == null || obj.Count == 0) return;

            var tipo = new TEntidade() as IdentificadorGuid;
            var identificacao = tipo.GetIdentificacaoPersonalizacao();
            PersonaCadDTO personaCad = PersonalizacaoCache.RecuperarPersonaCadIdentificacao(identificacao);
            if (personaCad == null) return;
            UtilContextoPersonalizacao.ValidarTabelaTenant(personaCad.NomeTabela);

            if (obj.ContainsKey("Id"))
            {
                obj["Id"] = id.uToOracle();
            }
            else
            {
                obj.Add("Id", id.uToOracle());
            }
            var json = JsonConvert.SerializeObject(obj);

            var registroExisteObject = _contexto.Executar(string.Format("select count(*) from {0} where id = {1}", personaCad.NomeTabela, id)).First();
            var registroExiste = (int)registroExisteObject;

            if (registroExiste > 0)
            {
                var sqlUpdate = new JsonToSql().SqlCriarUpdate(json, UtilContextoPersonalizacao.ConverteConfigMetaData(personaCad));
                _contexto.Database.ExecuteSqlRaw(sqlUpdate);
            }
            else
            {
                var sqlInsert = new JsonToSql().SqlCriarInsert(json, UtilContextoPersonalizacao.ConverteConfigMetaData(personaCad));
                _contexto.Database.ExecuteSqlRaw(sqlInsert);
            }
        }
        #endregion

        #region MontarCamposPersonalizadosLista
        public object MontarCamposPersonalizadosLista<TEntidade>(object resultado) where TEntidade : Identificador, new()
        {
            if (PersonalizacaoConfig.ProjetoDePersonalizacao)
            {
                return resultado;
            }
            if (resultado == null)
            {
                return null;
            }

            var dados = MontarCamposPersonalizadosLista<TEntidade>(new List<object> { resultado });
            return dados.FirstOrDefault();
        }

        public object MontarCamposPersonalizadosListaGuid<TEntidade>(object resultado) where TEntidade : IdentificadorGuid, new()
        {
            if (PersonalizacaoConfig.ProjetoDePersonalizacao)
            {
                return resultado;
            }
            if (resultado == null)
            {
                return null;
            }

            var dados = MontarCamposPersonalizadosListaGuid<TEntidade>(new List<object> { resultado });
            return dados.FirstOrDefault();
        }

        public IList<object> MontarCamposPersonalizadosLista<TEntidade>(List<object> dadosPaginacao) where TEntidade : Identificador, new()
        {
            if (PersonalizacaoConfig.ProjetoDePersonalizacao)
            {
                return dadosPaginacao;
            }
            if (!PersonalizacaoConfig.CodigoSistemaInformado())
            {
                return dadosPaginacao;
            }

            var tipo = new TEntidade() as Identificador;
            var identificacao = tipo.GetIdentificacaoPersonalizacao();
            if (identificacao == null) return dadosPaginacao;

            var personaCad = PersonalizacaoCache.RecuperarPersonaCadIdentificacao(identificacao);
            if (personaCad == null) return dadosPaginacao;

            var tipoDeLista = RecuperarTipoDeLista(dadosPaginacao);
            var codigos = RecuperarIdsRegistros(dadosPaginacao, tipoDeLista);

            if (codigos == null || !codigos.Any()) return dadosPaginacao;
            var sql = "";
            var codigosString = "";

            if (codigos.Count > 1000)
            {
                sql = string.Format("select * from {0} where 1 = 1 ", personaCad.NomeTabela);
                while (codigos.Count > 0)
                {
                    var selecao1000 = codigos.Take(1000);
                    codigosString = string.Join(",", selecao1000.ToArray());
                    sql = sql + " or id in (" + codigosString + ")";
                    if (codigos.Count >= 1000)
                    {
                        codigos.RemoveRange(0, 1000);
                    }
                    else
                    {
                        codigos.RemoveRange(0, codigos.Count);
                    }
                }
            }
            else
            {
                codigosString = string.Join(",", codigos.ToArray());
                sql = string.Format("select * from {0} where id in ({1})", personaCad.NomeTabela, codigosString);
            }
            var dadosPersonalizados = _contexto.Executar(sql);
            List<object> retornoObjeto = new List<object>();
            foreach (var dadoPaginado in dadosPaginacao)
            {
                Dictionary<string, object> dicionario = new Dictionary<string, object>();
                switch (tipoDeLista)
                {
                    case EnumTipoDeLista.Tipada:
                        dicionario = MontarDicionarioListaTipada(personaCad.PersonaCadCampos, dadosPersonalizados, dadoPaginado);
                        break;
                    case EnumTipoDeLista.Dinamica:
                        dicionario = MontarDicionarioListaDinamica(personaCad.PersonaCadCampos, dadosPersonalizados, dadoPaginado);
                        break;
                    default:
                        break;
                }
                retornoObjeto.Add(dicionario);
            }
            return retornoObjeto;
        }

        public IList<object> MontarCamposPersonalizadosListaGuid<TEntidade>(List<object> dadosPaginacao) where TEntidade : IdentificadorGuid, new()
        {
            if (PersonalizacaoConfig.ProjetoDePersonalizacao)
            {
                return dadosPaginacao;
            }
            if (!PersonalizacaoConfig.CodigoSistemaInformado())
            {
                return dadosPaginacao;
            }

            var tipo = new TEntidade() as IdentificadorGuid;
            var identificacao = tipo.GetIdentificacaoPersonalizacao();
            if (identificacao == null) return dadosPaginacao;

            var personaCad = PersonalizacaoCache.RecuperarPersonaCadIdentificacao(identificacao);
            if (personaCad == null) return dadosPaginacao;

            var tipoDeLista = RecuperarTipoDeLista(dadosPaginacao);
            var codigos = RecuperarIdsRegistros(dadosPaginacao, tipoDeLista);

            if (codigos == null || !codigos.Any()) return dadosPaginacao;
            var sql = "";
            var codigosString = "";

            if (codigos.Count > 1000)
            {
                sql = string.Format("select * from {0} where 1 = 1 ", personaCad.NomeTabela);
                while (codigos.Count > 0)
                {
                    var selecao1000 = codigos.Take(1000);
                    codigosString = string.Join(",", selecao1000.ToArray());
                    sql = sql + " or id in (" + codigosString + ")";
                    if (codigos.Count >= 1000)
                    {
                        codigos.RemoveRange(0, 1000);
                    }
                    else
                    {
                        codigos.RemoveRange(0, codigos.Count);
                    }
                }
            }
            else
            {
                codigosString = string.Join(",", codigos.ToArray());
                sql = string.Format("select * from {0} where id in ({1})", personaCad.NomeTabela, codigosString);
            }
            var dadosPersonalizados = _contexto.Executar(sql);
            List<object> retornoObjeto = new List<object>();
            foreach (var dadoPaginado in dadosPaginacao)
            {
                Dictionary<string, object> dicionario = new Dictionary<string, object>();
                switch (tipoDeLista)
                {
                    case EnumTipoDeLista.Tipada:
                        dicionario = MontarDicionarioListaTipada(personaCad.PersonaCadCampos, dadosPersonalizados, dadoPaginado);
                        break;
                    case EnumTipoDeLista.Dinamica:
                        dicionario = MontarDicionarioListaDinamica(personaCad.PersonaCadCampos, dadosPersonalizados, dadoPaginado);
                        break;
                    default:
                        break;
                }
                retornoObjeto.Add(dicionario);
            }
            return retornoObjeto;
        }

        private Dictionary<string, object> MontarDicionarioListaTipada(List<PersonaCadCampoDTO> personaCadCampos, IEnumerable<dynamic> dadosPersonalizados, object dadoPaginado)
        {
            var id = 0;
            var dicionario = dadoPaginado.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(dadoPaginado) ?? "");

            if (!dicionario.ContainsKey("Id"))
            {
                return dicionario;
            }

            id = int.Parse(dicionario["Id"].ToString());

            if (id != 0)
            {
                var dadoPersonalizado = dadosPersonalizados.Where(p => p.ID == id).FirstOrDefault();
                if (dadoPersonalizado != null)
                {
                    var dicionarioPersonalizado = (IDictionary<string, object>)dadoPersonalizado;
                    var dicionarioCamposPersonalizados = new Dictionary<string, object>();
                    foreach (var item in dicionarioPersonalizado)
                    {
                        if (item.Key == "ID")
                        {
                            continue;
                        }
                        var personaCadCampo = RecuperarPersonaCadCampo(personaCadCampos, item.Key);
                        if (!string.IsNullOrEmpty(personaCadCampo.NomeCampo))
                        {
                            dicionarioCamposPersonalizados.Add(personaCadCampo.NomeCampo, RecuperarValor(personaCadCampo, item.Value));
                        }
                    }
                    if (dicionarioCamposPersonalizados.Any())
                    {
                        dicionario.Add("CamposPersonalizados", dicionarioCamposPersonalizados);
                    }
                }
            }

            return dicionario;
        }

        private Dictionary<string, object> MontarDicionarioListaDinamica(List<PersonaCadCampoDTO> personaCadCampos, IEnumerable<dynamic> dadosPersonalizados, object linha)
        {
            var id = 0;
            var dicionario = linha.GetType().GetFields().ToDictionary(x => x.Name, x => x.GetValue(linha) ?? "");

            if (!dicionario.ContainsKey("Id"))
            {
                return dicionario;
            }

            id = int.Parse(dicionario["Id"].ToString());

            if (id != 0)
            {
                var dadoPersonalizado = dadosPersonalizados.Where(p => p.ID == id).FirstOrDefault();
                var dicionarioCamposPersonalizados = new Dictionary<string, object>();
                if (dadoPersonalizado != null)
                {
                    var dicionarioPersonalizado = (IDictionary<string, object>)dadoPersonalizado;
                    foreach (var item in dicionarioPersonalizado)
                    {
                        if (item.Key == "ID")
                        {
                            continue;
                        }
                        var personaCadCampo = RecuperarPersonaCadCampo(personaCadCampos, item.Key);
                        if (!string.IsNullOrEmpty(personaCadCampo.NomeCampo))
                        {
                            dicionarioCamposPersonalizados.Add(personaCadCampo.NomeCampo, RecuperarValor(personaCadCampo, item.Value));
                        }
                    }
                    if (dicionarioCamposPersonalizados.Any())
                    {
                        dicionario.Add("CamposPersonalizados", dicionarioCamposPersonalizados);
                    }
                }
            }

            return dicionario;
        }

        private PersonaCadCampoDTO RecuperarPersonaCadCampo(List<PersonaCadCampoDTO> personaCadCampo, string nomeColunaSql)
        {
            return personaCadCampo.FirstOrDefault(p => p.NomeCampo.ToUpper() == nomeColunaSql);
        }
        #endregion

        #region RecuperarValor
        private dynamic RecuperarValor(PersonaCadCampoDTO personaCadCampo, dynamic value)
        {
            if (personaCadCampo.TipoCampo == EnumTipoCampo.SimNao)
            {
                if (value is DBNull)
                {
                    return false;
                }

                if (value is int)
                {
                    return Convert.ToBoolean(value);
                }
            }
            return value;
        }
        #endregion

        #region RecuperarIdsRegistros
        private List<int> RecuperarIdsRegistros(IList<object> resultadoPaginado, EnumTipoDeLista tipoDeLista)
        {
            var listaDeCodigos = new List<int>();

            switch (tipoDeLista)
            {
                case EnumTipoDeLista.Dinamica:
                    foreach (var item in resultadoPaginado)
                    {
                        foreach (var property in item.GetType().GetFields())
                        {
                            if (property.Name == "Id")
                            {
                                listaDeCodigos.Add((int)property.GetValue(item));
                            }
                        }
                    }
                    break;
                case EnumTipoDeLista.Tipada:
                    foreach (var item in resultadoPaginado)
                    {
                        foreach (var property in item.GetType().GetProperties())
                        {
                            if (property.Name == "Id")
                            {
                                listaDeCodigos.Add((int)property.GetValue(item));
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            return listaDeCodigos;
        }
        #endregion

        #region RecuperarTipoDeLista
        public EnumTipoDeLista RecuperarTipoDeLista(IList<object> lista)
        {
            var primeiroRegistro = lista.FirstOrDefault();

            if (primeiroRegistro == null)
            {
                return EnumTipoDeLista.Tipada;
            }

            var tipo = primeiroRegistro.GetType().Name;

            if (tipo.ToLower().Contains("dynamiclinqtype"))
            {
                return EnumTipoDeLista.Dinamica;
            }
            return EnumTipoDeLista.Tipada;
        }
        #endregion
    }

    #region EnumTipoDeLista
    public enum EnumTipoDeLista
    {
        Dinamica = 1,
        Tipada = 2
    }
    #endregion
}