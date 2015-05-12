using System;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlBinaryExpression : SparqlExpression
    {
        public SparqlBinaryExpression(SparqlExpression l, SparqlExpression r, Func<object,object, object> @operator )
        {
            
            IsDistinct = l.IsDistinct || r.IsDistinct;
            IsAggragate = l.IsAggragate || r.IsAggragate;            
            var lc = l.Const;
            var rc = r.Const;
            l.SyncTypes(r);
            object c;
            switch (NullablePairExt.Get(lc, rc))
            {
                case NP.bothNull:
                    Operator = result => @operator(r.Operator(result), l.Operator(result));
                    TypedOperator = result => r.TypedOperator(result).Change(rr => @operator(l.Operator(result), rr));
                    break;
                case NP.leftNull:
                    c = rc.Content;
                    Operator = result => @operator(c, l.Operator(result));
                    TypedOperator = result => l.TypedOperator(result).Change(ll => @operator(ll, c));
                    break;
                case NP.rigthNull:
                    c = lc.Content;
                    Operator = result => @operator(r.Operator(result), c);
                    TypedOperator = result => r.TypedOperator(result).Change(rr => @operator(c, rr));
                    break;
                case NP.bothNotNull:
                    Const = lc.Change(ll => @operator(ll, rc.Content));
                    break;
            }
        }

        
    }
}