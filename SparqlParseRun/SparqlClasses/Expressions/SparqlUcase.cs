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
            Func = result => value.Func(result).Change(o => o.ToUpperInvariant());
        }
    }
}
