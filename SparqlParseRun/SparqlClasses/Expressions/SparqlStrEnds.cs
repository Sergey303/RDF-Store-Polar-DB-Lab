using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStrEnds : SparqlExpression
    {

        public SparqlStrEnds(SparqlExpression str, SparqlExpression pattern)
        {

            IsAggragate = pattern.IsAggragate || str.IsAggragate;
            IsDistinct = pattern.IsDistinct || str.IsDistinct;
            TypedOperator = result => str.TypedOperator(result).Change(o => o.EndsWith(pattern.TypedOperator(result).Content));

        }
    }
}
