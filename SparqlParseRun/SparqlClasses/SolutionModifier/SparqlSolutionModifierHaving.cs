using System.Collections.Generic;
using System.Linq;
using SparqlParseRun.SparqlClasses.Expressions;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SolutionModifier
{
    public class SparqlSolutionModifierHaving :List<SparqlExpression>
    {

        internal IEnumerable<SparqlResult> Having(IEnumerable<SparqlResult> enumerable, RdfQuery11Translator q)
        {
            var resultsGroups = enumerable as SparqlResult[] ?? enumerable.ToArray();
            if (resultsGroups.Length == 0) return resultsGroups;
            return resultsGroups[0] is SpraqlGroupOfResults
                ? resultsGroups.Where(results => this.All(expression => expression.Test(results)))
                : (this.All(expression => expression.Test(new SpraqlGroupOfResults(q) {Group = resultsGroups}))
                    ? resultsGroups
                    : Enumerable.Empty<SparqlResult>());
        }
    }
}
