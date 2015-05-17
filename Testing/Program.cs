using System;
using System.IO;

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
            DirectoryInfo root = new DirectoryInfo("../../../Databases/string based/");
           for (int i = 1; i < 13; i++)
           //  int i = 9;
            {

                DirectoryInfo dirQ = new DirectoryInfo("../../../Databases/string based/" + i + "/");
                if(dirQ.Exists)
                dirQ.Delete(true);
                
                dirQ.Create();
                foreach (var fileInfo in root.GetFiles())
                    fileInfo.CopyTo(dirQ.FullName + "/" + fileInfo.Name);          

              //  using (var Store = new SecondStringSore(dirQ.FullName))

                {
                    var Store = new SecondStringSore(dirQ.FullName);
               //       Store.ReloadFrom(Config.Source_data_folder_path + Millions + ".ttl");

               //   SparqlTesting.CallsAnalyze(Store, 12, 100);
               //SparqlTesting.OneParametrized(Store, i, 100);
               //   Store.Warmup();
               //  SparqlTesting.CacheMeasureAllWithConstants<SecondStringSore>(Store);

               //  SparqlTesting.TestQuery(_queryString, false, 1);
               //   SparqlTesting._ts.ReloadFrom(Config.Source_data_folder_path + 1 + ".ttl");
               //      SparqlTesting.RunBerlinsWithConstants();
               //    SparqlTesting.RunTestParametred(1);
               //    SparqlTesting.RunBerlinsParameters(Store);
               //SparqlTesting.CreateParameters(5, 1000, 1);  
               //SparqlTesting.CacheMeasure();

                  SparqlTesting.InterpretMeas(Store, i);
           }
            }
        }

        public static int Millions = 1;
    }
}