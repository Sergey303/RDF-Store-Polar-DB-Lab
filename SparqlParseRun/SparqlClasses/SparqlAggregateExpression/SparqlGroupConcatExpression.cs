using System;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SparqlAggregateExpression
{
    class SparqlGroupConcatExpression : SparqlAggregateExpression
    {
        public SparqlGroupConcatExpression() :base()
        {
            Func = result =>
            {
                var spraqlGroupOfResults = ((SpraqlGroupOfResults) result);
                try
                {
                    return new OV_string(string.Join(Separator, spraqlGroupOfResults.Group.Select(Func)));
                }
                catch
                {
                }
                return null;
            };
        }
    }
}
