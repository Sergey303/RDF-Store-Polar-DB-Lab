using SparqlParseRun;
using SparqlParseRun.RdfCommon;

namespace SparqlEndpointForm
{
    public class RdfStores
    {
        public static readonly   IStore Store=new Rdf2DictionaryStore("http://default");
        
    }
}