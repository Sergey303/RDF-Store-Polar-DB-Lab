using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStrUuid : SparqlExpression
    {
        public SparqlStrUuid()
        {
          
            TypedOperator = result => new OV_string(Guid.NewGuid().ToString());
        }
    }
}
