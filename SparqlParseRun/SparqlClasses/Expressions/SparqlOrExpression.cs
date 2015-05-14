namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlOrExpression : SparqlExpression
    {
        public SparqlOrExpression(SparqlExpression l, SparqlExpression r)
        {
            IsAggragate = l.IsAggragate || r.IsAggragate;
            IsDistinct = l.IsDistinct || r.IsDistinct;
            l.SetVariablesTypes(ExpressionType.@bool);
            r.SetVariablesTypes(ExpressionType.@bool);
            SetVariablesTypes(ExpressionType.@bool);
            Func = result => l.Func(result).Change(ll => ll || r.Func(result).Content);
        }
    }
}
