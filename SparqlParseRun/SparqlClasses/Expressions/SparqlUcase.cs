using System;
using RDFCommon;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlUcase :SparqlExpression
    {
        public SparqlUcase(SparqlExpression value, INodeGenerator q)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;    
            Func = result => value.Func(result).Change(o => o.ToUpperInvariant());
        }
    }
}
