using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using RDFCommon;
using RDFTripleStore.OVns;
using SparqlParseRun;

namespace RDFTripleStore
{
    class SparqlStore :GoGraphStringBased, IStore
    {
        public SparqlStore(string path) : base(path)
        {
        }

        public void ReloadFrom(string fileName)
        {
            ClearAll();
            FromTurtle(fileName);    
        }

        public string Run(string query)
        {
            var parser = new sparq11lTranslatorParser(new CommonTokenStream(new sparq11lTranslatorLexer(new AntlrInputStream(query))))
            {
                q=new RdfQuery11Translator(this)
            };
            var queryContext = parser.query();
            var sparqlResultSet = queryContext.value.Run(this);
            return sparqlResultSet.ToJson();
        }

        public IStoreNamedGraphs NamedGraphs { get; private set; }
        public void ClearAll()
        {
           base.Clear();
        }

        public IGraph CreateTempGraph()
        {
           return new RamListOfTriplesGraph(ng.CreateUriNode("temp"));
        }
    }
}
