﻿namespace TestingNs
{
    class Program
    {
        static void Main(string[] args)
        {
            //RamGraph graph=new RamGraph();
            //graph.TestReadTtl_Cocor(1);
            //graph.TestSearch();
        //     SparqlTesting.TestSparqlStore(1);

            //SparqlTesting.BSBm(1, false);

            //  Testing.TestExamples();


            ///dataFromProducer1:Product12 - конкретное значение параметра в запросе %productXYZ%
            SparqlTesting.TestQuery(@"
                                  PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
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
                                    ", false, 1);
        }
    }
}