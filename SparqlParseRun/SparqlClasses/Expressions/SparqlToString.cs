using RDFCommon;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlToString : SparqlExpression
    {
       // private SparqlExpression sparqlExpression;

        public SparqlToString(SparqlExpression value, NodeGenerator q)
        {
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;

            TypedOperator = result => new OV_string(value.TypedOperator(result).Content.ToString());
        }
    }
}
