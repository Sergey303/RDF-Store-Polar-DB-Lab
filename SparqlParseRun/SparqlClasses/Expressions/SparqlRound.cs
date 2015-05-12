using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlRound : SparqlExpression
    {
        private SparqlExpression sparqlExpression;

        public SparqlRound(SparqlExpression value)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
          
            sparqlExpression = value;
            TypedOperator = result => value.TypedOperator(result).Change(o => Math.Round(o));
        }
    }
}
