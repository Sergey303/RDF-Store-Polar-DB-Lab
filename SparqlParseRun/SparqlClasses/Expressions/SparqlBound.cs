using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlBound : SparqlExpression
    {
        public SparqlBound(VariableNode value)
        {
            SetExprType(ObjectVariantEnum.Bool);
            Operator = result => result.ContainsKey(value);
            TypedOperator = result => new OV_bool(result.ContainsKey(value));
        }

   
    }
}