using RDFCommon;
using RDFTripleStore;


namespace SparqlEndpointForm
{
    public class RdfStores
    {
        public static StoreCascadingInt Store;

        public RdfStores()
        {
            Store = new StoreCascadingInt("../../../Databases/int based/");
            Store.Start();
        }
        
    }
}