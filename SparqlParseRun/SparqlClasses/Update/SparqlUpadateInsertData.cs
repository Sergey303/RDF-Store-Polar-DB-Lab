using System;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples;

namespace SparqlParseRun.SparqlClasses.Update
{
    public class SparqlUpdateInsertData : ISparqlUpdate
    {
        private readonly SparqlQuardsPattern sparqlQuardsPattern;

        public SparqlUpdateInsertData(SparqlQuardsPattern sparqlQuardsPattern)
        {
            // TODO: Complete member initialization
            this.sparqlQuardsPattern = sparqlQuardsPattern;
        }
        public  void Run(IStore store)   
        {
            throw new NotImplementedException();
            //store.Insert( sparqlQuardsPattern.Where(pattern => pattern.PatternType == SparqlGraphPatternType.SparqlTriple)
            //    .Cast<SparqlTriple>().Select(t=>new TripleOV(t.Subject, t.Predicate,t.Object)));
            //foreach (var sparqlGraph in
            //        sparqlQuardsPattern.Where(pattern => pattern.PatternType == SparqlGraphPatternType.Graph)
            //            .Cast<SparqlGraphGraph>()
            //            //.Where(graph => store.NamedGraphs.ContainsGraph(graph.Name))
            //            )
            //    store.NamedGraphs.Insert(sparqlGraph.Name, sparqlGraph.GetTriples().Select(t => new TripleOV(t.Subject, t.Predicate, t.Object)));
        }
    }
}
