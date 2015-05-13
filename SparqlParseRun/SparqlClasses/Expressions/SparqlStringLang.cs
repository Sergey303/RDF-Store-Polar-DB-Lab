using System;
using RDFCommon;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStringLang  :SparqlExpression
    {
        public SparqlStringLang(SparqlExpression literalExpression, SparqlExpression langExpression, NodeGenerator q)
        {
            IsAggragate = langExpression.IsAggragate || literalExpression.IsAggragate;
            IsDistinct = langExpression.IsDistinct || literalExpression.IsDistinct;
            TypedOperator = result =>
            {
                string literal = (string) literalExpression.TypedOperator(result).Content;
                string lang = (string) langExpression.TypedOperator(result).Content;

                return new OV_langstring(literal, lang);
            };
        }
    }
}
