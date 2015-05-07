using System;
using RDFCommon;
using RDFCommon.OVns;


namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlContains : SparqlExpression
    {
        public SparqlContains(SparqlExpression str, SparqlExpression pattern)
        {
            IsAggragate = pattern.IsAggragate || str.IsAggragate;
            IsDistinct = pattern.IsDistinct || str.IsDistinct;
            SetVariablesTypes(ExpressionType.@bool);
            str.SetVariablesTypes(ExpressionType.stringOrWithLang); str.SetVariablesTypes(ExpressionType.stringOrWithLang);
            pattern.SetVariablesTypes(ExpressionType.stringOrWithLang); str.SetVariablesTypes(ExpressionType.stringOrWithLang);

            Func = result =>
            {
                var s = str.Func(result);
                var ps = pattern.Func(result);
                if ((s is OV_langstring && ps is OV_langstring) ||
                    (s is ILanguageLiteral && ps is OV_string) || (s is OV_string && ps is OV_string))
                    return new OV_bool(s.Content.Contains(ps.Content));
                
                throw new ArgumentException();
            };
        }
    }
}
