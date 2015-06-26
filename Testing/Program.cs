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

namespace TestingNs
{
    class Program
    {
        static void Main(string[] args)
        {
          //  var Store = new StoreCascadingInt("../../../Databases/int based/");
          ////  Store.ReloadFrom(Config.Source_data_folder_path + "1.ttl");
          //  Store.Start();

          //  for (int i = 0; i < 12; i++)
          //  {
          //      SparqlTesting.OneParametrized(Store, i + 1, 100);
          //  }
         TestExamples();
             //  SparqlTesting.RunBerlinsWithConstants();
        }

        private static void TestExamples()
        {
            DirectoryInfo examplesRoot = new DirectoryInfo(@"..\..\examples");
            foreach (var exampleDir in examplesRoot.GetDirectories().Skip(0))
                //  var exampleDir = new DirectoryInfo(@"..\..\examples\bsbm");
            {
                Console.WriteLine("example: " + exampleDir.Name);
                //if (exampleDir.Name != @"13.2.1 Specifying the Default Graph"
                //    //&& rqQueryFile.FullName != @"C:\Users\Admin\Source\Repos\SparqlWpf\UnitTestDotnetrdf_test\examples\insert where\query2.rq"
                //  ) continue;
                //var nameGraphsDir = new DirectoryInfo(Path.Combine(exampleDir.FullName, "named graphs"));
                //if (nameGraphsDir.Exists) continue;
                foreach (var ttlDatabase in exampleDir.GetFiles("*.ttl"))
                {
                    var store = new StoreCascadingInt(exampleDir.FullName + "/tmp");
                    store.ClearAll();
                    //using (StreamReader reader = new StreamReader(ttlDatabase.FullName))
                    store.ReloadFrom(ttlDatabase.FullName);
                //  store.Start();
                  var nameGraphsDir = new DirectoryInfo(Path.Combine(exampleDir.FullName, "named graphs"));
                  if (nameGraphsDir.Exists)
                      foreach (var namedGraphFile in nameGraphsDir.GetFiles())
                      {
                          IGraph graph;
                          using (StreamReader reader = new StreamReader(namedGraphFile.FullName))
                          {
                              var readLine = reader.ReadLine();
                              if (readLine == null) continue;
                              var headComment = readLine.Trim();
                              if (!headComment.StartsWith("#")) continue;
                              headComment = headComment.Substring(1);
                              //Uri uri;
                              //if (!Uri.TryCreate(headComment, UriKind.Absolute, out uri)) continue;Prologue.SplitUri(uri.AbsoluteUri).FullName
                              graph = store.NamedGraphs.CreateGraph(headComment);

                          }
                          graph.FromTurtle(namedGraphFile.FullName);
                      }

                    foreach (var rqQueryFile in exampleDir.GetFiles("*.rq"))
                    {

                        Console.WriteLine("query file: " + rqQueryFile);
                        var outputFile = rqQueryFile.FullName + " results of run.txt";
                        SparqlResultSet sparqlResultSet = null;
                        //  try
                        var query = rqQueryFile.OpenText().ReadToEnd();

                        SparqlQuery sparqlQuery = null;
                        {
                            //Perfomance.ComputeTime(() =>
                            {
                                sparqlQuery = SparqlQueryParser.Parse(store, query);
                            } //, exampleDir.Name+" "+rqQueryFile.Name+" parse ", true);

                            if (sparqlQuery != null)
                                // Perfomance.ComputeTime(() =>
                            {
                                sparqlResultSet = sparqlQuery.Run();
                                File.WriteAllText(outputFile, sparqlResultSet.ToJson().ToString());
                            } //, exampleDir.Name + " " + rqQueryFile.Name + " run ", true);
                            //    Assert.AreEqual(File.ReadAllText(rqQueryFile.FullName + " expected results.txt"),
                            //      File.ReadAllText(outputFile));
                        }
                        //  catch (Exception e)
                        {
                            // Assert.(e.Message);
                        }
                    }
                }
            }
        }

        private static
           void MainPersons(string[] args)
        {
            TestingPhotoPersons.Npersons = 40*1000;
            string path = "../../../Databases/int based/" + TestingPhotoPersons.Npersons/1000+"/";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            StoreCascadingInt store = new StoreCascadingInt(path);
            using (StreamWriter perfomance = new StreamWriter("../../Perfomance.txt"))
                perfomance.WriteLine(TestingPhotoPersons.Npersons);
            if(true)
            Performance.ComputeTime(() => Reload(store), "load " + TestingPhotoPersons.Npersons + " ", true);
            TestingPhotoPersons.Run((q) =>
            {
                var sparqlQuery = SparqlQueryParser.Parse(store, q);
                if (sparqlQuery.Type == SparqlQueryTypeEnum.Ask)
                {var b=   sparqlQuery.Run().AnyResult;}
                else 
                    sparqlQuery.Run().Results.Count();
            });
        }

        private static void Reload(StoreCascadingInt store)
        {
            store.Build(TestingPhotoPersons.data.Generate().SelectMany(ele =>
            {
                string id = ele.Name + ele.Attribute("id").Value;
                var seq = Enumerable.Repeat(
                    new TripleStrOV(id, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type",
                        new OV_iri(ele.Name.LocalName)), 1)
                    .Concat(ele.Elements().Select(subele =>
                    {
                        XAttribute ratt = subele.Attribute("ref");
                        TripleStrOV triple = null;
                        if (ratt != null)
                        {
                            string r = (subele.Name == "reflected" ? "person" : "photo_doc") +
                                       ratt.Value;
                            triple = new TripleStrOV(id, subele.Name.LocalName, new OV_iri(r));
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
                            triple = new TripleStrOV(id, subele.Name.LocalName,
                                possiblenumber
                                    ? (ObjectVariants) new OV_int(int.Parse(value))
                                    : (ObjectVariants) new OV_string(value));
                        }
                        return triple;
                    }));
                return seq;
            }));
          //  File.WriteAllText("../../../Databases/string based/photoPersons.ttl", store.ToTurtle());
        }

        public static int Millions = 1;
    }
}