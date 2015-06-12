using System;
using System.Collections.Generic;
using System.Linq;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.Query;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SolutionModifier
{
    public class SparqlSolutionModifier
    {
        public SparqlSolutionModifier()
        {
            //if (isDistinct)
            //Modifier = enumerable => enumerable.Distinct();
        }

        private Func<IEnumerable<SparqlResult>, IEnumerable<SparqlResult>> LimitOffset;
        private Func<IEnumerable<SparqlResult>, IEnumerable<SparqlResult>> Order;
        private Func<IEnumerable<SparqlResult>, SparqlGroupsCollection> Group;
        private SparqlSelect Select;
        private SparqlSolutionModifierHaving sparqlSolutionModifierHaving;
        private RdfQuery11Translator q;

        internal void Add(SparqlSolutionModifierLimit sparqlSolutionModifierLimit)
        {
        
                LimitOffset = sparqlSolutionModifierLimit.LimitOffset;
        }

        internal void Add(SparqlSolutionModifierOrder sparqlSolutionModifierOrder)
        {
                Order = sparqlSolutionModifierOrder.Order;
        }

        internal void Add(SparqlSolutionModifierHaving sparqlSolutionModifierHaving, RdfQuery11Translator q)
        {
            this.sparqlSolutionModifierHaving = sparqlSolutionModifierHaving;
            this.q = q;
        }

        internal void Add(SparqlSolutionModifierGroup sparqlSolutionModifierGroup)
        {
            Group = sparqlSolutionModifierGroup.Group;
        }

        internal void Add(SparqlSelect projection)
        {
            Select = projection;
        }
        public IEnumerable<SparqlResult> Run( IEnumerable<SparqlResult> results, SparqlResultSet sparqlResultSet=null)
        {
            if (Group != null)
            {
                var groupedResults = Group(results);
                if (sparqlSolutionModifierHaving != null)
                    groupedResults = sparqlSolutionModifierHaving.Having(groupedResults, q);

                if (Order != null)
                    results = Order(groupedResults.Select(r => r.Clone()));

                if (Select != null)
                    results = Select.Run(results, sparqlResultSet);

                if (LimitOffset != null)
                    results = LimitOffset(results);
                return results;
            }
            else
            {
                if (sparqlSolutionModifierHaving != null)
                    results = sparqlSolutionModifierHaving.Having(results, q);

                if (Order != null)
                    results = Order(results.Select(r => r.Clone()));

                if (Select != null)
                    results = Select.Run(results, sparqlResultSet);

                if (LimitOffset != null)
                    results = LimitOffset(results);
                return results;
            }
        }
    }
}
