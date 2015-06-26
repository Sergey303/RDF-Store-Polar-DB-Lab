using System;
using System.IO;
using System.Web;
using RDFCommon;
using RDFTripleStore;


namespace SparqlEndpointForm
{
    public class RdfStores
    {
        private static StoreCascadingInt store;

        public static StoreCascadingInt Store
        {
            get
            {
                if (store == null)
                {
                    store = new StoreCascadingInt(AppDomain.CurrentDomain.BaseDirectory+"../Databases/int based/");
                    store.ClearAll();
                    store.ReloadFrom(@"C:\Users\Admin\Source\Repos\RDF-Store-Polar-DB-Lab\SparqlEnfdPointWebApi\Data.ttl");
                }
                return store;
            }
        }

    
        
    }
}