using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RDFCommon;
using RDFTripleStore.OVns;
using RDFTripleStore.parsers.RDFTurtle;
using SparqlParseRun.SparqlClasses;

namespace RDFTripleStore
{
    /// <summary>
    /// Расширения для интерфейсов    <see cref="IGraph<string,string,ObjectVariants>"/> и <see cref="IGraph<Ts,Tp,To>"/>
    /// </summary>
    public static class Testing
    {
        /// <summary>
        /// запускает Build и  замеряет время.
        /// </summary>
        /// <param name="graph"> тестируемый граф должен реализовать интерфейс <see cref="IGraph<string,string,ObjectVariants>"/></param>
        /// <param name="millions">в данных пока предполагаются варианты: 1, 10, 100, 1000</param>
        public static void TestReadTtl(this IGraph<Triple<string, string, ObjectVariants>> graph, int millions)
        {
            Perfomance.ComputeTime(() =>
                graph.Build(
                    ReadTripleStringsFromTurtle.LoadGraph(
                        Config.Source_data_folder_path + millions + ".ttl")), "build " + millions + ".ttl ", true);
        }          

        /// <summary>
        /// запускает Build и  замеряет время.
        /// </summary>
        /// <param name="graph"> тестируемый граф должен реализовать интерфейс <seealso cref="IGraph<string,string,ObjectVariants>"/></param>
        /// <param name="turtleFileName"> путь к внешнему файлу ttl</param>
        public static void TestReadTtl(this IGraph<Triple<string, string, ObjectVariants>> graph, string turtleFileName)
        {
            Perfomance.ComputeTime(() =>
                graph.Build(
                    ReadTripleStringsFromTurtle.LoadGraph(turtleFileName)),
                "build " + turtleFileName + " ", true);
        }

        /// <summary>
        /// запускает Build и  замеряет время.
        ///    использует <see cref="TripleGeneratorBufferedParallel"/>
        /// 
        /// </summary>
        /// <param name="graph"> тестируемый граф должен реализовать интерфейс <see cref="IGraph<string,string,ObjectVariants>"/></param>
        /// <param name="millions">в данных пока предполагаются варианты: 1, 10, 100, 1000</param>
        public static void TestReadTtl_Cocor(this IGraph<Triple<string, string, ObjectVariants>> graph, int millions)
        {

            Perfomance.ComputeTime(() =>
            {
                var generator = new TripleGeneratorBufferedParallel(Config.Source_data_folder_path + millions + ".ttl", "g");
                graph.Build(generator);
            },
                "build " + millions + ".ttl ", true);
        }

        /// <summary>
        /// Я пока не смог организовать выдачу потока триплетов из парсера.
        /// Парсер выполняет делегат для каждого триплета.
        /// Этот метод группирует триплеты в буфер и выполняет указанный делегат над буфером триплетов.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="foreachBuffer">делегат выполняемый над буффером, когда тот заполнен, после чего очищает его.</param>
        /// <param name="bufferlength">максимальная длина. Чем больше, тем реже будет выполняется делегат.</param>
        private static void ForeachBuffer(Parser parser, Action<List<Triple<string,string, ObjectVariants>>> foreachBuffer, int bufferlength=1000)
        {
                            var buffer=new List<Triple<string, string, ObjectVariants>>(bufferlength);
         parser.ft = (s, s1, arg3) =>
            {
                buffer.Add(new Triple<string, string, ObjectVariants>(s, s1, arg3));
                if (buffer.Count == bufferlength)
                {
                    foreachBuffer(buffer);
                    //buffer=new List<Triple<string, string, ObjectVariants>>();
                    buffer.Clear();
                }
            };
               parser.Parse();
        }

        /// <summary>
        /// запускает Build и  замеряет время.
        /// </summary>
        /// <param name="graph"> тестируемый граф должен реализовать интерфейс <seealso cref="IGraph<string,string,ObjectVariants>"/></param>
        /// <param name="turtleFileName"> путь к внешнему файлу ttl</param>
        public static void TestReadTtl_Cocor(this IGraph<Triple<string, string, ObjectVariants>> graph, string turtleFileName)
        {
            Perfomance.ComputeTime(() =>
                graph.Build(new TripleGeneratorBufferedParallel(turtleFileName,"g")),
                "build " + turtleFileName + " ", true);
        }


