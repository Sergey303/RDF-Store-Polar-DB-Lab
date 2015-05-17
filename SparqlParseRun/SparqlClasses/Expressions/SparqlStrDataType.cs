using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStrDataType : SparqlExpression
    {
     

        public SparqlStrDataType(SparqlExpression sparqlExpression1, SparqlExpression sparqlExpression2, NodeGenerator q)
        {
            // TODO: Complete member initialization
       
            TypedOperator = result =>
            {                                                 
                string str = (string) sparqlExpression1.TypedOperator(result).Content;
                string type = (string) sparqlExpression2.TypedOperator(result).Content;
                return q.CreateLiteralNode(str, type);
            };
        }
    }
}
