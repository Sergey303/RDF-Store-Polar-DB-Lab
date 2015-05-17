using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStrBefore : SparqlExpression
    {
        public SparqlStrBefore(SparqlExpression str, SparqlExpression pattern, NodeGenerator q)
        {
            IsAggragate = pattern.IsAggragate || str.IsAggragate;
            IsDistinct = pattern.IsDistinct || str.IsDistinct;
           str.SetExprType(ExpressionTypeEnum.@stringOrWithLang);
                pattern.SetExprType(ExpressionTypeEnum.@stringOrWithLang);

                SetExprType(ExpressionTypeEnum.@stringOrWithLang);


            TypedOperator = result =>
            {
                var patternValue = pattern.TypedOperator(result);
                return str.TypedOperator(result).Change(o => StringBefore(o, (string)patternValue.Content));
            };

        }
          string StringBefore(string str, string pattern)
        {
            if (pattern == string.Empty) return string.Empty;
           int index = str.LastIndexOf(pattern, StringComparison.InvariantCultureIgnoreCase);
            if (index == -1 || (index += pattern.Length )>= str.Length) return string.Empty;
            return str.Substring(index);
        }
    }
}
