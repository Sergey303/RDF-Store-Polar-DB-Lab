using System;
using System.Collections.Generic;
using System.Linq;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SparqlAggregateExpression
{
    class SparqlAvgExpression : SparqlAggregateExpression
    {
        public SparqlAvgExpression():base()
        {
            TypedOperator = result =>
            {
                if (result is SpraqlGroupOfResults)
                {
                    var @group = (result as SpraqlGroupOfResults).Group.ToArray();
                    if(group.Length==0) throw new NotImplementedException();
                    if (group.Length == 1) return Expression.TypedOperator(group[0]);
                    return Expression.TypedOperator(group[0]).Change(o=> @group.Average(sparqlResult => (double)Expression.TypedOperator(sparqlResult).Content));
                }
                else throw new Exception();
            };
        }
    }
}
