namespace TestingNs
{
    class Program
    {
        static void Main(string[] args)
        {
            //RamGraph graph=new RamGraph();
            //graph.TestReadTtl_Cocor(1);
            //graph.TestSearch();
        
            //SparqlTesting.TestSparqlStore(1);

            //SparqlTesting.BSBm(1, false);
                                                                                 
            //  Testing.TestExamples();



            ///dataFromProducer1:Product12 - конкретное значение параметра в запросе %productXYZ%
          //  SparqlTesting.TestQuery(_queryString, false, 1);
         //   SparqlTesting.InterpretMeas(1, false);
         //   SparqlTesting._ts.ReloadFrom(Config.Source_data_folder_path + 1 + ".ttl");
            SparqlTesting.RunTestParametred(50, 100);
            //SparqlTesting.CreateParameters(5, 1000, 1);  


        }
    
    }
}