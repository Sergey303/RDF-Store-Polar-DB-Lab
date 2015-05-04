using RDFCommon;
using RDFCommon.OVns;

namespace RDFTripleStore
{
    public class RamListOftriplesStore : RamListOfTriplesGraph, IStore
    {
        readonly RdfNamedGraphs graphs=new RdfNamedGraphs();

        public RamListOftriplesStore(ObjectVariants name)
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

        public void Close()
        {
            
        }
    }
}