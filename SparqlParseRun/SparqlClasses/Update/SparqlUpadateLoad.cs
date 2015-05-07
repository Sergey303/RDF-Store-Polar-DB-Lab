using System;
using System.Net;
using System.Xml.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.Update
{
    public class SparqlUpdateLoad : SparqlUpdateSilent
    {
        private ObjectVariants from;
        public string Graph;

        internal void SetIri(ObjectVariants sparqlUriNode)
        {
           from = sparqlUriNode;
        }

        internal void Into(string sparqlUriNode)
        {
            Graph = sparqlUriNode;
        }


        public override void RunUnSilent(IStore store)
        {
            using (WebClient wc = new WebClient())
            {
                //  wc.Headers[HttpRequestHeader.ContentType] = "application/sparql-query"; //"query="+ 
                string gString = wc.DownloadString(((ObjectVariants)from).Content);
                var graph = (Graph != null)
                    ? store.NamedGraphs.CreateGraph(Graph)
                    : store;
                try
                {
                    var gXml = XElement.Parse(gString);
                    graph.FromXml(gXml);
                }
                catch (Exception)
                {
                    try
                    {
                        graph.FromTurtle(gString);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
