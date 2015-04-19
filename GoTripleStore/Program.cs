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
            string path = "../../../Databases/";
            Console.WriteLine("Start GoTripleStore.");
            var query = ReadTripleStringsFromTurtle.LoadGraph(Config.Source_data_folder_path + "1.ttl");
            //Console.WriteLine(query.Count());
            GoGraphStringBased ggraph = new GoGraphStringBased(path);
            ggraph.Build(query);
        }
    }
}
