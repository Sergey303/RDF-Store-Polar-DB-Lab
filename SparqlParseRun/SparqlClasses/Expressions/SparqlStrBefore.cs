using System;
using RDFCommon;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStrBefore : SparqlExpression
    {
        public SparqlStrBefore(SparqlExpression str, SparqlExpression pattern, INodeGenerator q)
        {
            IsAggragate = pattern.IsAggragate || str.IsAggragate;
            IsDistinct = pattern.IsDistinct || str.IsDistinct;
           str.SetVariablesTypes(ExpressionType.@stringOrWithLang);
                pattern.SetVariablesTypes(ExpressionType.@stringOrWithLang);

                SetVariablesTypes(ExpressionType.@stringOrWithLang);


            Func = result =>
            {
                var patternValue = pattern.Func(result);
                return str.Func(result).Change(o => StringBefore(o, patternValue.Content));
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
