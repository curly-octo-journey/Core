using Core6.Infra.Base.MetodosExtensao;
using Core6.Infra.Base.Personalizacao.Consultas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core6.Infra.Base.Personalizacao.Consultas.Operadores
{
    public class ResolvedorOperadorInOuNullPer : IResolvedorOperadorPersonal
    {
        public ObjetoResolvidoPersona Resolve(EnumTipoCampo tipo, string nome, object valores)
        {
            var objects = valores as object[];
            objects.uExceptionSeNull("Os valores do filtro não são do tipo lista.");

            var val = new List<string>();

            if (tipo == EnumTipoCampo.Texto || tipo == EnumTipoCampo.TextoMultiLinha)
            {
                val = objects.Select(p => "'" + p.ToString().ToUpper() + "'").ToList();
                nome = "upper(" + nome + ")";
            }
            else if (tipo == EnumTipoCampo.Selecao || tipo == EnumTipoCampo.NumeroInteiro)
            {
                val = objects.Select(p => p.ToString()).ToList();
            }
            else if (tipo == EnumTipoCampo.NumeroDecimal)
            {
                val = objects.Select(p => string.IsNullOrWhiteSpace(p.ToString())
                    ? "null"
                    : p.ToString().Replace(",", ".")).ToList();
            }
            else if (tipo == EnumTipoCampo.Data)
            {
                val = objects.Select(p =>
                {
                    var data = Convert.ToDateTime(p);
                    return string.Format("TO_DATE('{0:dd/MM/yyyy}','DD/MM/YYYY')", data);
                }).ToList();
            }
            else if (tipo == EnumTipoCampo.SimNao)
            {
                val = objects.Select(p =>
                {
                    bool boolVal;
                    if (bool.TryParse(p.ToString(), out boolVal))
                    {
                        return boolVal ? "1" : "0";
                    }
                    if (p.ToString() == "1")
                    {
                        return "1";
                    }
                    return "0";
                }).ToList();
            }
            else if (tipo == EnumTipoCampo.DataHora)
            {
                val = objects.Select(p =>
                {
                    var dataHora = Convert.ToDateTime(p);
                    return string.Format("TO_DATE('{0:dd/MM/yyyy HH:mm:ss}','DD/MM/YYYY HH24:mi:ss')", dataHora);
                }).ToList();
            }

            return new ObjetoResolvidoPersona
            {
                Instrucao = string.Format("{0} is null or {0} in ({1})", nome, string.Join(",", val))
            };
        }
    }
}
