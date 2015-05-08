using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using PolarDB;
using RDFTurtleParser;

namespace GoTripleStore
{
    public class Program
    {
        public static void Main2(string Source_data_folder_path)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            string path = "../../../Databases/";
            Console.WriteLine("Start GoTripleStore coding triples (GaGraphStringBased).");
            var query = ReadTripleStringsFromTurtle.LoadGraph(Source_data_folder_path + "1.ttl")
                .Select(tri => new Tuple<string, string, ObjectVariants>(tri.Subject, tri.Predicate, tri.Object));

            //IGra<PaEntry> g = new GoGraphStringBased(path);
            GaGraphStringBased g = new GaGraphStringBased(path);
            bool toload = false;
            if (toload)
            {
                sw.Restart();
                g.Build(query);
                sw.Stop();
                Console.WriteLine("Load ok. duration={0}", sw.ElapsedMilliseconds);
            }
            else
            { // разогрев
                //g.Warmup();
            }

            {
                sw.Restart();
                //var fl = g.GetTriplesWithSubjectPredicate(
                //    "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/datafromproducer1/product12",
                //    "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productfeature"
                //    );
                var fl = g.GetTriplesWithSubjectPredicate(
                    "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/datafromproducer1/product12",
                    "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productpropertynumeric1"
    );
                int n = fl.Count();
                sw.Stop();
                Console.WriteLine("n={0} duration={1}", n, sw.ElapsedMilliseconds);
                //foreach (var ent in fl)
                //{
                //    var v = g.Dereference(ent);
                //    Console.WriteLine("{0} {1} {2} .", v[0], v[1], v[2].ToOVariant());
                //}

                sw.Restart();
                var fl2 = g.GetTriplesWithSubjectPredicate(
                    "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/datafromproducer1/product12",
                    "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productpropertynumeric1"
                    );
                n = fl2.Count();
                sw.Stop();
                Console.WriteLine("Ой! n={0} duration={1}", n, sw.ElapsedMilliseconds);

                var c = g.GetTriplesWithPredicateObject(
               "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productfeature",
               new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/productfeature142"))
               .Count();
                Console.WriteLine("Ай! n={0}", c);
                
                TripleStore ts = new TripleStore(g);

                //var str_fl = ts.GetObjBySubjPred(
                //    "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/datafromproducer1/product12",
                //    "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productfeature"
                //    );
                //foreach (var v in str_fl)
                //{
                //    Console.WriteLine(v);
                //}

                // =============== Это пропуск СПАРКЛ-ЛИНК тестов ===============

                //TripleStore ts = new TripleStore(g);
                int cnt;
                var test = BerlinTests.Query1(ts);
                sw.Restart();
                cnt = test.Count();
                sw.Stop();
                Console.WriteLine("n_results={0} duration={1}", cnt, sw.ElapsedMilliseconds);

                // Диагностический фрагмент
                //TripleStore_Diag ts_d = new TripleStore_Diag(g); 
                //int cnt;
                //var test = BerlinTests.QueryTestOpti(ts_d);
                //sw.Restart();
                //ts_d.StartAccumulate();
                //cnt = test.Count();
                //sw.Stop();
                //Console.WriteLine("n_results={0} duration={1}", cnt, sw.ElapsedMilliseconds);
                //sw.Restart();
                //ts_d.StartUse();
                //cnt = test.Count();
                //sw.Stop();
                //Console.WriteLine("n_results={0} duration={1}", cnt, sw.ElapsedMilliseconds);

                return;
            }
            var flow = g.GetTriplesWithSubjectPredicate(
                "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer10/Product468",
                "http://www.w3.org/2000/01/rdf-schema#label");
            foreach (var ent in flow)
            {
                var q = (object[])ent.Get();
                Console.WriteLine("{0} {1} {2}", g.DecodeIRI(q[0]), g.DecodeIRI(q[1]), g.DecodeOV(q[2]));
            }
            //Console.WriteLine(flow.Count());
            ProcessTrace(g, Source_data_folder_path);
        }
        private static void ProcessTrace(IGra<PaEntry> graph, string Source_data_folder_path)
        {
            System.Xml.Linq.XElement tracing = System.Xml.Linq.XElement.Load(Source_data_folder_path + "tracing100th.xml");
            Console.WriteLine("N_tests = {0}", tracing.Elements().Count());
            DateTime tt0 = DateTime.Now;
            tt0 = DateTime.Now;
            int ecnt = 0, ncnt = 0;
            foreach (XElement spo in tracing.Elements())
            {
                XAttribute s_att = spo.Attribute("subj");
                XAttribute p_att = spo.Attribute("pred");
                XAttribute o_att = spo.Attribute("obj");
                XAttribute r_att = spo.Attribute("res");
                string s = s_att == null ? null : s_att.Value;
                string p = p_att == null ? null : p_att.Value;
                string o = o_att == null ? null : o_att.Value;
                string res = r_att == null ? null : r_att.Value;
                if (spo.Name == "spo")
                {
                    bool r = graph.Contains(s, p, new OV_iri(o));
                    if ((res == "true" && r) || (res == "false" && !r)) { ecnt++; }
                    else ncnt++;
                }
                else if (spo.Name == "spD_")
                {
                    var query = graph.GetTriplesWithSubjectPredicate(s, p);
                    int cnt = query.Count(); ecnt++;
                }
                else if (spo.Name == "spO" || spo.Name == "spD")
                {
                    var query = graph.GetTriplesWithSubjectPredicate(s, p);
                    int count = query.Count();
                    if (count == 0 && res == "") { ecnt++; continue; }
                    if (count == res.Split(' ').Count())
                    {
                        ecnt++;
                        if (count == 1)
                        {
                            var ent = query.First();
                            var v = (graph.Dereference(ent)[2]).ToOVariant();
                            Console.WriteLine("{0}==>{1}", res, v);
                        }
                    }
                    else
                    {
                        ncnt++;
                    }

                }
                else if (spo.Name == "Spo")
                {
                    var query = graph.GetTriplesWithPredicateObject(p, new OV_iri(o));
                    if (query.Count() == 0 && res == "") continue;
                    ecnt++;
                }
            }
            Console.WriteLine("tracing duration=" + (DateTime.Now - tt0).Ticks / 10000L); tt0 = DateTime.Now;
            Console.WriteLine("Equal {0} Not equal {1}", ecnt, ncnt);
        }
        public static void Main1(string Source_data_folder_path)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            string path = "../../../Databases/";
            Console.WriteLine("Start GoTripleStore coding triples.");
            var query = ReadTripleStringsFromTurtle.LoadGraph(Source_data_folder_path + "1.ttl")
                .Select(tri => new Tuple<string, string, ObjectVariants>(tri.Subject, tri.Predicate, tri.Object));

