using System;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlBinaryExpression : SparqlExpression
    {
        public SparqlBinaryExpression(SparqlExpression l, SparqlExpression r, Func<dynamic, dynamic, dynamic> @operator)
        {
            var lc = l.Const;
            var rc = r.Const;
            
            switch (NullablePairExt.Get(lc, rc))
            {
                case NP.bothNull:
                    Operator = result => @operator(l.Operator(result), r.Operator(result));
                    TypedOperator = result => l.TypedOperator(result).Change(o => @operator(o, r.Operator(result)));
                   AggregateLevel= SetAggregateLevel(l.AggregateLevel, r.AggregateLevel);
                    break;
                case NP.leftNull:
                    Operator = result => @operator(l.Operator(result), rc.Content);
                    TypedOperator = result => l.TypedOperator(result).Change(o => @operator(o, rc.Content));
                    AggregateLevel = l.AggregateLevel;
                    break;
                case NP.rigthNull:
                    Operator = result => @operator(lc.Content, r.Operator(result));
                    TypedOperator = result => lc.Change(o => @operator(o, r.Operator(result)));
                    AggregateLevel = r.AggregateLevel;
                    break;
                case NP.bothNotNull:
                    Const = lc.Change(ll => @operator(ll, rc.Content));
                    break;
            }
        }

        
    }
    public class SparqlBinaryExpression<T> : SparqlExpression
    {
        public SparqlBinaryExpression(SparqlExpression l, SparqlExpression r, Func<dynamic, dynamic, dynamic> @operator, Func<dynamic, T> typedCtor)
        {
            var lc = l.Const;
            var rc = r.Const;

            switch (NullablePairExt.Get(lc, rc))
            {
                case NP.bothNull:
                    Operator = result => @operator(l.Operator(result), r.Operator(result));
                    AggregateLevel = SetAggregateLevel(l.AggregateLevel, r.AggregateLevel);
                    break;
                case NP.leftNull:
                    Operator = result => @operator(l.Operator(result), rc.Content);
                    AggregateLevel = l.AggregateLevel;
                    break;
                case NP.rigthNull:
                    Operator = result => @operator(lc.Content, r.Operator(result));
                    AggregateLevel = r.AggregateLevel;
                    break;
                case NP.bothNotNull:
                    Const = typedCtor(@operator(lc.Content, rc.Content));
                    break;
            }
            TypedOperator = res => typedCtor(Operator(res));
        }


    }
}