using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlBound : SparqlExpression
    {
        public SparqlBound(VariableNode value)
        {
            SetExprType(ObjectVariantEnum.Bool);
            TypedOperator = result => new OV_bool(result.ContainsKey(value));
        }

   
    }
}