            //IGra<PaEntry> g = new GoGraphStringBased(path);
            GoGraphStringBased g = new GoGraphStringBased(path);
            bool toload = false;
            if (toload)
            {
                sw.Restart();
                g.Build(query);
                sw.Stop();
                Console.WriteLine("Load ok. duration={0}", sw.ElapsedMilliseconds);
            }
            var flow = g.GetTriplesWithSubjectPredicate(
                "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer10/Product468",
                "http://www.w3.org/2000/01/rdf-schema#label");
            foreach (var ent in flow)
            {
                var q = (object[])ent.Get();
                Console.WriteLine("{0} {1} {2}", g.DecodeIRI(q[0]), g.DecodeIRI(q[1]), g.DecodeOV(q[2]));
            }
            //Console.WriteLine(flow.Count());
            ProcessTrace(g, Source_data_folder_path);
        }
        private static void ProcessTrace(GoGraphStringBased graph, string Source_data_folder_path)
        {
            System.Xml.Linq.XElement tracing = System.Xml.Linq.XElement.Load(Source_data_folder_path + "tracing100th.xml");
            Console.WriteLine("N_tests = {0}", tracing.Elements().Count());
            DateTime tt0 = DateTime.Now;
            tt0 = DateTime.Now;
            int ecnt = 0, ncnt = 0;
            foreach (XElement spo in tracing.Elements())
            {
                XAttribute s_att = spo.Attribute("subj");
                XAttribute p_att = spo.Attribute("pred");
                XAttribute o_att = spo.Attribute("obj");
                XAttribute r_att = spo.Attribute("res");
                string s = s_att == null ? null : s_att.Value;
                string p = p_att == null ? null : p_att.Value;
                string o = o_att == null ? null : o_att.Value;
                string res = r_att == null ? null : r_att.Value;
                if (spo.Name == "spo")
                {
                    bool r = graph.Contains(s, p, new OV_iri(o));
                    if ((res == "true" && r) || (res == "false" && !r)) { ecnt++; }
                    else ncnt++;
                }
                else if (spo.Name == "spD")
                {
                    var query = graph.GetTriplesWithSubjectPredicate(s, p);
                    int cnt = query.Count(); ecnt++;
                }
                else if (spo.Name == "spO")
                {
                    var query = graph.GetTriplesWithSubjectPredicate(s, p);
                    if (query.Count() == 0 && res == "") continue;
                    ecnt++;

                }
                else if (spo.Name == "Spo")
                {
                    var query = graph.GetTriplesWithPredicateObject(p, new OV_iri(o));
                    if (query.Count() == 0 && res == "") continue;
                    ecnt++;
                }
            }
            Console.WriteLine("tracing duration=" + (DateTime.Now - tt0).Ticks / 10000L); tt0 = DateTime.Now;
            Console.WriteLine("Equal {0} Not equal {1}", ecnt, ncnt);
        }


