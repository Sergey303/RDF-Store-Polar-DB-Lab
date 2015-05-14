using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlHours : SparqlExpression
    {
        public SparqlHours(SparqlExpression value)
        {
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            value.SetVariablesTypes(ExpressionType.withDate);
            SetVariablesTypes(ExpressionType.@int);
            Func = result =>
            {
                var f = value.Func(result).Content;
                if (f is DateTime)
                    return new OV_int(((DateTime)f).Hour);
                throw new ArgumentException();
            };
        }
    }
}
