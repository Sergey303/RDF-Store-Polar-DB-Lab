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
             SetExprType(ObjectVariantEnum.Bool);
            TypedOperator = result => new OV_bool(sparqlExpression.TypedOperator(result) is ObjectVariants);

        }
    }
}
