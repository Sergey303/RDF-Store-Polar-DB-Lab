using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlLangMatches : SparqlExpression
    {
        public SparqlLangMatches(SparqlExpression value, SparqlExpression sparqlExpression)
        {
            IsDistinct = value.IsDistinct || sparqlExpression.IsDistinct;
            IsAggragate = value.IsAggragate || sparqlExpression.IsAggragate;
            value.SetVariablesTypes(ExpressionType.@string); //todo lang
            sparqlExpression.SetVariablesTypes(ExpressionType.@string); //todo lang
            Func = result =>
            {
                var lang = value.Func(result);
                var langRange = sparqlExpression.Func(result);
                return new OV_bool(Equals(langRange.Content, "*")
                    ? !string.IsNullOrWhiteSpace(langRange.Content)
                    : Equals(lang, langRange));
          
            };
        }
    }
}