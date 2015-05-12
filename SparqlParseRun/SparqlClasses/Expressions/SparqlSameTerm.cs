using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlSameTerm : SparqlExpression
    {
        public SparqlSameTerm(SparqlExpression str, SparqlExpression pattern)
        {
            IsAggragate = pattern.IsAggragate || str.IsAggragate;
            IsDistinct = pattern.IsDistinct || str.IsDistinct;
            TypedOperator = result => new OV_bool(str.TypedOperator(result).Equals(pattern.TypedOperator(result))); 
        }
    }
}
