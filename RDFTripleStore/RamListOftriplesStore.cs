using RDFCommon;

namespace RDFTripleStore
{
    public class RamListOftriplesStore : RamListOfTriplesGraph, IStore
    {
        readonly RdfNamedGraphs graphs=new RdfNamedGraphs();

        public RamListOftriplesStore(IGraphNode name) : base(name)
        {
        }

        public RamListOftriplesStore()
        {
         
        }

        public IStoreNamedGraphs NamedGraphs { get { return graphs; } }
        public void ClearAll()
        {
            Clear();
            NamedGraphs.ClearAllNamedGraphs();
        }

        public IGraph CreateTempGraph()
        {
            return new RamListOfTriplesGraph();
        }

        public void Close()
        {
            
        }
    }
}