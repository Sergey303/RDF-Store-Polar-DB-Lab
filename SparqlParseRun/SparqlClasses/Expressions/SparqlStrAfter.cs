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
           str.SetVariablesTypes(ExpressionType.@stringOrWithLang);
           pattern.SetVariablesTypes(ExpressionType.@stringOrWithLang);

            SetVariablesTypes(ExpressionType.@stringOrWithLang);

            
            Func = result =>
            {
                var patternValue = pattern.Func(result);
               return str.Func(result).Change(o => StringAfter(o, patternValue.Content));
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
