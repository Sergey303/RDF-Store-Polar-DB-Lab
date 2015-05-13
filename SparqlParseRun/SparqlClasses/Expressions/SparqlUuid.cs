using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlUuid : SparqlExpression
    {
        public SparqlUuid(NodeGenerator q)
        {                          
          
            TypedOperator = result => q.GetUri("urn:uuid:" + Guid.NewGuid());
        }
    }
}
