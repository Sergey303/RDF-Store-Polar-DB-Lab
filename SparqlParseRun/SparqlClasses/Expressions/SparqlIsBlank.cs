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
           SetExprType(ObjectVariantEnum.Bool);
            TypedOperator = result => new OV_bool(sparqlExpression.TypedOperator(result) is IBlankNode); //todo 
        }
    }
}
