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
        public static void Main() // Main9()
        {
            string path = "../../../Databases/";
            //PolarDB.MachineInfo.SetPathForTmp(path);
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            Random rnd = new Random();
            TriplesGraphInt g = new TriplesGraphInt(path);
            int npersons = 40000;
            PaEntry.bufferBytes = 200 * 1000000;

            bool toload = false;
            toload = true;
            if (toload)
            {
                sw.Restart();
                TestDataGenerator generator = new TestDataGenerator(npersons, 2378459);
                g.Build(generator.Generate().SelectMany(ele =>
                {
                    string id = ele.Name + ele.Attribute("id").Value;
                    var seq = Enumerable.Repeat<Tuple<string, string, ObjectVariants>>(
                        new Tuple<string, string, ObjectVariants>(id, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", new OV_iri(ele.Name.LocalName)), 1)
                        .Concat(ele.Elements().Select(subele =>
                        {
                            XAttribute ratt = subele.Attribute("ref");
                            Tuple<string, string, ObjectVariants> triple = null;
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
            {
                g.ActivateCache();
                g.Start();
            }

            int ic = g.Code("person3322");
            Console.WriteLine("person3322={0}", ic);
            string s = g.Decode(ic);
            Console.WriteLine("{0}", s);

            int iname = g.Code("name");
            Console.WriteLine("name={0}", iname);
            string s2 = g.Decode(iname);
            Console.WriteLine("{0}", s2);

            //// Измерение скорости кодирования
            //sw.Restart();
            //for (int i = 0; i < 10000; i++) { ic = ttab.Code("person" + rnd.Next(npersons - 1)); }
            //sw.Stop();
            //Console.WriteLine("10000 Code ok. duration={0}", sw.ElapsedMilliseconds);

            var query = g.GetTriplesWithSubjectPredicate(new OV_iriint(ic, null), new OV_iriint(iname, null));
            foreach (var tr in query)
            {
                Console.WriteLine("{0} {1} {2}", tr.Subj, tr.Pred, tr.Obj);
            }

            // Измерение времени поиска по заданным предикату и субъекту
            sw.Restart();
            for (int i = 0; i < 10000; i++)
            {
                ic = g.Code("person" + rnd.Next(npersons - 1));
                var names = g.GetTriplesWithSubjectPredicate(new OV_iriint(ic, null), new OV_iriint(iname, null));
                if (names.Count() != 1) Console.WriteLine("NOT ONE NAME: {0}", names.Count());
            }
            sw.Stop();
            Console.WriteLine("10000 person names ok. duration={0}", sw.ElapsedMilliseconds);

            int ireflected = g.Code("reflected");
            var qu3 = g.GetTriplesWithPredicateObject(new OV_iriint(ireflected, null), new OV_iriint(ic, null));
            Console.WriteLine(qu3.Count());

            int sum = 0;
            int in_doc = g.Code("in_doc");
            if (!toload || (toload && npersons < 4000000))
            {
                sw.Restart();
                for (int i = 0; i < 10000; i++)
                {
                    string id = "person" + rnd.Next(npersons - 1);
                    int iid = g.Code(id);
                    ObjectVariants ov = new OV_iriint(iid, null);

                    var qu4 = g.GetTriplesWithPredicateObject(new OV_iriint(ireflected, null), ov)
                        .SelectMany(tr => g.GetTriplesWithSubjectPredicate(tr.Subj, new OV_iriint(in_doc, null)))
                        .SelectMany(tr => g.GetTriplesWithSubjectPredicate(tr.Obj, new OV_iriint(iname, null)))
                        ;
                    sum += qu4.Count();
                }
                sw.Stop();
                Console.WriteLine("10000 person inv relations ok. duration={0} sum={1}", sw.ElapsedMilliseconds, sum);
            }
        }
        public static void Main8() // Main8()
        {
            string path = "../../../Databases/";
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            Random rnd = new Random();
            TripleSetInt ttab = new TripleSetInt(path);
            int npersons = 40000;
            PaEntry.bufferBytes = 200000000;

            bool toload = false;
            toload = true;
            if (toload)
            {
                sw.Restart();
                TestDataGenerator generator = new TestDataGenerator(npersons, 2378459);
                ttab.Build(generator.Generate().SelectMany(ele => 
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
            else { ttab.Warmup(); ttab.Start(); }
            
            int ic = ttab.Code("person3322");
            Console.WriteLine("person3322={0}", ic);
            string s = ttab.Decode(ic);
            Console.WriteLine("{0}", s);

            int iname = ttab.Code("name");
            Console.WriteLine("name={0}", iname);
            string s2 = ttab.Decode(iname);
            Console.WriteLine("{0}", s2);

            //// Измерение скорости кодирования
            //sw.Restart();
            //for (int i = 0; i < 10000; i++) { ic = ttab.Code("person" + rnd.Next(npersons - 1)); }
            //sw.Stop();
            //Console.WriteLine("10000 Code ok. duration={0}", sw.ElapsedMilliseconds);

            var query = ttab.GetTriplesWithPredicateSubject(iname, ic);
            foreach (PaEntry ent in query)
            {
                var pv = ent.GetValue();
                Console.WriteLine("{0}", pv.Type.Interpret(pv.Value));
            }

            // Измерение времени поиска по заданным предикату и субъекту
            sw.Restart();
            for (int i = 0; i < 10000; i++) 
            { 
                ic = ttab.Code("person" + rnd.Next(npersons - 1));
                var names = ttab.GetTriplesWithPredicateSubject(iname, ic);
                if (names.Count() != 1) Console.WriteLine("NOT ONE NAME: {0}", names.Count());
            }
            sw.Stop();
            Console.WriteLine("10000 person names ok. duration={0}", sw.ElapsedMilliseconds);

                //var qu3 = g.GetTriplesWithPredicateObject("reflected",
                //    new OV_iri("person" + rnd.Next(npersons - 1)))

            int ireflected = ttab.Code("reflected");
            //int iperson = ttab.Code("person" + rnd.Next(npersons - 1));
            var qu3 = ttab.GetTriplesWithPredicateObject(ireflected, new OV_iriint(ic, null));
            Console.WriteLine(qu3.Count());

            int sum = 0;
            int in_doc = ttab.Code("in_doc");
            TripleSetInt g = ttab;
            sw.Restart();
            for (int i = 0; i < 10000; i++)
            {
                string id = "person" + rnd.Next(npersons - 1);
                int iid = ttab.Code(id);
                ObjectVariants ov = new OV_iriint(iid, null);
                
                var qu4 = g.GetTriplesWithPredicateObjectTest(ireflected, ov)
                    .Select(ob => (int)((object[])ob)[0])
                    .SelectMany(c => g.GetTriplesWithPredicateSubjectTest(in_doc, (int)c))
                    .Select(ob => ((object[])((object[])ob)[2])[1])
                    .SelectMany(c => g.GetTriplesWithPredicateSubject(iname, (int)c))
                    //.Select(en => g.Dereference(en))
                    ;
                var qu5 = g.GetTriplesWithPredicateObject(ireflected, ov)
                    .Select(ent => (int)((object[])g.Dereference(ent))[0])
                    .SelectMany(c => g.GetTriplesWithPredicateSubject(in_doc, c))
                    .Select(en =>
                    {
                        var tri_o = (object[])g.Dereference(en);
                        int o = (int)((object[])tri_o[2])[1];
                        return o;
                    })
                    .SelectMany(c => g.GetTriplesWithPredicateSubject(iname, c))
                    .Select(en => g.Dereference(en))
                    ;
                sum += qu4.Count();
            }
            sw.Stop();
            Console.WriteLine("10000 person inv relations ok. duration={0} sum={1}", sw.ElapsedMilliseconds, sum);

        }
        public static void Main5() //Main5()
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
            Console.WriteLine("1000 spo ok cnt={0}. duration={1}", cnt, sw.ElapsedMilliseconds);

            sw.Restart();
            for (int i = 0; i < 1000; i++)
            {
                var qu3 = g.GetTriplesWithPredicateObject("reflected",
                    new OV_iri("person" + rnd.Next(npersons - 1)))
                    //.Select(ent => (string)((object[])g.Dereference(ent))[0])
                    //.SelectMany(s => g.GetTriplesWithSubjectPredicate(s, "in_doc"))
                    //.Select(en =>
                    //{
                    //    var tri_o = g.Dereference(en);
                    //    var o = tri_o[2].ToOVariant();
                    //    return ((OV_iri)o).Name;
                    //})
                    //.SelectMany(s => g.GetTriplesWithSubjectPredicate(s, "name"))
                    //.Select(en => g.Dereference(en))
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
