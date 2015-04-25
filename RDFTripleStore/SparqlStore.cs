using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using RDFTripleStore.OVns;
using SparqlParseRun;

namespace RDFTripleStore
{
    class SparqlStore   
    {
        RamListOftriplesStore store=new RamListOftriplesStore(new OV_iri("g"));

        public SparqlStore()
        {
            
        }

        public void ReloadFrom(string fileName)
        {
            store.ClearAll();
            store.FromTurtle(fileName);    
        }

        public string Run(string query)
        {
            var parser = new sparq11lTranslatorParser(new CommonTokenStream(new sparq11lTranslatorLexer(new AntlrInputStream(query))))
            {
                q=new RdfQuery11Translator(store)
            };
            var queryContext = parser.query();
            var sparqlResultSet = queryContext.value.Run(store);
            return sparqlResultSet.ToJson();
        }
    }
}
