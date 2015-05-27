using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using RDFCommon.OVns;
using sema2012m;
using TestingNs;
using VirtuosoTest;
using Testing = TestingNs.Testing;

namespace VirtuosoBigData
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start");
         //   EngineVirtuoso engine = new EngineVirtuoso("HOST=localhost:1550;UID=dba;PWD=dba;Charset=UTF-8;Connection Timeout=500", "g");
          
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            AdapterVirtuoso engine = new AdapterVirtuoso("HOST=localhost:1550;UID=dba;PWD=dba;Charset=UTF-8", "g"); // "http://fogid.net/"


            
            if(false) Reload(engine);
            
         
            foreach (var row in engine.Query("sparql "+TestingPhotoPersons.QGetPerson3123Info()))
            {
                foreach (var o in row)
                {
                    Console.WriteLine(o);
                }
            }
          //  Console.WriteLine("Total duration=" + (DateTime.Now - tt0).Ticks / 10000L); tt0 = DateTime.Now;
            TestingPhotoPersons.Run(q =>
            {
                var qu = "sparql " + q;

                if (qu.Contains("SELECT "))
                {                                  
                    engine.Query(qu).Count();
                }
                else
                {
                    //IEnumerable<object[]> enumerable = engine.Query(qu);
                    var execute = engine.ExecuteScalar(qu);
                 
                }
            });
        }

        private static void Reload(AdapterVirtuoso store)
        {
            Performance.ComputeTime(() =>
            {
                store.Load(TestingPhotoPersons.data.Generate().SelectMany(ele =>
                {
                    string id = ele.Name + ele.Attribute("id").Value;
                    var seq = Enumerable.Repeat(
                        new Tuple<string, string, ObjectVariants>(id, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type",
                            new OV_iri(ele.Name.LocalName)), 1)
                        .Concat(ele.Elements().Select(subele =>
                        {
                            XAttribute ratt = subele.Attribute("ref");
                            Tuple<string, string, ObjectVariants> triple = null;
                            if (ratt != null)
                            {
                                string r = (subele.Name == "reflected" ? "person" : "photo_doc") +
                                           ratt.Value;
                                triple = new Tuple<string, string, ObjectVariants>(id, subele.Name.LocalName,
                                    new OV_iri(r));
                            }
                            else
                            {
                                string value = subele.Value; // Нужны языки и другие варианты!
                                bool possiblenumber = string.IsNullOrEmpty(value) ? false : true;
                                if (possiblenumber)
                                {
                                    char c = value[0];
                                    if (char.IsDigit(c) || c == '-')
                                    {
                                    }
                                    else possiblenumber = false;
                                }
                                triple = new Tuple<string, string, ObjectVariants>(id, subele.Name.LocalName,
                                    possiblenumber
                                        ? (ObjectVariants) new OV_int(int.Parse(value))
                                        : (ObjectVariants) new OV_string(value));
                            }
                            return triple;
                        }));
                    return seq;
                }));

            }, "load ", true);

        }



    }
}
