using System;
using System.Collections.Generic;
using System.Linq;
using RDFTripleStore;

namespace GoTripleStore
{
    public class Program
    {
        public static void Main()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            string path = "../../../Databases/";
            Console.WriteLine("Start GoTripleStore.");
            var query = ReadTripleStringsFromTurtle.LoadGraph(Config.Source_data_folder_path + "1.ttl");
            //Console.WriteLine(query.Count());
            GoGraphStringBased ggraph = new GoGraphStringBased(path);
            bool toload = true;
            if (toload)
            {
                sw.Restart();
                ggraph.Build(query);
                sw.Stop();
                Console.WriteLine("Load ok. duration={0}", sw.ElapsedMilliseconds);
            }
            var search_query = ggraph.Search("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer10/Product468",
                "http://www.w3.org/2000/01/rdf-schema#label", null);
            foreach (var t in search_query)
            {
                Console.WriteLine("Triple: {0} {1}", t.Subject, t.Predicate);
            }
            
        }
    }
}
