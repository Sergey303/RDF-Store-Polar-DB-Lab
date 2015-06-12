using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStrUuid : SparqlExpression
    {
        public SparqlStrUuid()      :base(VariableDependenceGroupLevel.UndependableFunc)
        {

            TypedOperator = result => new OV_string(Guid.NewGuid().ToString());
            Operator = result => Guid.NewGuid().ToString();
        }
    }
}
