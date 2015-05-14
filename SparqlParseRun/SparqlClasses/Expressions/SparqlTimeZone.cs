using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlTimeZone : SparqlExpression
    {
        private SparqlExpression sparqlExpression;

        public SparqlTimeZone(SparqlExpression value)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            Func = result =>
            {
                var f = value.Func(result).Content;
                if (f is DateTime)
                    return new OV_dayTimeDuration(TimeZoneInfo.Utc.GetUtcOffset((DateTime)f));
                else if(f is DateTimeOffset)
                    return new OV_dayTimeDuration(((DateTimeOffset)f).Offset);
                throw new ArgumentException();
            };
        }
    }
}
