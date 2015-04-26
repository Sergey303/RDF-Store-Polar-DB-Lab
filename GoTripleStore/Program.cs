﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RDFCommon;
using RDFTripleStore;
using RDFTripleStore.OVns;

namespace GoTripleStore
{
    public class Program
    {
        public static void Main()
        {  // Работа с кодированными триплетами
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            string path = "../../../Databases/";
            Console.WriteLine("Start GoTripleStore coding triples.");
            var query = ReadTripleStringsFromTurtle.LoadGraph(Config.Source_data_folder_path + "1.ttl");
            
            GoGraphIntBased cgraph = new GoGraphIntBased(path);
            bool toload = true;
            if (toload)
            {
                sw.Restart();
                cgraph.Build(query);
                sw.Stop();
                Console.WriteLine("Load ok. duration={0}", sw.ElapsedMilliseconds);
            }
            var search_query = cgraph.Search("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer10/Product468",
                "http://www.w3.org/2000/01/rdf-schema#label", null);
            Console.WriteLine(search_query.Count());
            foreach (var t in search_query)
            {
                Console.WriteLine("Triple: {0} {1} {2}", cgraph.Decode(t.Subject), cgraph.Decode(t.Predicate), t.Object);
            }

            //ProcessTrace(cgraph);
        }
        // Выполнение операций по "следу" tracing
        private static void ProcessTrace(IGraph<Triple<int, int, ObjectVariants>> graph)
        {
            System.Xml.Linq.XElement tracing = System.Xml.Linq.XElement.Load(Config.Source_data_folder_path + "tracing100th.xml");
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
                if (spo.Name == "spo_")
                {
                    //bool r = ts.ChkOSubjPredObj(
                    //    s.GetHashCode(),
                    //    p.GetHashCode(),
                    //    o.GetHashCode());
                    //if ((res == "true" && r) || (res == "false" && !r)) { ecnt++; }
                    //else ncnt++;
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
                else if (spo.Name == "spO")
                {
                    //var query = ts.GetObjBySubjPred(
                    //    s.GetHashCode(),
                    //    p.GetHashCode()).OrderBy(v => v).ToArray();
                    //if (query.Count() == 0 && res == "") continue;
                    //ecnt++;
                    var query = graph.Search(s, p, null);
                    if (query.Count() == 0 && res == "") continue;
                    if (ecnt < 10) 
                    {
                        var tr_res = query.Select(t => t.Object).FirstOrDefault();
                        Console.WriteLine("res={0}, obj={1}", res, tr_res);
                    }
                    ecnt++;
                }
                else if (spo.Name == "Spo_")
                {
                    //var query = ts.GetSubjectByObjPred(
                    //    o.GetHashCode(),
                    //    p.GetHashCode()).OrderBy(v => v).ToArray();
                    //if (query.Count() == 0 && res == "") continue;
                    //ecnt++;
                }
            }
            Console.WriteLine("tracing duration=" + (DateTime.Now - tt0).Ticks / 10000L); tt0 = DateTime.Now;
            Console.WriteLine("Equal {0} Not equal {1}", ecnt, ncnt);
        }
        public static void Main0()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            string path = "../../../Databases/";
            Console.WriteLine("Start GoTripleStore.");
            var query = ReadTripleStringsFromTurtle.LoadGraph(Config.Source_data_folder_path + "1.ttl");
            //Console.WriteLine(query.Count());
            GoGraphStringBased ggraph = new GoGraphStringBased(path);
            bool toload = false;
            if (toload)
            {
                sw.Restart();
                ggraph.Build(query);
                sw.Stop();
                Console.WriteLine("Load ok. duration={0}", sw.ElapsedMilliseconds);
            }
            var search_query = ggraph.Search("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer10/Product468",
                "http://www.w3.org/2000/01/rdf-schema#label", null);
            foreach (var t in search_query)
            {
                Console.WriteLine("Triple: {0} {1} {2}", t.Subject, t.Predicate, t.Object);
            }
            
        }
    }
}
