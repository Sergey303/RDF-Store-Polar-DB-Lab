using System;
using RDFCommon;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlUuid : SparqlExpression
    {
        public SparqlUuid(INodeGenerator q)
        {                          
          
            TypedOperator = result => q.GetUri("urn:uuid:" + Guid.NewGuid());
        }
    }
}
