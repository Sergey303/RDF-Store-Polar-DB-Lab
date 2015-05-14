using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlIsIri : SparqlExpression
    {
        private SparqlExpression sparqlExpression;

        public SparqlIsIri(SparqlExpression value)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
             SetVariablesTypes(ExpressionType.@bool);
            Func = result => new OV_bool(sparqlExpression.Func(result) is ObjectVariants);

        }
    }
}
