using System;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlIf : SparqlExpression
    {
        public SparqlIf(SparqlExpression conditionExpression1, SparqlExpression sparqlExpression2, SparqlExpression sparqlExpression3)
        {
            IsDistinct = conditionExpression1.IsDistinct || sparqlExpression2.IsDistinct || sparqlExpression3.IsDistinct;
            IsAggragate = conditionExpression1.IsAggragate || sparqlExpression2.IsAggragate || sparqlExpression3.IsAggragate;
            //todo SetVariabletype
            TypedOperator = result =>
            {
                var condition = conditionExpression1.TypedOperator(result).Content;
                if (condition is bool)
                {
                    return (bool)condition ? sparqlExpression2.TypedOperator(result) : sparqlExpression3.TypedOperator(result);
                }   throw new ArgumentException();
            };
        }
    }
}
