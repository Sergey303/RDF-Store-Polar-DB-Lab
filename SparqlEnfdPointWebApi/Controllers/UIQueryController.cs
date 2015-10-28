using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml.Linq;
using RDFCommon;
using SparqlEndpointForm;
using SparqlParseRun.SparqlClasses;

namespace SparqlEnfdPointWebApi.Controllers
{
    [System.Web.Mvc.RoutePrefix("query")]
    public class UIQueryController :Controller
    {
        public static readonly List<string> QuerieStrings = new List<string>() 
        { @"PREFIX dc: <http://purl.org/dc/elements/1.1/> INSERT { <http://example/egbook> dc:title  'This is an example title' } WHERE {}",
            "PREFIX dc: <http://purl.org/dc/elements/1.1/> SELECT * {?x ?y ?z}",
        "CLEAR ALL",
        @"PREFIX foaf: <http://xmlns.com/foaf/0.1/>     PREFIX dc:   <http://purl.org/dc/elements/1.1/> PREFIX xsd:   <http://www.w3.org/2001/XMLSchema%23>  INSERT DATA  { dc:a  foaf:givenName  'Alice'	. dc:b  foaf:givenName  'Bob' 	. dc:c  foaf:givenName  'Carol'	.  dc:d  foaf:givenName  'Dmitriy' .  dc:b  dc:date        '2005-04-04T04:04:04'^^xsd:dateTime . }",
        @"PREFIX foaf: <http://xmlns.com/foaf/0.1/> PREFIX dc:   <http://purl.org/dc/elements/1.1/>  PREFIX xsd:   <http://www.w3.org/2001/XMLSchema%23> SELECT ?givenName  WHERE { 	?x dc:date ?date . 	SERVICE <http://localhost/SparqlEndPointWebApi/sparql> {  ?x foaf:givenName  ?givenName }          }"};

        [System.Web.Mvc.Route("get")]
        // GET: Query
        [System.Web.Mvc.HttpGet]
        public ActionResult Get()
        {
            return View("SimpleUI");
        }

        [System.Web.Mvc.Route("add")]
        // GET: Query
        [System.Web.Mvc.HttpPost]
        public ActionResult Add([FromBody] string q)
        {
          QuerieStrings.Add(q);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        [System.Web.Mvc.Route("load")]
        // GET: Query
        [System.Web.Mvc.HttpGet]
        public ActionResult LoadXml()
        {

            //var gString = System.IO.File.ReadAllText(@"");
            
                var gXml = XElement.Load(@"C:\Users\Admin\Source\Repos\RDF-Store-Polar-DB-Lab\SparqlEnfdPointWebApi\fogInOne.xml");
                RdfStores.Store.AddFromXml(gXml);
            
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

      
        [System.Web.Mvc.HttpPost]
        public ActionResult Run([FromBody] string QueryText)
        {
            try
            {
                ViewBag.SparqlResultSet = SparqlQueryParser.Parse(RdfStores.Store, QueryText).Run();
                return View("default");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("default");
            }
        }
    }
}
