using System;
using System.Diagnostics;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlFloor : SparqlExpression
    {
        public SparqlFloor(SparqlExpression value)
        {
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
          
            Func = result =>
            {
                var val = value.Func(result);
                dynamic content = val.Content;
                if (content is decimal || content is double)
                    return val.Change(d => Math.Floor(d));
                if (content is float)
                    return val.Change(d => Math.Floor((double)d));
                if (content is int) //todo uint
                    return val;
                throw new ArgumentException("Ceil " + val);
            };
        }
    }
}