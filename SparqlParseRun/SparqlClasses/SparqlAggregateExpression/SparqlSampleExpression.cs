using System;
using System.Linq;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SparqlAggregateExpression
{
    class SparqlSampleExpression : SparqlAggregateExpression
    {
        public SparqlSampleExpression()
            : base()
        {
            Random random=new Random();
            TypedOperator = result =>
            {
                    var spraqlGroupOfResults = (result as SpraqlGroupOfResults);
                if (spraqlGroupOfResults != null)
                    return
                        Expression.TypedOperator(
                            spraqlGroupOfResults.Group.ElementAt(random.Next(spraqlGroupOfResults.Group.Count())));
                else throw new Exception();
            };
        }
    }
}