        /// <summary>
        /// Замеряет время:
        ///  1) поток всех триплетов ограничен 100 триплетами;
        ///  2) заменяет субъекты объектами, если они uri и проводит поиск; 
        ///  3) поиск только по предикаьам взятым из первых 100 триплетов; 
        /// </summary>
        /// <typeparam name="Ts"></typeparam>
        /// <typeparam name="Tp"></typeparam>
        /// <typeparam name="To"></typeparam>
        /// <param name="graph"></param>
        public static void TestSearch(this IGraph<Triple<string, string, ObjectVariants>> graph)
        {
            var all = graph.Search();
            Triple<string, string, ObjectVariants>[] ts100 = null;
            Perfomance.ComputeTime(() =>
            {
                ts100 = all.Take(100).ToArray();
            }, "get first's 100 triples ", true);
            Perfomance.ComputeTime(() =>
            {
                foreach (var t in ts100)
                {
                    if (t.Object.Variant == ObjectVariantEnum.Iri)
                        graph.Search(((OV_iri) t.Object).UriString).ToArray();

                }
            }, "search by object as subject from first's 100 triples ", true);
            Perfomance.ComputeTime(() =>
            {
                foreach (var t in ts100)
                {
                    graph.Search(predicate: t.Predicate).ToArray();

                }
            }, "search by predicate from first's 100 triples ", true);
            Perfomance.ComputeTime(() =>
            {
                foreach (var t in ts100)
                {
                    var triples = graph.Search(t.Subject, t.Predicate, t.Object).ToArray();
                    if (!triples.All(
                        tt => tt.Subject == t.Subject && tt.Predicate == t.Predicate && tt.Object == t.Object))
                        throw new Exception();
                }
            }, "search by subject predicate and object from first's 100 triples, compare correctness ", true);
            
        }

