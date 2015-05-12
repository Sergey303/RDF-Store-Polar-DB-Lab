﻿namespace TestingNs
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
            

        }
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