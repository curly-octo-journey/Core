using Core6.Infra.Base.Personalizacao.Consultas;
using System;

namespace Core6.Infra.Base.Personalizacao.Consultas.Operadores
{
    public class ResolvedorOperadorMaiorPer : IResolvedorOperadorPersonal
    {
        public ObjetoResolvidoPersona Resolve(EnumTipoCampo tipo, string nome, object valores)
        {
            var val = string.Empty;

            if (tipo == EnumTipoCampo.Texto || tipo == EnumTipoCampo.TextoMultiLinha)
            {
                val = string.Format("'{0}'", valores.ToString().ToUpper());
                nome = "upper(" + nome + ")";
            }
            else if (tipo == EnumTipoCampo.Selecao || tipo == EnumTipoCampo.NumeroInteiro)
            {
                val = string.Format("{0}", valores);
            }
            else if (tipo == EnumTipoCampo.NumeroDecimal)
            {
                val = string.IsNullOrWhiteSpace(valores.ToString())
                    ? "null"
                    : valores.ToString().Replace(",", ".");
            }
            else if (tipo == EnumTipoCampo.Data)
            {
                var data = Convert.ToDateTime(valores);
                val = string.Format("TO_DATE('{0:dd/MM/yyyy}','DD/MM/YYYY')", data);
            }
            else if (tipo == EnumTipoCampo.SimNao)
            {
                bool boolVal;
                if (bool.TryParse(valores.ToString(), out boolVal))
                {
                    val = boolVal ? "1" : "0";
                }
                else if (valores.ToString() == "1")
                {
                    val = "1";
                }
                else
                {
                    val = "0";
                }
            }
            else if (tipo == EnumTipoCampo.DataHora)
            {
                var dataHora = Convert.ToDateTime(valores);
                val = string.Format("TO_DATE('{0:dd/MM/yyyy HH:mm:ss}','DD/MM/YYYY HH24:mi:ss')", dataHora);
            }

            return new ObjetoResolvidoPersona
            {
                Instrucao = string.Format("{0} > {1}", nome, val)
            };
        }
    }
}
