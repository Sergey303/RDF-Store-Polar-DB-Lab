
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
   public class SparqlBoolLiteralExpression : SparqlExpression  
    {
      //  private SparqlBoolLiteralNode sparqlLiteralNode;

        public SparqlBoolLiteralExpression(ObjectVariants sparqlLiteralNode)
        {
            Func = result => sparqlLiteralNode;
        }
           }

}
