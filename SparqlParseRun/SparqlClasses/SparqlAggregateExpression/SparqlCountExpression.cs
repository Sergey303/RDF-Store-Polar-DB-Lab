using System;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SparqlAggregateExpression
{
    class SparqlCountExpression : SparqlAggregateExpression
    {
        public SparqlCountExpression() :base()
        {
            if(isAll)
                Func = result =>
                {
                    if (result is SpraqlGroupOfResults)
                    {
                        return new OV_int(((SpraqlGroupOfResults)result).Group.Count());
                    }
                    else throw new Exception();
                };
            else
                Func = result =>
            {
                if (result is SpraqlGroupOfResults)
                {
                    return new OV_int(((SpraqlGroupOfResults)result).Group.Count());//sparqlResult => Expression.Func(sparqlResult)
                }
                else throw new Exception();
            };
        }
    }
}
