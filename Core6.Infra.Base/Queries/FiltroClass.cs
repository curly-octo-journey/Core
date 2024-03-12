using System;
using System.Linq;

namespace Core6.Infra.Base.Queries
{
    public class FiltroClass
    {
        public const string csEquals = "equal";
        public const string csGreater = "greater";
        public const string csLess = "less";
        public const string csContains = "contains";
        public const string csContainsAll = "containsall";
        public const string csIn = "in";
        public const string csGreaterOrEqual = "greaterOrEqual";
        public const string csLessOrEqual = "lessOrEqual";
        public const string IniciaCom = "startswith";
        public const string TerminaCom = "endswith";
        public const string csInOrNull = "inOrNull";

        public FiltroClass()
        {
            @operator = csEquals;
            And = true;
            Not = false;
        }

        public string property { get; set; }
        public object value { get; set; }
        public string @operator { get; set; }
        public OperadorFiltros? Operador { get; set; }
        public bool And { get; set; }
        public bool Not { get; set; }
        public bool MainFilter { get; set; }

        public OperadorFiltros GetOperador()
        {
            if (Operador.HasValue)
                return Operador.Value;
            else
            {
                switch (@operator)
                {
                    case csEquals:
                        return OperadorFiltros.Igual;
                    case csGreater:
                        return OperadorFiltros.Maior;
                    case csLess:
                        return OperadorFiltros.Menor;
                    case csContains:
                        return OperadorFiltros.Contem;
                    case csContainsAll:
                        return OperadorFiltros.ContemTodos;
                    case csIn:
                        return OperadorFiltros.In;
                    case csGreaterOrEqual:
                        return OperadorFiltros.MaiorOuIgual;
                    case csLessOrEqual:
                        return OperadorFiltros.MenorOuIgual;
                    case IniciaCom:
                        return OperadorFiltros.IniciaCom;
                    case TerminaCom:
                        return OperadorFiltros.TerminaCom;
                    case csInOrNull:
                        return OperadorFiltros.InOuNull;
                    default:
                        throw new Exception(string.Format("@operator não identificado para '{0}'", @operator));
                }
            }
        }

        public string NomeDaPropriedade
        {
            get
            {
                string strProperty = property != null ? property : string.Empty;
                string[] strPropertySplit = strProperty.Split('.');
                return strPropertySplit[strPropertySplit.Count() - 1];
            }
        }
    }

    public enum OperadorFiltros
    {
        Igual,
        Maior,
        Menor,
        Contem,
        ContemTodos,
        In,
        MaiorOuIgual,
        MenorOuIgual,
        IniciaCom,
        TerminaCom,
        InOuNull

        /*
                {operator: 'contains',   label: _('contains')},
                {operator: 'regex',      label: _('reg. exp.')},
                {operator: 'equals',     label: _('is equal to')},
                {operator: 'greater',    label: _('is greater than')},
                {operator: 'less',       label: _('is less than')},
                {operator: 'not',        label: _('is not')},
                {operator: 'in',         label: _('one of')},
                {operator: 'notin',      label: _('none of')},
                {operator: 'before',     label: _('is before')},
                {operator: 'after',      label: _('is after')},
                {operator: 'within',     label: _('is within')},
                {operator: 'inweek',     label: _('is in week no.')},
                {operator: 'startswith', label: _('starts with')},
                {operator: 'endswith',   label: _('ends with')},
                {operator: 'definedBy',  label: _('defined by')}
     
      
       // filter operators
        if (this.operators.length == 0) {
            switch (this.valueType) {
                case 'string':
                    this.operators.push('contains', 'equals', 'startswith', 'endswith', 'not', 'in');
                    break;
                case 'customfield':
                    this.operators.push('contains', 'equals', 'startswith', 'endswith', 'not');
                    break;
                case 'date':
                    this.operators.push('equals', 'before', 'after', 'within', 'inweek');
                    break;
                case 'number':
                case 'percentage':
                    this.operators.push('equals', 'greater', 'less');
                    break;
                default:
                    this.operators.push(this.defaultOperator);
                    break;
            }
        }
    
     * 
     */
    }
}