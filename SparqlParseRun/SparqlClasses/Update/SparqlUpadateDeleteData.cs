using System;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples;

namespace SparqlParseRun.SparqlClasses.Update
{
    public class SparqlUpdateDeleteData : ISparqlUpdate
    {
        private readonly SparqlQuardsPattern sparqlQuardsPattern;

        public SparqlUpdateDeleteData(SparqlQuardsPattern sparqlQuardsPattern)
        {
            // TODO: Complete member initialization
            this.sparqlQuardsPattern = sparqlQuardsPattern;
        }

        public void Run(IStore store)
        {
            throw new NotImplementedException();
            foreach (var triple in sparqlQuardsPattern
                .Where(pattern => pattern.PatternType == SparqlGraphPatternType.SparqlTriple)
                .Cast<SparqlTriple>())
            {
                store.Delete(triple.Subject, triple.Predicate, triple.Object);
                
            }

            foreach (var sparqlGraphGraph in
                sparqlQuardsPattern.Where(pattern => Equals(pattern.PatternType, SparqlGraphPatternType.Graph))
                    .Cast<SparqlGraphGraph>())                                                                     
            {
               // if (sparqlGraphGraph.Name == null)
                    //store.NamedGraphs.DeleteFromAll(
                      //  sparqlGraphGraph.GetTriples().Select(t => new Triple<ObjectVariants, ObjectVariants>(t.Subject, t.Predicate, t.Object)));
                //store.NamedGraphs.Delete(sparqlGraphGraph.Name, sparqlGraphGraph.GetTriples().Select(t => new Triple<ObjectVariants, ObjectVariants>(t.Subject, t.Predicate, t.Object)));
            }
        }
    }
}
