using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using RDFCommon;
using SparqlParseRun;
using SparqlParseRun.SparqlClasses;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace TestingNs
{
    class SparqlTesting
    {
  

        public static void TestQuery(string queryString, bool load, int millions)
        {
            SecondStringSore sparqlStore = new SecondStringSore("../../../Databases/");
            if (load)
            Perfomance.ComputeTime(() =>
            {
                sparqlStore.ReloadFrom(Config.Source_data_folder_path + millions + ".ttl");
            }, "build " + millions + ".ttl ");

            
            

            SparqlQuery query = null;
            Perfomance.ComputeTime(() => query = sparqlStore.Parse(queryString), "parse ");
            SparqlResultSet results = null;
            Perfomance.ComputeTime(() => results = query.Run(), "run ");

         

            Console.WriteLine("count "+results.Results.Count());
         //   Console.WriteLine("{0} ", results.ToJson());
        }

     
        public static void InterpretMeas(int millions, bool load)
        {
            SecondStringSore sparqlStore = new SecondStringSore("../../../Databases/");
            if (load)
                sparqlStore.ReloadFrom(Config.Source_data_folder_path + millions + ".ttl");
         // sparqlStore.TrainingMode = true;
   

            Console.WriteLine("bsbm with constants train");
            RunBerlinsParameters();

         // Console.WriteLine("history count " + sparqlStore.history.Count);
           // using (StreamWriter sr = new StreamWriter(@"..\..\output.txt", true))
             //   sr.WriteLine(sparqlStore.Output());
            //sparqlStore.TrainingMode = false;
            
            //Console.WriteLine("bsbm with constants from history");
            //RunBerlinsParameters(sparqlStore, millions);
            //     File.WriteAllText(string.Format(@"..\..\examples\bsbm\queries\with constants\{0}.json", i), json);




        }
    
        public static void RunBerlinsParameters()
        {
            StoreLauncher.Store.Warmup();
            Console.WriteLine("bsbm parametered");
            var paramvaluesFilePath = string.Format(@"..\..\examples\bsbm\queries\parameters\param values for{0} m.txt", StoreLauncher.Millions);
            //            using (StreamWriter streamQueryParameters = new StreamWriter(paramvaluesFilePath))
            //                for (int j = 0; j < 1000; j++)
            //                    foreach (var file in fileInfos.Select(info => File.ReadAllText(info.FullName)))
            //                        QueryWriteParameters(file, streamQueryParameters, ts);
            //return;

            using (StreamReader streamQueryParameters = new StreamReader(paramvaluesFilePath))
            {
                for (int j = 0; j < 500; j++)
                    for (int i = 1; i < 13; i++)
                        BSBmParams.QueryReadParameters(File.ReadAllText(string.Format(@"..\..\examples\bsbm\queries\parameters\{0}.rq", i)),
                            streamQueryParameters);

                SubTestRun(streamQueryParameters, 500);
            }
        }

        private static void SubTestRun( StreamReader streamQueryParameters, int i1)
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
                    readAllText = BSBmParams.QueryReadParameters(readAllText, streamQueryParameters);

                    var st = DateTime.Now;
                    var sparqlQuery = SparqlQueryParser.Parse(StoreLauncher.Store, readAllText);

                    totalparseMS[i] += (DateTime.Now - st).Ticks / 10000L;
                    var st1 = DateTime.Now;
                    var sparqlResultSet = sparqlQuery.Run().ToJson();
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
                r.WriteLine("milions " + StoreLauncher.Millions);
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

        private static void RunBerlinsWithConstants(SecondStringSore ts, int millions)
        {
            long[] results = new long[12];
            double[] minimums = Enumerable.Repeat(double.MaxValue, 12).ToArray();
            double[] maximums = new double[12];
            double maxMemoryUsage = 0;
            long[] totalparseMS = new long[12];
            long[] totalrun = new long[12];
            Console.WriteLine("antrl with constants");
            for (int i = 0; i < 12; i++)
            {
                string file = string.Format(@"..\..\examples\bsbm\queries\with constants\{0}.rq", i+1);
                var readAllText = File.ReadAllText(file);

                var st = DateTime.Now;
                var sparqlQuery = ts.Parse(readAllText);

                totalparseMS[i] += (DateTime.Now - st).Ticks / 10000L;
                var st1 = DateTime.Now;
                var sparqlResultSet = sparqlQuery.Run().ToJson();
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
                File.WriteAllText(Path.ChangeExtension(file, ".json"), sparqlResultSet);
                //.Save(Path.ChangeExtension(file.FullName,".json"));
            }
            Console.WriteLine(string.Join(", ", results));
          
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
                r.WriteLine("total " + totalparseMS.Sum() + totalrun.Sum());
                //    r.WriteLine("countCodingUsages {0} totalMillisecondsCodingUsages {1}", TripleInt.EntitiesCodeCache.Count, TripleInt.totalMilisecondsCodingUsages);

                //r.WriteLine("EWT average search" + EntitiesMemoryHashTable.total / EntitiesMemoryHashTable.count);
                //r.WriteLine("EWT average range" + EntitiesMemoryHashTable.totalRange / EntitiesMemoryHashTable.count);  
            }
        }


        public static void TestExamples()
        {
            DirectoryInfo examplesRoot = new DirectoryInfo(@"..\..\examples");
            var store = new SecondStringSore("../../../Databases/");
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
                                        Prologue.SplitUri(uri.AbsoluteUri).FullName);
                                graph.FromTurtle(reader.ReadToEnd());
                            }
                        RunOneExample(exampleDir, store);
                    }
                }
                else RunOneExample(exampleDir, store);
                //  store.ClearAll();
            }
        }

        private static void RunOneExample(DirectoryInfo exampleDir, SecondStringSore store)
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
                        Perfomance.ComputeTime(() => { sparqlResultSet = sparqlQuery.Run(); },
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

      

        public static void RunTestParametred(int iq, int count = 100)
        {
            var paramvaluesFilePath =
                string.Format(@"..\..\examples\bsbm\queries\parameters\param values for{0}m {1} query.txt", 1, iq);
            var qFile = 
                string.Format(@"..\..\examples\bsbm\queries\parameters\{0}.rq", iq);
            using (StreamReader streamParameters = new StreamReader(paramvaluesFilePath))
            using (StreamReader streamQuery = new StreamReader(qFile))
            {
                string qparams = streamQuery.ReadToEnd();
                Stopwatch timer = new Stopwatch();
                for (int j = 0; j < count; j++)
                {
                   string q = BSBmParams.QueryReadParameters(qparams, streamParameters);
                    timer.Start();
                    SparqlQueryParser.Parse(StoreLauncher.Store, q).Run().Results.ToArray();
                    timer.Stop();
                }

                using (StreamWriter r = new StreamWriter(@"..\..\output.txt", true))
                {
                    r.WriteLine();
                    r.WriteLine("one query {0}, {1} times", iq, count);
                    r.WriteLine("milions " + 1);
                    r.WriteLine("date time " + DateTime.Now);
                    r.WriteLine("total ms " + timer.ElapsedMilliseconds);
                    double l = ((double)timer.ElapsedMilliseconds)/count;
                    r.WriteLine("ms " + l);
                    
                    r.WriteLine("qps " + (int)(1000.0/l));
                    string q = BSBmParams.QueryReadParameters(qparams, streamParameters);
                    r.WriteLine("11 results count: {0}",
                        SparqlQueryParser.Parse(StoreLauncher.Store, q).Run().Results.Count());
                }
            }
        }
        private static string sq51 = @" PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX bsbm: <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/>
PREFIX dataFromProducer1: <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer1/> 

SELECT ?product
WHERE { 
	dataFromProducer1:Product12 bsbm:productFeature ?prodFeature .
	?product bsbm:productFeature ?prodFeature .
    FILTER (dataFromProducer1:Product12 != ?product)	
?product rdfs:label ?productLabel .
	dataFromProducer1:Product12 bsbm:productPropertyNumeric1 ?origProperty1 .
	?product bsbm:productPropertyNumeric1 ?simProperty1 .
#	FILTER (?simProperty1 < (?origProperty1 + 120) && ?simProperty1 > (?origProperty1 - 120))
     }
";

        private static string sq52 = @" PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
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
";
        private static string sq = @"SELECT  ?prodFeature
WHERE { 
 <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer1/Product12>  <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productFeature> ?prodFeature .
	?product <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productFeature> ?prodFeature .
}
";

        private static string sq5 = @"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX bsbm: <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/>
PREFIX dataFromProducer1: <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer1/> 

SELECT DISTINCT ?product ?productLabel
WHERE { 
	dataFromProducer1:Product12 bsbm:productFeature ?prodFeature .
	?product bsbm:productFeature ?prodFeature .
    FILTER (dataFromProducer1:Product12 != ?product)	
	?product rdfs:label ?productLabel .
	dataFromProducer1:Product12 bsbm:productPropertyNumeric1 ?origProperty1 .
	?product bsbm:productPropertyNumeric1 ?simProperty1 .
	FILTER (?simProperty1 < (?origProperty1 + 120) && ?simProperty1 > (?origProperty1 - 120))
	dataFromProducer1:Product12 bsbm:productPropertyNumeric2 ?origProperty2 .
	?product bsbm:productPropertyNumeric2 ?simProperty2 .
	FILTER (?simProperty2 < (?origProperty2 + 170) && ?simProperty2 > (?origProperty2 - 170))
}
ORDER BY ?productLabel
LIMIT 5
";
        private static readonly string _queryString = @"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX bsbm: <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/>

SELECT ?product ?label
WHERE {
    ?product rdf:type bsbm:Product .
	?product rdfs:label ?label .
	FILTER regex(?label, ""^s"")}";
    }
}