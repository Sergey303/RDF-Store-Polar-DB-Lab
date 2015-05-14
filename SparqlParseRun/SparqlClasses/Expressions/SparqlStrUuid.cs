using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStrUuid : SparqlExpression
    {
        public SparqlStrUuid()
        {
          
            Func = result => new OV_string(Guid.NewGuid().ToString());
        }
    }
}
