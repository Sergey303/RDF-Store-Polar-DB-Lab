using System;
using System.IO;
using System.Linq;
using RDFCommon;
using SparqlParseRun;
using SparqlParseRun.SparqlClasses.Query.Result;
using RDFTripleStore;

namespace TestingNs
{
    class SparqlTesting
    {
        public static void TestSparqlStore(int millions)
        {
            SparqlStore2 sparqlStore = new SparqlStore2("../../../Databases/");
            Perfomance.ComputeTime(() =>
            {
                sparqlStore.ReloadFrom(Config.Source_data_folder_path + millions + ".ttl");
            }, "build " + millions + ".ttl ");
            //   Console.WriteLine(sparqlStore.GetTriplesWithSubject(sparqlStore.NodeGenerator.CreateUriNode("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromVendor1/")));
            Perfomance.ComputeTime(() =>
            {
                Console.WriteLine(
                    (string)sparqlStore.ParseAndRun(
   @" PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX bsbm: <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/>
PREFIX dataFromProducer1: <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer1/> 

SELECT DISTINCT ?product
WHERE { 
	dataFromProducer1:Product12 bsbm:productFeature ?prodFeature .
	?product bsbm:productFeature ?prodFeature .
    FILTER (dataFromProducer1:Product12 != ?product)	
?product rdfs:label ?productLabel .
	dataFromProducer1:Product12 bsbm:productPropertyNumeric1 ?origProperty1 .
	?product bsbm:productPropertyNumeric1 ?simProperty1 .
	FILTER (?simProperty1 < (?origProperty1 + 120) && ?simProperty1 > (?origProperty1 - 120))
     }
").ToJson());
              
            }, "run simple" + millions + ".ttl ");
        }

        public static void TestQuery(string queryString, bool load, int millions)
        {
            SparqlStore2 sparqlStore = new SparqlStore2("../../../Databases/");
            if (load)
            Perfomance.ComputeTime(() =>
            {
                sparqlStore.ReloadFrom(Config.Source_data_folder_path + millions + ".ttl");
            }, "build " + millions + ".ttl ");

            
            

            SparqlQuery query = null;
            Perfomance.ComputeTime(() => query = sparqlStore.Parse(queryString), "parse ");
            SparqlResultSet results = null;
            Perfomance.ComputeTime(() => results = query.Run(sparqlStore), "run ");
            

            Console.WriteLine("count "+results.Results.Count());
        }

        public static void BSBm(int millions, bool load)
        {
            SparqlStore2 sparqlStore = new SparqlStore2("../../../Databases/");
            if (load)
                sparqlStore.ReloadFrom(Config.Source_data_folder_path + millions + ".ttl");
            //   RunBerlinsWithConstants(sparqlStore, millions);
            RunBerlinsParameters(sparqlStore, millions);
        }

        public static void RunBerlinsParameters(SparqlStore2 ts, int millions)
        {
           ts.Warmup();
            Console.WriteLine("bsbm parametered");
            var paramvaluesFilePath = string.Format(@"..\..\examples\bsbm\queries\parameters\param values for{0} m.txt", millions);
            //            using (StreamWriter streamQueryParameters = new StreamWriter(paramvaluesFilePath))
            //                for (int j = 0; j < 1000; j++)
            //                    foreach (var file in fileInfos.Select(info => File.ReadAllText(info.FullName)))
            //                        QueryWriteParameters(file, streamQueryParameters, ts);
            //return;

            using (StreamReader streamQueryParameters = new StreamReader(paramvaluesFilePath))
            {
                for (int j = 0; j < 500; j++)
                    for (int i = 1; i < 13; i++)
                        QueryReadParameters(File.ReadAllText(string.Format(@"..\..\examples\bsbm\queries\parameters\{0}.rq", i)),
                            streamQueryParameters);

                SubTestRun(ts, streamQueryParameters, 500, millions);
            }
        }

        private static void SubTestRun(SparqlStore2 ts, StreamReader streamQueryParameters, int i1, int millions)
        {
            long[] results = new long[12];
            double[] minimums = Enumerable.Repeat(double.MaxValue, 12).ToArray();
            double[] maximums = new double[12];
            double maxMemoryUsage = 0;
            long[] totalparseMS = new long[12];
            long[] totalrun = new long[12];
            for (int j = 0; j < i1; j++)
            {
                for (int i = 0; i < 12; i++)
                {
                    string file = string.Format(@"..\..\examples\bsbm\queries\parameters\{0}.rq", i+1);
                    var readAllText = File.ReadAllText(file);
                    readAllText = QueryReadParameters(readAllText, streamQueryParameters);

                    var st = DateTime.Now;
                    var sparqlQuery = ts.Parse(readAllText);

                    totalparseMS[i] += (DateTime.Now - st).Ticks / 10000L;
                    var st1 = DateTime.Now;
                    var sparqlResultSet = sparqlQuery.Run(ts);
                    totalrun[i] += (DateTime.Now - st1).Ticks / 10000L;
                    var totalMilliseconds = (DateTime.Now - st).Ticks / 10000L;

                    var memoryUsage = GC.GetTotalMemory(false);
                    if (memoryUsage > maxMemoryUsage)
                        maxMemoryUsage = memoryUsage;
                    if (minimums[i] > totalMilliseconds)
                        minimums[i] = totalMilliseconds;
                    if (maximums[i] < totalMilliseconds)
                        maximums[i] = totalMilliseconds;
                    results[i] += totalMilliseconds;
                    //  File.WriteAllText(Path.ChangeExtension(file.FullName, ".txt"), resultString);
                    //.Save(Path.ChangeExtension(file.FullName,".xml"));
                }
            }
            using (StreamWriter r = new StreamWriter(@"..\..\output.txt", true))
            {
                r.WriteLine("milions " + millions);
                r.WriteLine("date time " + DateTime.Now);
                r.WriteLine("max memory usage " + maxMemoryUsage);
                r.WriteLine("average " + string.Join(", ", results.Select(l => l == 0 ? "inf" : (500 * 1000 / l).ToString())));
                r.WriteLine("minimums " + string.Join(", ", minimums));
                r.WriteLine("maximums " + string.Join(", ", maximums));
                r.WriteLine("total parse " + string.Join(", ", totalparseMS));
                r.WriteLine("total run " + string.Join(", ", totalrun));
                r.WriteLine("total " + totalparseMS.Sum()+totalrun.Sum());
                //    r.WriteLine("countCodingUsages {0} totalMillisecondsCodingUsages {1}", TripleInt.EntitiesCodeCache.Count, TripleInt.totalMilisecondsCodingUsages);

                //r.WriteLine("EWT average search" + EntitiesMemoryHashTable.total / EntitiesMemoryHashTable.count);
                //r.WriteLine("EWT average range" + EntitiesMemoryHashTable.totalRange / EntitiesMemoryHashTable.count);  
            }
        }

        private static void RunBerlinsWithConstants(SparqlStore2 ts, int millions)
        {
            long[] results = new long[12];
            Console.WriteLine("antrl with constants");
            for (int i = 1; i < 13; i++)
            {
                string file = string.Format(@"..\..\examples\bsbm\queries\with constants\{0}.rq", i);
                var readAllText = File.ReadAllText(file);
                var st = DateTime.Now;

                Console.WriteLine(i);
                var sparqlResultSet = ts.ParseAndRun(readAllText);

                var totalMilliseconds = (DateTime.Now - st).Ticks / 10000L;
                results[i - 1] += totalMilliseconds;
                File.WriteAllText(Path.ChangeExtension(file, ".json"), sparqlResultSet.ToJson());
                //.Save(Path.ChangeExtension(file.FullName,".json"));
            }
            Console.WriteLine(string.Join(", ", results));
            using (StreamWriter r = new StreamWriter(@"..\..\output.txt", true))
            {
                r.WriteLine("milions " + millions);
                //   r.WriteLine("countCodingUsages {0} totalMillisecondsCodingUsages {1}", TripleInt.EntitiesCodeCache.Count, TripleInt.totalMilisecondsCodingUsages);
            }

        }

        private static string QueryReadParameters(string parameteredQuery, StreamReader input)
        {
            if (parameteredQuery.Contains("%ProductType%"))
                parameteredQuery = parameteredQuery.Replace("%ProductType%", Read(input.ReadLine()) + ">");
            if (parameteredQuery.Contains("%ProductFeature1%"))
                parameteredQuery = parameteredQuery.Replace("%ProductFeature1%", Read(input.ReadLine()));
            if (parameteredQuery.Contains("%ProductFeature2%"))
                parameteredQuery = parameteredQuery.Replace("%ProductFeature2%", Read(input.ReadLine()));
            if (parameteredQuery.Contains("%ProductFeature3%"))
                parameteredQuery = parameteredQuery.Replace("%ProductFeature3%", Read(input.ReadLine()));
            if (parameteredQuery.Contains("%x%"))
                parameteredQuery = parameteredQuery.Replace("%x%", input.ReadLine());
            if (parameteredQuery.Contains("%y%"))
                parameteredQuery = parameteredQuery.Replace("%y%", input.ReadLine());
            if (parameteredQuery.Contains("%ProductXYZ%"))
                parameteredQuery = parameteredQuery.Replace("%ProductXYZ%", "<" + input.ReadLine() + ">");
            if (parameteredQuery.Contains("%word1%"))
                parameteredQuery = parameteredQuery.Replace("%word1%", input.ReadLine());
            if (parameteredQuery.Contains("%currentDate%"))
                parameteredQuery = parameteredQuery.Replace("%currentDate%", input.ReadLine());
            if (parameteredQuery.Contains("%ReviewXYZ%"))
                parameteredQuery = parameteredQuery.Replace("%ReviewXYZ%", "<" + input.ReadLine() + ">");
            if (parameteredQuery.Contains("%OfferXYZ%"))
                parameteredQuery = parameteredQuery.Replace("%OfferXYZ%", "<" + input.ReadLine() + ">");
            return parameteredQuery;
        }

        private static string Read(string readLine)
        {
            if (readLine.StartsWith("http://"))
                return "<" + readLine + ">";
            UriPrefixed splitPrefixed = Prologue.SplitPrefixed(readLine);
            if (splitPrefixed.Prefix == "bsbm-inst:")
                return "<http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/>";
            return readLine;
        }

        public static void TestExamples()
        {
            DirectoryInfo examplesRoot = new DirectoryInfo(@"..\..\examples");
            var store = new SparqlStore2("../../../Databases/");
            foreach (var exampleDir in examplesRoot.GetDirectories())
                //  var exampleDir = new DirectoryInfo(@"..\..\examples\bsbm");
            {
                Console.WriteLine("example: " + exampleDir.Name);

                var ttlDatabase = exampleDir.GetFiles("*.ttl").FirstOrDefault();
                if (ttlDatabase == null) continue;
                using (StreamReader reader = new StreamReader(ttlDatabase.FullName))
                    store.ReloadFrom(reader.ReadToEnd());//store.ReloadFrom(reader.BaseStream);

                var nameGraphsDir = new DirectoryInfo(Path.Combine(exampleDir.FullName, "named graphs"));
                if (nameGraphsDir.Exists)
                {
                    if (store.NamedGraphs == null) Console.WriteLine("named graphs disabled");
                    else
                    {
                        foreach (var namedGraphFile in nameGraphsDir.GetFiles())
                            using (StreamReader reader = new StreamReader(namedGraphFile.FullName))
                            {
                                var readLine = reader.ReadLine();
                                if (readLine == null) continue;
                                var headComment = readLine.Trim();
                                if (!headComment.StartsWith("#")) continue;
                                headComment = headComment.Substring(1);
                                Uri uri;
                                if (!Uri.TryCreate(headComment, UriKind.Absolute, out uri)) continue;
                                var graph =
                                    store.NamedGraphs.CreateGraph(
                                        store.NodeGenerator.CreateUriNode(Prologue.SplitUri(uri.AbsoluteUri)));
                                graph.FromTurtle(reader.ReadToEnd());
                            }
                        RunOneExample(exampleDir, store);
                    }
                }
                else RunOneExample(exampleDir, store);
                //  store.ClearAll();
            }
        }

        private static void RunOneExample(DirectoryInfo exampleDir, SparqlStore2 store)
        {
            foreach (var rqQueryFile in exampleDir.GetFiles("*.rq"))
            {
                Console.WriteLine("query file: " + rqQueryFile);
                SparqlResultSet sparqlResultSet = null;
                //  try
                var query = rqQueryFile.OpenText().ReadToEnd();

                SparqlQuery sparqlQuery = null;
                {
                    Perfomance.ComputeTime(() => { sparqlQuery = store.Parse(query); },
                        exampleDir.Name + " " + rqQueryFile.Name + " parse ", true);

                    if (sparqlQuery != null)
                        Perfomance.ComputeTime(() => { sparqlResultSet = sparqlQuery.Run(store); },
                            exampleDir.Name + " " + rqQueryFile.Name + " run ", true);
                    File.WriteAllText(rqQueryFile.FullName + " results of run.txt", sparqlResultSet.ToJson());
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