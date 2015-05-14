using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlIsBlank : SparqlExpression
    {
        private SparqlExpression sparqlExpression;

        public SparqlIsBlank(SparqlExpression value)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
           SetVariablesTypes(ExpressionType.@bool);
            Func = result => new OV_bool(sparqlExpression.Func(result) is IBlankNode); //todo 
        }
    }
}
