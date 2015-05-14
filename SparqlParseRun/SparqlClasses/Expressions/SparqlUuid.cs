using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlUuid : SparqlExpression
    {
        public SparqlUuid(NodeGenerator q)
        {                          
          
            Func = result => q.GetUri("urn:uuid:" + Guid.NewGuid());
        }
    }
}
