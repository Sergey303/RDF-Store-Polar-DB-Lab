using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlNumLiteralExpression : SparqlExpression
    {
        public SparqlNumLiteralExpression(ObjectVariants sparqlLiteralNode)
        {
            SetVariablesTypes(ExpressionType.numeric);
            Func = result => sparqlLiteralNode;
        }
    }
}
