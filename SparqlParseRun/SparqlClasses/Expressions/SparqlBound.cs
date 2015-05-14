using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlBound : SparqlExpression
    {
        public SparqlBound(VariableNode value)
        {
            SetVariablesTypes(ExpressionType.@bool);
            Func = result => new OV_bool(result.ContainsKey(value));
        }
    }
}