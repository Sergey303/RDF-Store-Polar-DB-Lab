using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlNumLiteralExpression : SparqlExpression
    {
        public SparqlNumLiteralExpression(ObjectVariants sparqlLiteralNode)     :base(VariableDependenceGroupLevel.Const)
        {
            //SetExprType(ExpressionTypeEnum.numeric);
            Const = sparqlLiteralNode;
            //TypedOperator = result => sparqlLiteralNode;
        }
    }
}
