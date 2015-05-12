using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlConcat : SparqlExpression
    {
        public SparqlConcat(List<SparqlExpression> list, INodeGenerator q)
        {
             IsAggragate = list.Any(value=> value.IsAggragate);
            IsDistinct = list.Any(value=>  value.IsDistinct);
            SetExprType(ExpressionTypeEnum.stringOrWithLang);
            foreach (var sb in list)
                sb.SetExprType(ExpressionTypeEnum.stringOrWithLang);
            if (list.Count == 0) TypedOperator = r=>new OV_string(string.Empty);
            else
            TypedOperator = result =>
            {
                var values = list.Select(expression => expression.TypedOperator(result)).ToArray();
                if (values.All(o => o is OV_langstring))
                {
                    var commonLang = ((OV_langstring)values[0]).Lang;
                    if (values.Cast<OV_langstring>().All(ls => ls.Lang.Equals(commonLang)))
                        return new OV_langstring(string.Concat(values.Select(o => o.Content)), commonLang);
                }
                else if (values.All(o => o is OV_string))
                    return new OV_string(string.Concat(values.Select(v=>v.Content).Cast<string>()));
                throw new ArgumentException();
                //return q.CreateLiteralNode(string.Concat(values.Select(s => s.Content)));
            };

        }
    }
}
