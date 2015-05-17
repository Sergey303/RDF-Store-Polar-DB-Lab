using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStrAfter : SparqlExpression
    {
     

        public SparqlStrAfter(SparqlExpression str, SparqlExpression pattern, NodeGenerator q)
        {
      
            // TODO: Complete member initialization
            IsAggragate = pattern.IsAggragate || str.IsAggragate;
            IsDistinct = pattern.IsDistinct ||str.IsDistinct;
           str.SetExprType(ExpressionTypeEnum.@stringOrWithLang);
           pattern.SetExprType(ExpressionTypeEnum.@stringOrWithLang);

            SetExprType(ExpressionTypeEnum.@stringOrWithLang);

            
            TypedOperator = result =>
            {
                var patternValue = pattern.TypedOperator(result);
                return str.TypedOperator(result).Change(o => StringAfter(o, (string)patternValue.Content));
            };
          
        }

        string StringAfter(string str, string pattern)
        {
            if (Equals(pattern, string.Empty)) return string.Empty;
           int index = str.LastIndexOf(pattern, StringComparison.InvariantCultureIgnoreCase);
            if (Equals(index, -1) || (index += pattern.Length )>= str.Length) return string.Empty;
            return str.Substring(index);
        }
    }
}
