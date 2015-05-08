using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern
{
    public class SparqlMinusGraphPattern : ISparqlGraphPattern
    {
        private readonly ISparqlGraphPattern sparqlGraphPattern;

        public SparqlMinusGraphPattern(ISparqlGraphPattern sparqlGraphPattern)
        {
            // TODO: Complete member initialization
            this.sparqlGraphPattern = sparqlGraphPattern;
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            var minusResults =
                sparqlGraphPattern.Run(Enumerable.Repeat(new SparqlResult(), 1))
                    ;
            return variableBindings.Where(result => minusResults.All(minusResult =>
                minusResult.TestAll((minusVar, minusValue) =>
                {
                    var value = result[minusVar];
                    return value == null || !Equals(minusValue, value);
                })));
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.Minus;} }
    }
}
