using System;
using RDFCommon;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlAbs : SparqlExpression
    {
        public SparqlAbs(SparqlExpression value)
        {
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            value.SetVariablesTypes(ExpressionType.numeric);
            Func = result => value.Func(result).Change(o => Math.Abs(o));
        }
    }
}