using RDFCommon;
using RDFTripleStore;


namespace SparqlEndpointForm
{
    public class RdfStores
    {
        public static readonly   IStore Store=new RamListOftriplesStore("http://default");
        
    }
}