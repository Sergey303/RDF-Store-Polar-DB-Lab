using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlLiteralExpression : SparqlExpression
    {
        private ObjectVariants sparqlLiteralNode;

        public SparqlLiteralExpression(ObjectVariants sparqlLiteralNode)
        {
            // TODO: Complete member initialization
            this.sparqlLiteralNode = sparqlLiteralNode;
            Func = result => sparqlLiteralNode;
        }
    }
}
