using System.Linq;
using System;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using Core6.Infra.Base.Auth;
using Core6.Infra.Base.Personalizacao.DTOs;

namespace Core6.Infra.Base.Personalizacao
{
    public class PersonalizacaoCache
    {
        #region ctor
        private static MemoryCache cache;
        public static void Iniciar()
        {
            if (cache == null)
            {
                cache = new MemoryCache(new MemoryCacheOptions());
            }
        }
        #endregion

        #region Chave
        private static string Chave()
        {
            var codigoTenant = DadosTokenHelperBase.Dados().RecuperarTenant();
            return "PersonalizacaoCache_" + codigoTenant;
        }
        #endregion

        #region ExisteCachePersonalizacao
        public static bool ExisteCachePersonalizacao()
        {
            var chave = Chave();
            object valorCache;
            if (!cache.TryGetValue(chave, out valorCache))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region RecuperarDadosCache
        public static List<CacheDTO> RecuperarDadosCache()
        {
            var chaves = cache.GetKeys();
            var caches = new List<CacheDTO>();
            if (chaves != null)
            {
                foreach (var chave in chaves)
                {
                    var cache = Recuperar(chave);
                    if (cache != null)
                    {
                        caches.Add(new CacheDTO
                        {
                            Dados = cache,
                            Nome = chave.ToString()
                        });
                    }
                }
                return caches;
            }
            return new List<CacheDTO>();
        }
        #endregion

        #region Inserir
        public static void Inserir(PersonaDTO personalizacao)
        {
            var chave = Chave();
            var cacheExistente = Recuperar();
            if (cacheExistente != null)
            {
                cache.Remove(chave);
            }
            cache.Set(chave, personalizacao);
        }
        #endregion

        #region RecuperarPersonalizacaoPonto
        public static PersonaPontoDTO RecuperarPersonalizacaoPonto(string ponto, EnumOperacaoPontoPersonalizacao operacao)
        {
            var persona = Recuperar();
            if (persona == null)
            {
                return null;
            }
            switch (operacao)
            {
                case EnumOperacaoPontoPersonalizacao.Antes:
                    return persona.PersonaPontos.FirstOrDefault(p => p.Ponto == ponto && p.Antes == 1);
                case EnumOperacaoPontoPersonalizacao.Depois:
                    return persona.PersonaPontos.FirstOrDefault(p => p.Ponto == ponto && p.Depois == 1);
            }
            return null;
        }
        #endregion

        #region RecuperarPersonaCadTabela
        public static PersonaCadDTO RecuperarPersonaCadTabela(string nomeTabela)
        {
            var configPersona = Recuperar();
            if (configPersona != null)
            {
                var personalizacaoCadMemoria = configPersona.PersonaCad.Where(p => p.NomeTabela.ToUpper() == nomeTabela.ToUpper()).FirstOrDefault();

                if (personalizacaoCadMemoria != null)
                {
                    return personalizacaoCadMemoria;
                }
            }
            throw new Exception(string.Format("Não foi encontrado personalização para a tabela {0}.", nomeTabela));
        }
        #endregion

        #region RecuperarPersonaCadIdentificacao
        public static PersonaCadDTO RecuperarPersonaCadIdentificacao(string identificacao)
        {
            var configPersona = Recuperar();
            if (configPersona != null)
            {
                var personalizacaoCadMemoria = configPersona.PersonaCad.Where(p => p.Identificacao != null && p.Identificacao.ToUpper() == identificacao.ToUpper()).FirstOrDefault();

                if (personalizacaoCadMemoria != null)
                {
                    return personalizacaoCadMemoria;
                }
            }
            return null;
        }
        #endregion

        #region Recuperar
        private static PersonaDTO Recuperar()
        {
            return Recuperar(Chave());
        }

        private static PersonaDTO Recuperar(object chave)
        {
            PersonaDTO dadoDoCache;
            if (cache.TryGetValue(chave, out dadoDoCache))
            {
                return dadoDoCache;
            }
            return null;
        }

        #endregion
    }

    #region MemoryCacheExtensions
    public static class MemoryCacheExtensions
    {
        private static readonly Func<MemoryCache, object> GetEntriesCollection = Delegate.CreateDelegate(
            typeof(Func<MemoryCache, object>),
            typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true),
            throwOnBindFailure: true) as Func<MemoryCache, object>;

        public static IEnumerable GetKeys(this IMemoryCache memoryCache) =>
            memoryCache != null ? ((IDictionary)GetEntriesCollection((MemoryCache)memoryCache)).Keys : null;

        public static IEnumerable<T> GetKeys<T>(this IMemoryCache memoryCache) =>
            memoryCache.GetKeys().OfType<T>();
    }
    #endregion
}