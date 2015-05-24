using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses;
using SparqlParseRun.SparqlClasses.Query;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace TestingNs
{
    class Program
    {
        private static void Main(string[] args)
        {
            //RamGraph graph=new RamGraph();
            //graph.TestReadTtl_Cocor(1);
            //graph.TestSearch();

            //SparqlTesting.TestSparqlStore(1);

            //SparqlTesting.BSBm(1, false);

            //  Testing.TestExamples();
           // DirectoryInfo root = new DirectoryInfo("../../../Databases/string based/");
           //for (int i = 1; i < 13; i++)
           ////  int i = 9;
           // {

           //     DirectoryInfo dirQ = new DirectoryInfo("../../../Databases/string based/" + i + "/");
           //     if(dirQ.Exists)
           //     dirQ.Delete(true);
                
           //     dirQ.Create();
           //     foreach (var fileInfo in root.GetFiles())
           //         fileInfo.CopyTo(dirQ.FullName + "/" + fileInfo.Name);          

           //   //  using (var Store = new SecondStringSore(dirQ.FullName))

           //     {
           //         var Store = new SecondStringStore(dirQ.FullName);
           //    //       Store.ReloadFrom(Config.Source_data_folder_path + Millions + ".ttl");

           //    //   SparqlTesting.CallsAnalyze(Store, 12, 100);
           //    //SparqlTesting.OneParametrized(Store, i, 100);
           //    //   Store.Warmup();
           //    //  SparqlTesting.CacheMeasureAllWithConstants<SecondStringSore>(Store);

           //    //  SparqlTesting.TestQuery(_queryString, false, 1);
           //    //   SparqlTesting._ts.ReloadFrom(Config.Source_data_folder_path + 1 + ".ttl");
           //    //      SparqlTesting.RunBerlinsWithConstants();
           //    //    SparqlTesting.RunTestParametred(1);
           //    //    SparqlTesting.RunBerlinsParameters(Store);
           //    //SparqlTesting.CreateParameters(5, 1000, 1);  
           //    //SparqlTesting.CacheMeasure();

           //       SparqlTesting.InterpretMeas(Store, i);
           //}
           // }
            SecondStringStore store = new SecondStringStore("../../../Databases/string based/");
           // Reload(store);
            TestingPhotoPersons.testing.Run((q) =>
            {
                var sparqlQuery = SparqlQueryParser.Parse(store, q);
                if (sparqlQuery.Type == SparqlQueryTypeEnum.Ask)
                    return sparqlQuery.Run().AnyResult;
                else 
                    return sparqlQuery.Run().Results.Count();
            });
        }

        private static void Reload(SecondStringStore store)
        {
            store.Build(TestingPhotoPersons.testing.data.Generate().SelectMany(ele =>
            {
                string id = ele.Name + ele.Attribute("id").Value;
                var seq = Enumerable.Repeat(
                    new Tuple<string, string, ObjectVariants>(id, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type",
                        new OV_iri(ele.Name.LocalName)), 1)
                    .Concat(ele.Elements().Select(subele =>
                    {
                        XAttribute ratt = subele.Attribute("ref");
                        Tuple<string, string, ObjectVariants> triple = null;
                        if (ratt != null)
                        {
                            string r = (subele.Name == "reflected" ? "person" : "photo_doc") +
                                       ratt.Value;
                            triple = new Tuple<string, string, ObjectVariants>(id, subele.Name.LocalName, new OV_iri(r));
                        }
                        else
                        {
                            string value = subele.Value; // Нужны языки и другие варианты!
                            bool possiblenumber = string.IsNullOrEmpty(value) ? false : true;
                            if (possiblenumber)
                            {
                                char c = value[0];
                                if (char.IsDigit(c) || c == '-')
                                {
                                }
                                else possiblenumber = false;
                            }
                            triple = new Tuple<string, string, ObjectVariants>(id, subele.Name.LocalName,
                                possiblenumber
                                    ? (ObjectVariants) new OV_int(int.Parse(value))
                                    : (ObjectVariants) new OV_string(value));
                        }
                        return triple;
                    }));
                return seq;
            }));
            File.WriteAllText("../../../Databases/string based/photoPersons.ttl", store.ToTurtle());
        }

        public static int Millions = 1;
    }
}