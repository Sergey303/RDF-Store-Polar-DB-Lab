using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlMinutes : SparqlExpression
    {
        private SparqlExpression sparqlExpression;

        public SparqlMinutes(SparqlExpression value)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
           Func = result =>
            {
                var f = value.Func(result).Content;        //todo offeset
                if (f is DateTime)
                    return new OV_int(((DateTime)f).Minute);
                throw new ArgumentException();
            };
        }
    }
}
