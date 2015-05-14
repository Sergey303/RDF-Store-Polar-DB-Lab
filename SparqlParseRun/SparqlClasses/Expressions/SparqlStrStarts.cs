using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStrStarts : SparqlExpression
    {


        public SparqlStrStarts(SparqlExpression str, SparqlExpression pattern)
        {
            IsAggragate = pattern.IsAggragate || str.IsAggragate;
            IsDistinct = pattern.IsDistinct || str.IsDistinct;
            Func = result => str.Func(result).Change(o => o.StartsWith(pattern.Func(result)));
        }
    }
}
