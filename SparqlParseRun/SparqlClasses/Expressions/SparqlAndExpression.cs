using System;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlAndExpression : SparqlExpression
    {
        public SparqlAndExpression(SparqlExpression l, SparqlExpression r)
        {
            IsDistinct = l.IsDistinct || r.IsDistinct;
            IsAggragate = l.IsAggragate || r.IsAggragate;

            l.SetVariablesTypes(ExpressionType.@bool); 
            r.SetVariablesTypes(ExpressionType.@bool);
            SetVariablesTypes(ExpressionType.@bool);
          
            Func = result => l.Func(result).Change(ll => ll && r.Func(result).Content);   
        }
    }
}
