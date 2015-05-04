using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RDFCommon;
using SparqlParseRun;
using SparqlParseRun.SparqlClasses.Query.Result;
using RDFTripleStore;
using Antlr4.Runtime;

namespace TestingNs
{
    public class SparqlStore : SecondGraphString, IStore
    {
        public SparqlStore(string path) : base(path)
        {
        }

        public void ReloadFrom(string fileName)
        {
            ClearAll();
            FromTurtle(fileName);    
        }
        public void ReloadFrom(Stream baseStream)
        {
          //  ClearAll();
          //base.FromTurtle(baseStream);    
        }

        public SparqlResultSet ParseAndRun(string query)
        {
            var queryContext = Parse(query);
            return Run(queryContext);
        }

        private SparqlResultSet Run(SparqlQuery queryContext)
        {
            return queryContext.Run(this);
        }

        public SparqlQuery Parse(string query)
        {
            var parser =
                new sparq11lTranslatorParser(new CommonTokenStream(new sparq11lTranslatorLexer(new AntlrInputStream(query))))
                {
                    q = new RdfQuery11Translator(this)
                };
            var queryContext = parser.query().value;
            return queryContext;
        }

        public IStoreNamedGraphs NamedGraphs { get; private set; }
        public void ClearAll()
        {
           base.Clear();
        }

        public IGraph CreateTempGraph()
        {
           return new RamListOfTriplesGraph(NodeGenerator.CreateUriNode("temp"));
        }

       
    }
}
