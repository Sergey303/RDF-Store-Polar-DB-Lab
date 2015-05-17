using System;
using RDFCommon;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlTz : SparqlExpression
    {
        public SparqlTz(SparqlExpression value, NodeGenerator q)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            TypedOperator = result =>
            {
                var f = value.TypedOperator(result).Content;
                if (f is DateTimeOffset)
                {
                    return new OV_string(((DateTimeOffset)f).Offset.ToString());
                }
                else if(f is DateTime) return    new OV_string("");
                throw new ArgumentException();
            };
        }
    }
}
