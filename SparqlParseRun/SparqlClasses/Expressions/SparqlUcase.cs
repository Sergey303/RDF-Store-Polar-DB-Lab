using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlUcase :SparqlExpression
    {
        public SparqlUcase(SparqlExpression value, NodeGenerator q)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;    
            TypedOperator = result => value.TypedOperator(result).Change(o => o.ToUpperInvariant());
        }
    }
}
