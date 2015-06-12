using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlNow : SparqlExpression
    {
        public SparqlNow()    :base(VariableDependenceGroupLevel.UndependableFunc)
        {
            Operator = res => DateTime.UtcNow;
            TypedOperator=res=>new OV_dateTime(DateTime.UtcNow);
        }
    }
}
