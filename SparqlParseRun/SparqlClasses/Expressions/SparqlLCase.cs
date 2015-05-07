using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlLCase :SparqlExpression
    {
        private SparqlExpression sparqlExpression;

        public SparqlLCase(SparqlExpression value, INodeGenerator q)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            Func = result => value.Func(result).Change(o => o.ToLower());
        }
    }
}
