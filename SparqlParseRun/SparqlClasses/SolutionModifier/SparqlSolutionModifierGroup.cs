using System.Collections.Generic;
using System.Linq;
using SparqlParseRun.SparqlClasses.Query.Result;
using SparqlParseRun.SparqlClasses.SparqlAggregateExpression;

namespace SparqlParseRun.SparqlClasses.SolutionModifier
{
    public class SparqlSolutionModifierGroup : List<SparqlGroupConstraint>
    {
        private readonly RdfQuery11Translator q;

        public SparqlSolutionModifierGroup(RdfQuery11Translator q)
        {
            this.q = q;
        }

        public IEnumerable<SparqlResult> Group(IEnumerable<SparqlResult> enumerable)
        {
               if(Count==1)
                   if (this[0].Variable != null)
                   {
                       return enumerable.GroupBy(result => this[0].Constrained(result)).Select( grouping => new SpraqlGroupOfResults(this[0].Variable, grouping.Key) { Group = grouping });
                   }
                   else return enumerable.GroupBy(result => this[0].Constrained(result)).Select(grouping => new SpraqlGroupOfResults() { Group = this[0].IsDistinct ? grouping.Distinct() : grouping });
               var isDictinct = this.Any(constraint => constraint.IsDistinct);
                   
            var groups = enumerable
                    .GroupBy(result =>this.Select(constraint => constraint.Constrained(result)).ToList(), new CollectionEqualityComparer())
                    .Select(grouping => new SpraqlGroupOfResults(this.Select(constraint => constraint.Variable), grouping.Key) { Group = isDictinct ? grouping.Distinct() : grouping })
                .ToArray();
           
            return groups;
        }
    }
}
