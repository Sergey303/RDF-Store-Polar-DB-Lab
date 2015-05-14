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
       
            Func = result =>
            {                                                 
                var str = sparqlExpression1.Func(result).Content;
                var type = sparqlExpression2.Func(result).Content;
                if (str is string)
                    return q.CreateLiteralNode(str, type);
                throw new ArgumentException();
            };
        }
    }
}
