using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses;

namespace RDFTripleStore
{
    public class RamListOftriplesStore : RamListOfTriplesGraph, IStore
    {
        readonly RdfNamedGraphs graphs=new RdfNamedGraphs();

        public RamListOftriplesStore(string name)
            : base(name)
        {
        }

        public RamListOftriplesStore()
        {
         
        }

        public IStoreNamedGraphs NamedGraphs { get { return null; } }
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