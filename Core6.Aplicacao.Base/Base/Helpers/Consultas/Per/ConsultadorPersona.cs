using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Interfaces;
using Core6.Infra.Base.MetodosExtensao;
using Core6.Infra.Base.Personalizacao;
using Core6.Infra.Base.Personalizacao.Consultas;
using Core6.Infra.Base.Queries;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Core6.Aplicacao.Base.Base.Helpers.Consultas.Per
{
    public class ConsultadorPersona
    {
        private readonly IUnitOfWork _unitOfWork;
        public ConsultadorPersona(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private string GetIdentificacaoPersonalizacao(Type tipo)
        {
            var origemAttr = tipo.GetCustomAttributes(typeof(DescriptionAttribute), true);

            return origemAttr.uVazio()
                ? null
                : ((DescriptionAttribute)origemAttr.First()).Description;
        }

        public Expression<Func<T, bool>> Aplicar<T>(List<GrupoFiltroQuery> filtros) where T : class
        {
            if (!typeof(Identificador).IsAssignableFrom(typeof(T)))
                return null;

            if (!PersonalizacaoConfig.CodigoSistemaInformado())
                return null;

            var identificacao = GetIdentificacaoPersonalizacao(typeof(T));

            if (string.IsNullOrEmpty(identificacao))
                return null;

            var personaCad = PersonalizacaoCache.RecuperarPersonaCadIdentificacao(identificacao);
            if (personaCad == null)
                return null;

            if (personaCad.PersonaCadCampos.uVazio())
                return null;

            var camposPersonalizados = personaCad.PersonaCadCampos;
            if (camposPersonalizados.uVazio())
                return null;

            var resolvedor = new ResolvedorOperadorPersona();

            var objetosResolvidos = new List<ObjetoResolvidoPersona>();

            camposPersonalizados.ForEach(p =>
            {
                var filtroPersonalizado = filtros.RecuperarERemover(p.NomeCampo);
                if (filtroPersonalizado == null)
                    return;
                objetosResolvidos.Add(resolvedor.Resolver(p.TipoCampo, p.NomeCampo, filtroPersonalizado.GetOperador(), filtroPersonalizado.value));
            });

            if (objetosResolvidos.uVazio())
                return null;

            var sql = string.Format("SELECT ID FROM {0} WHERE {1}", personaCad.NomeTabela, MontaObjetosResolvidos(objetosResolvidos));
            var dados = _unitOfWork.Executar(sql);

            var codigosRegistrosPersonalizados = new List<int>();

            if (dados.uVazio())
                return p => false;

            var ids = dados.Select(p => p.ID).ToList();
            codigosRegistrosPersonalizados.AddRange(ids.OfType<int>().ToList());

            return ConverterExpression<T>(codigosRegistrosPersonalizados);
        }

        private Expression<Func<T, bool>> ConverterExpression<T>(List<int> ids)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var constante = Expression.Constant(ids);
            var metodo = typeof(Enumerable).GetMethods().First(p => p.Name == "Contains").MakeGenericMethod(typeof(int));
            var prop = Expression.Property(param, "Id");
            var call = Expression.Call(metodo, constante, prop);
            return Expression.Lambda<Func<T, bool>>(call, param);
        }

        private string MontaObjetosResolvidos(List<ObjetoResolvidoPersona> objetosResolvidos)
        {
            var insts = objetosResolvidos.Select(p => p.Instrucao).ToList();
            return string.Join(" AND ", insts);
        }
    }
}
