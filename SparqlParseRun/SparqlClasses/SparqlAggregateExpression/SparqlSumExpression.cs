using System;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SparqlAggregateExpression
{
    class SparqlSumExpression : SparqlAggregateExpression
    {
        public SparqlSumExpression() :base()
        {

            Func = result =>
            {
                var @group = ((SpraqlGroupOfResults) result).Group.ToArray();
                if (group.Length == 0) return new OV_int(0);
                var firsts = Expression.Func(@group[0]);
                if (group.Length == 1)
                    return firsts;
                return firsts.Change(f => f + @group.Skip(1).Sum(sparqlResult => (double)Expression.Func(sparqlResult).Content));
            };
        }
    }
}
