using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Polar.Data;
using PolarDB;
using RDFCommon.OVns;
using SparqlParseRun;
using SparqlParseRun.SparqlClasses;

namespace TestingNs
{
   public class TestingPhotoPersons
   {
       private static int npersons = 400 * 1000;
       public TestDataGenerator data = new TestDataGenerator(npersons, 2378459);
       private Random rnd=new Random();

       public string QDescribePerson()
       {
           return @"DESCRIBE person" + rnd.Next(npersons - 1);
       }
       public string QGetPerson3123Info()
       {
           return @"SELECT ?property ?value WHERE {{ <person3123> ?property ?value }}";
       }
       public string QGetPersonInfo()
       {
           return string.Format(@"SELECT ?property ?value WHERE {{ <person{0}> ?property ?value }}", rnd.Next(npersons - 1));
       }
       public string QGetPersonName()
       {
           return string.Format(@"SELECT ?name WHERE {{ <person{0}> <name> ?name }}", rnd.Next(npersons - 1));
       }
       public string QContainsPersonType()
       {
           return string.Format(@"ASK WHERE {{ <person{0}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <person> }}", rnd.Next(npersons - 1));
       }
       public string QGetPersonPhotoNames()
       {
           return string.Format(@"SELECT ?name WHERE {{ ?reflection <reflected> <person{0}>   .
                                                  ?reflection  <in_doc>   ?doc                 .
                                                  ?doc <name> ?name  }}", rnd.Next(npersons - 1));
       }
       public static TestingPhotoPersons testing = new TestingPhotoPersons();
       private static int runQueryReturnCount;

       public void Run(Func<string, object> runQueryReturnCount)
       {     
           //SparqlQuery sparqlQuery = SparqlQueryParser.Parse(store, QGetPerson3123Info());
           //Console.WriteLine(sparqlQuery.Run().ToJson());  
                      TestingPhotoPersons.runQueryReturnCount = 0;
           Perfomance.ComputeTime(() =>
           {
               for (int i = 0; i < 1000; i++)
               {
                  TestingPhotoPersons.runQueryReturnCount+= (int) runQueryReturnCount(QGetPersonInfo());
               }
           }, "1000 sPO ok. duration=", true);

           TestingPhotoPersons.runQueryReturnCount = 0;
           Perfomance.ComputeTime(() =>
           {
               for (int i = 0; i < 1000; i++)
               {
                   TestingPhotoPersons.runQueryReturnCount += (int) runQueryReturnCount(QGetPersonName());
               }
           }, string.Format("1000 spO ok cnt={0}. duration=", TestingPhotoPersons.runQueryReturnCount), true);

           Console.WriteLine("1000 spO ok cnt={0}. duration=", TestingPhotoPersons.runQueryReturnCount);
           Perfomance.ComputeTime(() =>
           {                                 
               for (int i = 0; i < 1000; i++)
               {
                   bool exists= (bool) runQueryReturnCount(QContainsPersonType());
                   if (!exists) throw new Exception("438723");
               }
           }, "1000 spo ok duration=", true);
           TestingPhotoPersons.runQueryReturnCount = 0;

           Perfomance.ComputeTime(() =>
           {
               for (int i = 0; i < 100; i++)
               {
                   TestingPhotoPersons.runQueryReturnCount = (int) runQueryReturnCount(QGetPersonPhotoNames());
               }
           }, string.Format("100 portraits ok cnt={0}. duration=", TestingPhotoPersons.runQueryReturnCount), true);
           Console.WriteLine("100 portraits ok cnt={0}. duration=", TestingPhotoPersons.runQueryReturnCount);
       }
    }
}
