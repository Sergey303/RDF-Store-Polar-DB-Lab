using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RDFCommon;
using RDFCommon.OVns;
using RDFTripleStore;
using SparqlParseRun;
using SparqlParseRun.SparqlClasses;
using SparqlParseRun.SparqlClasses.Query;
using SparqlParseRun.SparqlClasses.Query.Result;
using UnitTest;

namespace TestingNs
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = new StoreCascadingInt(@"..\..\..\Databases\int based\");
             store.ClearAll();
            //Performance.ComputeTime(() => store.ReloadFrom(Config.Source_data_folder_path + "10M.ttl"), "load 10 млн ", true);
            //store.AddFromXml(XElement.Load(@"C:\deployed\0001.xml"));
            //File.WriteAllText(@"C:\deployed\all.fog.ttl", store.ToTurtle());
            store.ReloadFrom(@"..\..\all.fog.ttl");
            return;
            //   store.ActivateCache();
            store.Start();
            ObjectVariants temp;
            store.NodeGenerator.TryGetUri(new OV_iri("http://fogid.net/e/svet_100616111408_2835"), out temp);
            var code = ((NodeGeneratorInt)store.NodeGenerator).coding_table.GetCodeByString("http://fogid.net/e/svet_100616111408_2835");

            var sparqlQuery = SparqlQueryParser.Parse(store, "select * {<http://fogid.net/e/svet_100616111408_2835> ?p ?o}");
            SparqlResultSet sparqlResultSet = sparqlQuery.Run();
            Console.WriteLine(sparqlResultSet.ToJson());
        }


    }
}