        public static void TestSparqlStore(int millions)
        {
            SparqlStore sparqlStore = new SparqlStore("../../../Databases/");
            Perfomance.ComputeTime(() =>
            {
               // sparqlStore.ReloadFrom(Config.Source_data_folder_path + millions + ".ttl");
            }, "build " + millions + ".ttl ");
         //   Console.WriteLine(sparqlStore.GetTriplesWithSubject(sparqlStore.NodeGenerator.CreateUriNode("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromVendor1/")));
            Perfomance.ComputeTime(() =>
            {
                Console.WriteLine(
                   sparqlStore.ParseAndRun(
                       @"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX bsbm: <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/>
PREFIX dataFromProducer1: <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer1/> 

SELECT DISTINCT ?prodFeature
WHERE { 
	?prodFeature ?pp <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromRatingSite1/Reviewer1> .	
     }
").ToJson());
                Console.WriteLine("___________________________________________________________________________________");
                Console.WriteLine(
                    sparqlStore.ParseAndRun(
                        @"PREFIX dataFromVendor1: <http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromVendor1/>
SELECT ?property ?hasValue ?isValueOf
WHERE {
  { dataFromVendor1:Offer1 ?property ?hasValue }
  UNION
  { ?isValueOf ?property dataFromVendor1:Offer1 }
}
").ToJson());
            }, "run simple" + millions + ".ttl ");
        }

        public static void BSBm(int millions, bool load)
        {
            SparqlStore sparqlStore = new SparqlStore("../../../Databases/");
                     if(load)
                         sparqlStore.ReloadFrom(Config.Source_data_folder_path + millions + ".ttl");
            RunBerlinsWithConstants(sparqlStore, millions);
        }
        public static void RunBerlinsParameters(SparqlStore ts, int millions)
        {

            Console.WriteLine("antrl parametered");
            var fileInfos = new[]
                {
                    @"..\..\examples\bsbm\queries\parameters\1.rq"     ,  
                    @"..\..\examples\bsbm\queries\parameters\2.rq"   ,
                    @"..\..\examples\bsbm\queries\parameters\3.rq"    , 
                    @"..\..\examples\bsbm\queries\parameters\4.rq",
                    @"..\..\examples\bsbm\queries\parameters\5.rq" ,     
                    @"..\..\examples\bsbm\queries\parameters\6.rq" ,
                    @"..\..\examples\bsbm\queries\parameters\7.rq"  ,
                    @"..\..\examples\bsbm\queries\parameters\8.rq"  ,
                    @"..\..\examples\bsbm\queries\parameters\9.rq",
                    @"..\..\examples\bsbm\queries\parameters\10.rq"  ,
                    @"..\..\examples\bsbm\queries\parameters\11.rq",
                    @"..\..\examples\bsbm\queries\parameters\12.rq"  ,
                }
                .Select(s => new FileInfo(s))
                .ToArray();
            var paramvaluesFilePath = string.Format(@"..\..\examples\bsbm\queries\parameters\param values for{0} m.txt", millions);
            //            using (StreamWriter streamQueryParameters = new StreamWriter(paramvaluesFilePath))
            //                for (int j = 0; j < 1000; j++)
            //                    foreach (var file in fileInfos.Select(info => File.ReadAllText(info.FullName)))
            //                        QueryWriteParameters(file, streamQueryParameters, ts);
            //return;

            using (StreamReader streamQueryParameters = new StreamReader(paramvaluesFilePath))
            {
                for (int j = 0; j < 500; j++)
                    fileInfos.Select(file => QueryReadParameters(File.ReadAllText(file.FullName),
                        streamQueryParameters))
                        // .Select(ts.ParseRunSparql)
                        .ToArray();

                SubTestRun(ts, fileInfos, streamQueryParameters, 500, millions);
            }
        }

        private static void SubTestRun(SparqlStore ts, FileInfo[] fileInfos, StreamReader streamQueryParameters, int i1, int millions)
        {
            int i;
            long[] results = new long[12];
            double[] minimums = Enumerable.Repeat(double.MaxValue, 12).ToArray();
            double[] maximums = new double[12];
            double maxMemoryUsage = 0;
            long[] totalparseMS = new long[12];
            long[] totalrun = new long[12];
            for (int j = 0; j < i1; j++)
            {
                i = 0;

                foreach (var file in fileInfos)
                {
                    var readAllText = File.ReadAllText(file.FullName);
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
                    results[i++] += totalMilliseconds;
                    //  File.WriteAllText(Path.ChangeExtension(file.FullName, ".txt"), resultString);
                    //.Save(Path.ChangeExtension(file.FullName,".xml"));
                }
            }
            using (StreamWriter r = new StreamWriter(@"..\..\output.txt", true))
            {
                r.WriteLine("milions " + millions);
                r.WriteLine("max memory usage " + maxMemoryUsage);
                r.WriteLine("average " + string.Join(", ", results.Select(l => l == 0 ? "inf" : (500 * 1000 / l).ToString())));
                r.WriteLine("minimums " + string.Join(", ", minimums));
                r.WriteLine("maximums " + string.Join(", ", maximums));
                r.WriteLine("total parse " + string.Join(", ", totalparseMS));
                r.WriteLine("total run " + string.Join(", ", totalrun));
                //    r.WriteLine("countCodingUsages {0} totalMillisecondsCodingUsages {1}", TripleInt.EntitiesCodeCache.Count, TripleInt.totalMilisecondsCodingUsages);

                //r.WriteLine("EWT average search" + EntitiesMemoryHashTable.total / EntitiesMemoryHashTable.count);
                //r.WriteLine("EWT average range" + EntitiesMemoryHashTable.totalRange / EntitiesMemoryHashTable.count);  
            }
        }

        private static void RunBerlinsWithConstants(SparqlStore ts, int millions)
        {
            long[] results = new long[12];
            Console.WriteLine("antrl with constants");
            int i = 0;
            var fileInfos = new[]
                {
                    @"..\..\examples\bsbm\queries\with constants\1.rq"     ,  
                    @"..\..\examples\bsbm\queries\with constants\2.rq"   ,
                    @"..\..\examples\bsbm\queries\with constants\3.rq"    , 
                    @"..\..\examples\bsbm\queries\with constants\4.rq",
                    @"..\..\examples\bsbm\queries\with constants\5.rq" ,     
                    @"..\..\examples\bsbm\queries\with constants\6.rq" ,
                    @"..\..\examples\bsbm\queries\with constants\7.rq"  ,
                    @"..\..\examples\bsbm\queries\with constants\8.rq"  ,
                    @"..\..\examples\bsbm\queries\with constants\9.rq",
                    @"..\..\examples\bsbm\queries\with constants\10.rq"  ,
                    @"..\..\examples\bsbm\queries\with constants\11.rq",
                    @"..\..\examples\bsbm\queries\with constants\12.rq"  ,
                }
                .Select(s => new FileInfo(s))
                .ToArray();
            for (int j = 0; j < 0; j++)
            {                    
                foreach (var file in fileInfos)
                {
                    var readAllText = File.ReadAllText(file.FullName);

                    //   var st = DateTime.Now;
                    //  var q = new Query(ts);
                    //  q.Parse(readAllText, ts);
                    //     var resultString = q.Run();
                    //var totalMilliseconds = (long)(DateTime.Now - st).TotalMilliseconds;
                    // results[i++] += totalMilliseconds;
                    //   File.WriteAllText(Path.ChangeExtension(file.FullName, ".txt"), resultString);
                    //.Save(Path.ChangeExtension(file.FullName,".xml"));
                }
            }
            for (int j = 0; j < 1; j++)
            {
                i = 0;
                foreach (var file in fileInfos)
                {
                    var readAllText = File.ReadAllText(file.FullName);
                    var st = DateTime.Now;

                    Console.WriteLine(file.Name);
                 var sparqlResultSet =   ts.ParseAndRun(readAllText);
                    

                    var totalMilliseconds = (DateTime.Now - st).Ticks / 10000L;
                    results[i++] += totalMilliseconds;
                    File.WriteAllText(Path.ChangeExtension(file.FullName, ".json"), sparqlResultSet.ToJson());
                    //.Save(Path.ChangeExtension(file.FullName,".json"));
                }
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
                parameteredQuery = parameteredQuery.Replace("%ProductType%", input.ReadLine());
            if (parameteredQuery.Contains("%ProductFeature1%"))
                parameteredQuery = parameteredQuery.Replace("%ProductFeature1%", input.ReadLine());
            if (parameteredQuery.Contains("%ProductFeature2%"))
                parameteredQuery = parameteredQuery.Replace("%ProductFeature2%", input.ReadLine());
            if (parameteredQuery.Contains("%ProductFeature3%"))
                parameteredQuery = parameteredQuery.Replace("%ProductFeature3%", input.ReadLine());
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
    
    }
}
