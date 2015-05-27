﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PolarDB;
using Polar.Data;
using RDFCommon.OVns;

namespace GoTripleStore
{
    public class TestPerPhoRef
    {
        public static void Main() //Main5()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            string path = "../../../Databases/";
            Console.WriteLine("Start GoTripleStore TestPerPhoRef.");

            int npersons = 40000;

            // Сначала коннектимся к базе данных
            GaGraphStringBased g = new GaGraphStringBased(path);
            bool toload = false;
            if (toload)
            { // Загружаем данные
                sw.Restart();
                TestDataGenerator generator = new TestDataGenerator(npersons, 2378459);
                g.Build(generator.Generate().SelectMany(ele => 
                {
                    string id = ele.Name + ele.Attribute("id").Value;
                    var seq = Enumerable.Repeat<Tuple<string,string,ObjectVariants>>(
                        new Tuple<string,string,ObjectVariants>(id, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", new OV_iri(ele.Name.LocalName)), 1)
                        .Concat(ele.Elements().Select(subele => 
                        {
                            XAttribute ratt = subele.Attribute("ref");
                            Tuple<string,string,ObjectVariants> triple = null;
                            if (ratt != null)
                            {
                                string r = (subele.Name == "reflected" ? "person" : "photo_doc") +
                                    ratt.Value;
                                triple = new Tuple<string, string, ObjectVariants>(id, subele.Name.LocalName, new OV_iri(r));
                            }
                            else
                            {
                                string value = subele.Value; // Нужны языки и другие варианты!
                                bool possiblenumber = string.IsNullOrEmpty(value) ? false : true;
                                if (possiblenumber)
                                {
                                    char c = value[0];
                                    if (char.IsDigit(c) || c == '-') { } else possiblenumber = false;
                                }
                                triple = new Tuple<string, string, ObjectVariants>(id, subele.Name.LocalName,
                                     possiblenumber ? (ObjectVariants)new OV_int(int.Parse(value)) : (ObjectVariants)new OV_string(value));
                            }
                            return triple;
                        }));
                    return seq;
                }));
                sw.Stop();
                Console.WriteLine("Load ok. duration={0}", sw.ElapsedMilliseconds);
            }
            else
            { // разогрев
                g.Warmup();
            }
            TPackGraph graph = new TPackGraph(g);
            Console.WriteLine("gra ok.");
            var qu = g.GetTriplesWithSubject("person3123");
            foreach (PaEntry v in qu)
            {
                object[] r = g.Dereference(v);
                Console.WriteLine("{0} {1} {2}:{3}", r[0], r[1], ((object[])r[2])[0], ((object[])r[2])[1]);
            }
            //Console.WriteLine("c={0}", qu.Count());

            Random rnd = new Random();
            int cnt = -1;

            sw.Restart();
            for (int i = 0; i < 1000; i++)
            {
                var qu2 = g.GetTriplesWithSubject("person" + rnd.Next(npersons - 1));
                cnt = qu2.Count();
            }
            sw.Stop();
            Console.WriteLine("1000 sPO ok. duration={0}", sw.ElapsedMilliseconds);

            sw.Restart();
            for (int i = 0; i < 1000; i++)
            {
                var qu2 = g.GetTriplesWithSubjectPredicate("person" + rnd.Next(npersons - 1), "name");
                cnt = qu2.Count();
            }
            sw.Stop();
            Console.WriteLine("1000 spO ok cnt={0}. duration={1}", cnt, sw.ElapsedMilliseconds);

            sw.Restart();
            var ov = new OV_iri("person");
            for (int i = 0; i < 1000; i++)
            {
                var exists = g.Contains("person" + rnd.Next(npersons - 1),
                    "http://www.w3.org/1999/02/22-rdf-syntax-ns#type",
                     ov);
                if (!exists) throw new Exception("438723");
            }
            sw.Stop();
            Console.WriteLine("1000 spO ok cnt={0}. duration={1}", cnt, sw.ElapsedMilliseconds);

            sw.Restart();
            for (int i = 0; i < 1000; i++)
            {
                var qu3 = g.GetTriplesWithPredicateObject("reflected",
                    new OV_iri("person" + rnd.Next(npersons - 1)))
                    .Select(ent => (string)((object[])g.Dereference(ent))[0])
                    .SelectMany(s => g.GetTriplesWithSubjectPredicate(s, "in_doc"))
                    .Select(en =>
                    {
                        var tri_o = g.Dereference(en);
                        var o = tri_o[2].ToOVariant();
                        return ((OV_iri)o).Name;
                    })
                    .SelectMany(s => g.GetTriplesWithSubjectPredicate(s, "name"))
                    .Select(en => g.Dereference(en))
                    ;
                //foreach (var en in qu3)
                //{
                //    object[] tri = en; // g.Dereference(en);
                //    //Console.WriteLine("triple {0} {1} {2}", tri[0], tri[1], (OV_string)(tri[2].ToOVariant()));
                //}
                cnt = qu3.Count();
            }
            sw.Stop();
            Console.WriteLine("1000 portraits ok cnt={0}. duration={1}", cnt, sw.ElapsedMilliseconds);

        }
    }
}