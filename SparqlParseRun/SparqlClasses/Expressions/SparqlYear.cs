using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlYear : SparqlExpression
    {
        public SparqlYear(SparqlExpression value)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            Func = result =>
            {
                var f = value.Func(result).Content;
                if (f is DateTime)
                    return new OV_int(((DateTime)f).Year);
                if (f is DateTimeOffset)
                    return new OV_int(((DateTimeOffset)f).Year);
                throw new ArgumentException();
            };
          
        }
    }
}
