namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStrLength  : SparqlExpression
    {
        public SparqlStrLength(SparqlExpression value)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;

            Operator = result => ((string)value.TypedOperator(result).Content).Length;
        }
    }
}
