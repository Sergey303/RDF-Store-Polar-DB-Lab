using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using RDFCommon;
using SparqlEndpointForm;
using SparqlParseRun.SparqlClasses;
using SparqlParseRun.SparqlClasses.Query.Result;
using SparqlParseRun;

namespace SparqlEnfdPointWebApi.Controllers
{
    // [RoutePrefix("sparql")]
    public class SparqlController : Controller
    {
        [HttpGet]
        [ValidateInput(false)]
        [Route("sparql")]
        public ActionResult Get()
        {
          
            IGraph graph = null;
            var graphUri = Request["graph"];
            if (graphUri != null)
            {
                var graphUriNode = RdfStores.Store.NodeGenerator.GetUri(graphUri);
                if (graphUriNode == null)
                    return new HttpNotFoundResult();
                graph = RdfStores.Store.NamedGraphs.GetGraph(graphUri);
               // if(graph.Any())
                //return new HttpNotFoundResult();
            }
            if (Request["default"] != null)
            {
                graph = RdfStores.Store;
            }
            if (graph != null)
            {
                if (Request.AcceptTypes != null)
                {
                    if (Request.AcceptTypes.Contains("text/turtle"))
                    {
                        return Content(graph.ToTurtle(), "text/turtle");
                    }
                    else if (Request.AcceptTypes.Contains("text/xml"))
                    {
                        return Content(graph.ToXml().ToString(), "text/xml");
                    }
                    else if (Request.AcceptTypes.Contains("text/json"))
                    {
                        return Content(graph.ToJson(), "text/json");
                    }
                   
                }    else //default 
                        return Content(graph.ToTurtle(), "text/turtle");
            }

            var query = Request["query"];
            if (query != null)
            {
                var graphs = GetGraphsParames();
                //if (Request.HttpMethod == "POST" && Request.ContentType == @"application\url-encoded")
                //    query = HttpUtility.UrlDecode(query);
                var resultSet = SparqlQueryParser.Parse(RdfStores.Store, (graphs ?? "") + query).Run();
                if (Request.AcceptTypes != null && Request.AcceptTypes.Contains("text/xml"))
                {
                    return Content(resultSet.ToXml().ToString(), "text/xml");
                }
                else if (Request.AcceptTypes != null && Request.AcceptTypes.Contains("text/json"))
                {
                    return Json(resultSet.ToJson(),
                        JsonRequestBehavior.AllowGet);
                }
                else //default 
                    return Content(resultSet.ToXml().ToString(), "application/sparql-results+xml");
            }
            

            //service description   TODO
            return Content(@"<?xml version='1.0' encoding='utf-8'?>
<rdf:RDF
   xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'
   xmlns:sd='http://www.w3.org/ns/sparql-service-description#'
   xmlns:prof='http://www.w3.org/ns/owl-profile/'
   xmlns:void='http://rdfs.org/ns/void#'>
  <sd:Service>
    <sd:endpoint rdf:resource='"+Request.Path +@"/sparql/'/>  .
    <sd:supportedLanguage rdf:resource='http://www.w3.org/ns/sparql-service-description#SPARQL11Query'/>
    <sd:resultFormat rdf:resource='http://www.w3.org/ns/formats/RDF_XML'/>
    <sd:resultFormat rdf:resource='http://www.w3.org/ns/formats/Turtle'/>
    <sd:feature rdf:resource='http://www.w3.org/ns/sparql-service-description#DereferencesURIs'/>
    <sd:defaultEntailmentRegime rdf:resource='http://www.w3.org/ns/entailment/RDFS'/>            
    <sd:defaultDataset>
      <sd:Dataset>
        <sd:defaultGraph>
          <sd:Graph>
            <void:triples rdf:datatype='http://www.w3.org/2001/XMLSchema#integer'>"+RdfStores.Store.GetTriplesCount()+@"</void:triples>
          </sd:Graph>
        </sd:defaultGraph>"
               +
               string.Join(Environment.NewLine,
            RdfStores.Store.NamedGraphs.GetAllGraphCounts().Select(pair => 
        @"<sd:namedGraph>
          <sd:NamedGraph>
            <sd:name rdf:resource='"+pair.Key+@"'/>
            <sd:entailmentRegime rdf:resource='http://www.w3.org/ns/entailment/OWL-RDF-Based'/>
            <sd:supportedEntailmentProfile rdf:resource='http://www.w3.org/ns/owl-profile/RL'/>
            <sd:graph>
              <sd:Graph>
                <void:triples rdf:datatype='http://www.w3.org/2001/XMLSchema#integer'>"+pair.Value+@"</void:triples>
              </sd:Graph>
            </sd:graph>
          </sd:NamedGraph>
        </sd:namedGraph>"))
              +
       @"</sd:Dataset>
    </sd:defaultDataset>
  </sd:Service>
</rdf:RDF>", "application/rdf+xml");
        }


        [Route("sparql")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Post()
        {
            IGraph graph = null;
            var graphUri = Request["graph"];
            if (graphUri != null)
            {
                var graphUriNode = RdfStores.Store.NodeGenerator.GetUri(graphUri);
                if (graphUriNode == null)
                    return new HttpNotFoundResult();
                graph = RdfStores.Store.NamedGraphs.GetGraph(graphUri);
                if(!graph.Any())
                    return new HttpNotFoundResult();
            }
            if (Request["default"] != null)
            {
                graph = RdfStores.Store;
            }
            if (graph != null)
            {

                //default   if (Request.ContentType == "text/turtle")
                {
                    graph.FromTurtle(Request.InputStream);
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {
                var graphs = GetGraphsParames();

                var query = Request.Form["query"] ?? Request.Form["update"];
                SparqlQuery qParsed = null;
                  if (query != null)
                {
                 //   if (Request.ContentType == @"application\url-encoded") // Request.HttpMethod == "POST" && 
                  //      query = HttpUtility.UrlDecode(query);

                    qParsed = SparqlQueryParser.Parse(RdfStores.Store,(graphs ?? "") + query);
                }
                  else if (Request.ContentType == "application/sparql-update" || Request.ContentType == "application/sparql-query")
                      if (graphs == null) qParsed = SparqlQueryParser.Parse(RdfStores.Store, Request.InputStream);
                      else
                           using (StreamReader reader = new StreamReader(Request.InputStream))
                               qParsed = SparqlQueryParser.Parse(RdfStores.Store, graphs + reader.ReadToEnd());

                if (qParsed != null)
                {
                    var res = qParsed.Run();
                    if (Request.AcceptTypes != null && Request.AcceptTypes.Contains("text/xml"))
                    {
                        return Content(res.ToXml().ToString(),
                            "text/xml");
                    }
                    else if (Request.AcceptTypes != null && Request.AcceptTypes.Contains("text/json"))
                    {
                        return Json(res.ToJson(),
                            JsonRequestBehavior.AllowGet);
                    }
                    else //default 
                        return Content(res.ToXml().ToString(),
                            "application/sparql-results+xml");
                }
            }
            return new EmptyResult();     
            }

        private string GetGraphsParames()
        {
            var dgustr = Request["default-graph-uri"];
            string graphs = null; //FROM  named-graph-uri  NAMED
            if (dgustr != null)
                graphs = string.Join(Environment.NewLine, dgustr.Split(',').Select(dgu => "FROM <" + dgu + ">")) +
                         Environment.NewLine;
            var ngustr = Request["named-graph-uri"];
            if (ngustr != null)
                graphs += string.Join(Environment.NewLine, ngustr.Split(',').Select(ngu => "FROM NAMED <" + ngu + ">")) +
                          Environment.NewLine; //FROM  named-graph-uri  NAMED
            return graphs;
        }


        [Route("sparql")]
        [HttpPut]
        public ActionResult Put()
        {
            IGraph graph = null;
            var graphUri = Request["graph"];
            if (graphUri != null)
            {
                var graphUriNode = RdfStores.Store.NodeGenerator.GetUri(graphUri);
                if (graphUriNode != null)
                {
                    graph = RdfStores.Store.NamedGraphs.GetGraph(graphUri);
                }
                else
                {
                    return new HttpNotFoundResult();
                }
            }
            if (Request["default"] != null)
            {
                graph = RdfStores.Store;
            }
            if (graph != null)
            {
                //drop silent
                try
                {
                    graph.Clear();
                }
                catch (Exception)
                {
                }
                //default   if (Request.ContentType == "text/turtle")
                {
                    graph.FromTurtle(Request.InputStream);
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return new EmptyResult();
        }


        // POST: Store/Create
        [Route("sparql")]
        [HttpDelete]
        public ActionResult Delete()
        {
            var graph = Request["graph"];
            if (graph != null)
            {
                var graphUriNode = RdfStores.Store.NodeGenerator.GetUri(graph);
                IGraph g;
                if (graphUriNode != null)
                {
                    RdfStores.Store.NamedGraphs.DropGraph(graph);
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpNotFoundResult();
                }
            }
            if (Request["default"] != null)
            {
                RdfStores.Store.Clear();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return new EmptyResult();
        }
    }
}