        public static void Main0(string Source_data_folder_path)
        {  // Работа с кодированными триплетами
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            string path = "../../../Databases/";
            Console.WriteLine("Start GoTripleStore coding triples.");
            var query = ReadTripleStringsFromTurtle.LoadGraph(Source_data_folder_path + "1.ttl");
            
            GoGraphIntBased cgraph = new GoGraphIntBased(path);
            bool toload = false;
            if (toload)
            {
                sw.Restart();
                cgraph.Build(query);
                sw.Stop();
                Console.WriteLine("Load ok. duration={0}", sw.ElapsedMilliseconds);

            }
            else // Здесь можно было бы разогревать базу данных.
            {
                cgraph.Warmup();
            }

            var search_query = cgraph.Search("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer10/Product468",
                "http://www.w3.org/2000/01/rdf-schema#label", null);
            Console.WriteLine(search_query.Count());
            foreach (var t in search_query)
            {
                Console.WriteLine("Triple: {0} {1} {2}", cgraph.Decode(t.Subject), cgraph.Decode(t.Predicate), t.Object);
            }

            ProcessTrace(cgraph, Source_data_folder_path);
        }
        // Выполнение операций по "следу" tracing
        private static void ProcessTrace(GoGraphIntBased graph, string Source_data_folder_path)
        {
            System.Xml.Linq.XElement tracing = System.Xml.Linq.XElement.Load(Source_data_folder_path + "tracing100th.xml");
            Console.WriteLine("N_tests = {0}", tracing.Elements().Count());
            DateTime tt0 = DateTime.Now;
            // Трансляия трассы
            XElement translated = new XElement("tracing");
            foreach (XElement spo in tracing.Elements())
            {
                XAttribute s_att = spo.Attribute("subj");
                XAttribute p_att = spo.Attribute("pred");
                XAttribute o_att = spo.Attribute("obj");
                XAttribute r_att = spo.Attribute("res");
                string s = s_att == null ? null : s_att.Value;
                string p = p_att == null ? null : p_att.Value;
                string o = o_att == null ? null : o_att.Value;
                string res = r_att == null ? null : r_att.Value;
                if (spo.Name == "spo")
                {
                    translated.Add(new XElement("spo",
                        new XAttribute("isubj", graph.Code(s)),
                        new XAttribute("ipred", graph.Code(p)),
                        new XAttribute("iobj", graph.Code(o)),
                        new XAttribute("res", res)));
                }
                else if (spo.Name == "spD")
                {
                    translated.Add(new XElement("spD",
                        new XAttribute("isubj", graph.Code(s)),
                        new XAttribute("ipred", graph.Code(p)),
                        new XAttribute("res", res)));
                }
                else if (spo.Name == "spO")
                {
                    translated.Add(new XElement("spO",
                        new XAttribute("isubj", graph.Code(s)),
                        new XAttribute("ipred", graph.Code(p)),
                        new XAttribute("res", res)));
                }
                else if (spo.Name == "Spo")
                {
                    translated.Add(new XElement("Spo",
                        new XAttribute("ipred", graph.Code(p)),
                        new XAttribute("iobj", graph.Code(o)),
                        new XAttribute("res", res)));
                }
            }

            tt0 = DateTime.Now;
            int ecnt = 0, ncnt = 0;
            foreach (XElement spo in translated.Elements())
            {
                XAttribute s_att = spo.Attribute("isubj");
                XAttribute p_att = spo.Attribute("ipred");
                XAttribute o_att = spo.Attribute("iobj");
                XAttribute r_att = spo.Attribute("res");
                int s = s_att == null ? -1 : Int32.Parse(s_att.Value);
                int p = p_att == null ?  -1 : Int32.Parse(p_att.Value);
                int o = o_att == null ?  -1 : Int32.Parse(o_att.Value);
                string res = r_att == null ? null : r_att.Value;
                if (spo.Name == "spo")
                {
                    bool r = graph.Contains(s, p, new OV_iriint(o, i => null));
                    if ((res == "true" && r) || (res == "false" && !r)) { ecnt++; }
                    else ncnt++;
                }
                else if (spo.Name == "spD")
                {
                    var query = graph.GetTriplesWithSubjectPredicate(s, p);
                    int cnt = query.Count();
                    //Literal lit = ts.GetDataBySubjPred(
                    //    s.GetHashCode(),
                    //    p.GetHashCode()).FirstOrDefault();
                    //if (lit == null) { ncnt++; }
                    //else
                    //{
                    //    bool isEq = false;
                    //    if (lit.vid == LiteralVidEnumeration.text &&
                    //        ((Text)lit.value).s == res.Substring(1, res.Length - 2)) isEq = true;
                    //    else isEq = lit.ToString() == res;
                    //    if (isEq) ecnt++; else ncnt++;
                    //}
                }
                else if (spo.Name == "spO")
                {
                    var query = graph.GetTriplesWithSubjectPredicate(s, p);
                    //var query = ts.GetObjBySubjPred(
                    //    s.GetHashCode(),
                    //    p.GetHashCode()).OrderBy(v => v).ToArray();
                    if (query.Count() == 0 && res == "") continue;
                    ecnt++;

                }
                else if (spo.Name == "Spo")
                {
                    var query = graph.GetTriplesWithPredicateObject(p, new OV_iriint(o, i => null));
                    //Console.WriteLine("Spo {0}", query.Count());
                    //var query = ts.GetSubjectByObjPred(
                    //    o.GetHashCode(),
                    //    p.GetHashCode()).OrderBy(v => v).ToArray();
                    if (query.Count() == 0 && res == "") continue;
                    ecnt++;
                }
            }
            Console.WriteLine("tracing duration=" + (DateTime.Now - tt0).Ticks / 10000L); tt0 = DateTime.Now;
            Console.WriteLine("Equal {0} Not equal {1}", ecnt, ncnt);
        }
        // Выполнение операций по "следу" tracing
        private static void ProcessTrace0(IGraph<TripleIntOV> graph, string Source_data_folder_path)
        {
            System.Xml.Linq.XElement tracing = System.Xml.Linq.XElement.Load(Source_data_folder_path + "tracing100th.xml");
            Console.WriteLine("N_tests = {0}", tracing.Elements().Count());
            DateTime tt0 = DateTime.Now;
            int ecnt = 0, ncnt = 0;
            foreach (XElement spo in tracing.Elements())
            {
                XAttribute s_att = spo.Attribute("subj");
                XAttribute p_att = spo.Attribute("pred");
                XAttribute o_att = spo.Attribute("obj");
                XAttribute r_att = spo.Attribute("res");
                string s = s_att == null ? null : s_att.Value;
                string p = p_att == null ? null : p_att.Value;
                string o = o_att == null ? null : o_att.Value;
                string res = r_att == null ? null : r_att.Value;
                if (spo.Name == "spo")
                {
                    var query = graph.Search(s, p, new OV_iri(o));
                    bool r = query.Any();
                    if ((res == "true" && r) || (res == "false" && !r)) { ecnt++; }
                    else ncnt++;
                }
                else if (spo.Name == "spD_")
                {
                    //Literal lit = ts.GetDataBySubjPred(
                    //    s.GetHashCode(),
                    //    p.GetHashCode()).FirstOrDefault();
                    //if (lit == null) { ncnt++; }
                    //else
                    //{
                    //    bool isEq = false;
                    //    if (lit.vid == LiteralVidEnumeration.text &&
                    //        ((Text)lit.value).s == res.Substring(1, res.Length - 2)) isEq = true;
                    //    else isEq = lit.ToString() == res;
                    //    if (isEq) ecnt++; else ncnt++;
                    //}
                }
                else if (spo.Name == "spO_")
                {
                    //var query = ts.GetObjBySubjPred(
                    //    s.GetHashCode(),
                    //    p.GetHashCode()).OrderBy(v => v).ToArray();
                    //if (query.Count() == 0 && res == "") continue;
                    //ecnt++;
                    var query = graph.Search(s, p, null);
                    if (query.Count() == 0 && res == "") { }//continue;
                    ecnt++;
                }
                else if (spo.Name == "Spo_")
                {
                    //var query = ts.GetSubjectByObjPred(
                    //    o.GetHashCode(),
                    //    p.GetHashCode()).OrderBy(v => v).ToArray();
                    var query = graph.Search(null, p, new OV_iri(o));
                    if (query.Count() == 0 && res == "") continue;
                    ecnt++;
                }
            }
            Console.WriteLine("tracing duration=" + (DateTime.Now - tt0).Ticks / 10000L); tt0 = DateTime.Now;
            Console.WriteLine("Equal {0} Not equal {1}", ecnt, ncnt);
        }
    }
}
