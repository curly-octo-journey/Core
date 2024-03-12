using Core6.Repositorio.Base.Contextos;
using Core6.Repositorio.Base.Personalizacao;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Interfaces;
using Core6.Infra.Base.MetodosExtensao;

namespace Core6.Repositorio.Base.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Contexto _contexto;
        private readonly IContextoPersonalizacao _contextoPersonalizacao;

        public UnitOfWork(Contexto contexto, IContextoPersonalizacao contextoPersonalizacao)
        {
            _contexto = contexto;
            _contextoPersonalizacao = contextoPersonalizacao;
        }

        public void Dispose()
        {
            _contexto.Dispose();
        }

        public IEnumerable<dynamic> Executar(string sql)
        {
            return _contexto.Executar(sql);
        }

        public IList<object> MontarCamposPersonalizadosLista<TEntidade>(List<object> resultadoPaginado) where TEntidade : Identificador, new()
        {
            return _contextoPersonalizacao.MontarCamposPersonalizadosLista<TEntidade>(resultadoPaginado);
        }

        public IList<object> MontarCamposPersonalizadosListaGuid<TEntidade>(List<object> resultadoPaginado) where TEntidade : IdentificadorGuid, new()
        {
            return _contextoPersonalizacao.MontarCamposPersonalizadosListaGuid<TEntidade>(resultadoPaginado);
        }

        public object MontarCamposPersonalizadosLista<TEntidade>(object resultado) where TEntidade : Identificador, new()
        {
            return _contextoPersonalizacao.MontarCamposPersonalizadosLista<TEntidade>(resultado);
        }

        public object MontarCamposPersonalizadosListaGuid<TEntidade>(object resultado) where TEntidade : IdentificadorGuid, new()
        {
            return _contextoPersonalizacao.MontarCamposPersonalizadosListaGuid<TEntidade>(resultado);
        }

        public void PersonalizacaoInserir<TEntidade>(Guid id, Dictionary<string, object> obj) where TEntidade : IdentificadorGuid, new()
        {
            try
            {
                _contextoPersonalizacao.Inserir<TEntidade>(id, obj);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao inserir dados personalizados. " + ex.uTratar());
            }
        }

        public void PersonalizacaoAlterar<TEntidade>(Guid id, Dictionary<string, object> obj) where TEntidade : IdentificadorGuid, new()
        {
            try
            {
                _contextoPersonalizacao.Alterar<TEntidade>(id, obj);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao alterar dados personalizados. " + ex.uTratar());
            }
        }

        public void PersonalizacaoInserir<TEntidade>(int id, Dictionary<string, object> obj) where TEntidade : Identificador, new()
        {
            try
            {
                _contextoPersonalizacao.Inserir<TEntidade>(id, obj);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao inserir dados personalizados. " + ex.uTratar());
            }
        }

        public void PersonalizacaoAlterar<TEntidade>(int id, Dictionary<string, object> obj) where TEntidade : Identificador, new()
        {
            try
            {
                _contextoPersonalizacao.Alterar<TEntidade>(id, obj);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao alterar dados personalizados. " + ex.uTratar());
            }
        }

        public void Persistir()
        {
            try
            {
                var entities = from e in _contexto.ChangeTracker.Entries()
                               where e.State == EntityState.Added
                                   || e.State == EntityState.Modified
                               select e.Entity;
                foreach (var entity in entities)
                {
                    var validationContext = new ValidationContext(entity);
                    Validator.ValidateObject(entity, validationContext);
                }

                _contexto.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                RejectChanges();
                throw new Exception(e.uTratar());
            }
            catch (ValidationException e)
            {
                RejectChanges();
                throw new Exception(e.uTratar());
            }
        }

        public void RejectChanges()
        {
            var changedEntries = _contexto.ChangeTracker.Entries()
                .Where(x => x.State != EntityState.Unchanged).ToList();

            foreach (var entry in changedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
        }
    }
}
