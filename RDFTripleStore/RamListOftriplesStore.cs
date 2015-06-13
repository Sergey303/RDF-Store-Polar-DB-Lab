using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses;

namespace RDFTripleStore
{
    public class RamListOftriplesStore : RamListOfTriplesGraph, IStore
    {
        private readonly RdfNamedGraphs rdfNamedGraphs;

        public RamListOftriplesStore(string name)
            : base(name)
        {
            rdfNamedGraphs = new RdfNamedGraphs(NodeGenerator, s => CreateTempGraph());
        }

        public RamListOftriplesStore()
        {
            rdfNamedGraphs = new RdfNamedGraphs(NodeGenerator, s => CreateTempGraph());
        }

        public IStoreNamedGraphs NamedGraphs { get { return rdfNamedGraphs; } }
        public void ClearAll()
        {
            Clear();
            NamedGraphs.ClearAllNamedGraphs();
        }

        public IGraph CreateTempGraph()
        {
            return new RamListOfTriplesGraph();
        }

        public void ReloadFrom(string filePath)
        {
            FromTurtle(filePath);
        }

        public void Warmup()
        {
            throw new System.NotImplementedException();
        }

        public void Close()
        {
            
        }
    }
}