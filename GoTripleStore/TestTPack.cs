using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDFCommon.OVns;
using PolarDB;

namespace GoTripleStore
{
    class TestTPack
    {
        public static void Main()
        {
            Random rnd = new Random(987654321);
            string[] arr = Allproducts.Products;
            HashSet<string> hs = new HashSet<string>();
            string[] productsXYZ = new string[1000];
            for (int i = 0; i < productsXYZ.Length; i++)
            {
                string s = null;
                while (hs.Contains(s = arr[rnd.Next(arr.Length - 1)])) ;
                hs.Add(s);
                productsXYZ[i] = s;
            }

            string Source_data_folder_path = System.IO.File.ReadAllLines("../../../config.ini")
                .Where(line => line.StartsWith("#source_data_folder_path"))
                .Select(line => line.Substring("#source_data_folder_path".Length + 1))
                .First();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            string path = "../../../Databases/";
            Console.WriteLine("Start GoTripleStore coding triples (GaGraphStringBased).");
            var query = RDFTurtleParser.ReadTripleStringsFromTurtle.LoadGraph(Source_data_folder_path + "1.ttl")
                .Select(tri => new Tuple<string, string, ObjectVariants>(tri.Subject, tri.Predicate, tri.Object));

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

            IGraph gra = new TPackGraph(g);
            Console.WriteLine("gra ok.");

            var fl = g.GetTriplesWithPredicateObject(
                "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productFeature",
                new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/ProductFeature16"));
            int n = fl.Count();
            Console.WriteLine("n={0}", n);

            IEnumerable<TPack> qu;
            int cnt = -1;

            List<object[]> test_pars = new List<object[]>();
            TextReader tr = new StreamReader(Source_data_folder_path + "param values for1m 1 query.txt");
            string ln = null;
            while ((ln = tr.ReadLine()) != null)
            {
                string tp = ln;
                string pf1 = tr.ReadLine();
                string pf2 = tr.ReadLine();
                ln = tr.ReadLine();
                int x = Int32.Parse(ln);
                test_pars.Add(new object[] { tp, pf1, pf2, x });
            }

            cnt = 0;
            sw.Restart();
            for (int i = 0; i < 100; i++)
            {
                object[] pp = test_pars[i];
                qu = Query1p(gra, (string)pp[0], (string)pp[1], (string)pp[2], (int)pp[3]);
                cnt += qu.Count();
            }
            sw.Stop();
            Console.WriteLine("query1p ok. cnt={0} duration={1}", cnt, sw.ElapsedMilliseconds);

            return;

            sw.Restart();
            //qu = Query5t(gra);
            for (int i = 0; i < 100; i++)
            {
                qu = Query1t(gra);
                cnt = qu.Count();
            }
            sw.Stop();
            Console.WriteLine("query5t ok. cnt={0} duration={1}", cnt, sw.ElapsedMilliseconds);

            //return;

            sw.Restart();
            ((TPackGraph)gra).StartCount();
            //((TPackGraph)gra).StartCache();
            for (int i = 0; i < 100; i++)
            {
                //qu = Query5p(gra, productsXYZ[i]);
                qu = Query2p(gra, productsXYZ[i]);
                cnt = qu.Count();
            }
            ((TPackGraph)gra).PrintCount();
            sw.Stop();
            Console.WriteLine("query5p ok. cnt={0} duration={1}", cnt, sw.ElapsedMilliseconds);



            //TPackGraph_Diag grad = new TPackGraph_Diag(g);

            //grad.StartAccumulate();
            //sw.Restart();
            //qu = Query6t(grad);
            //cnt = qu.Count();
            //sw.Stop();
            //Console.WriteLine("query5t ok. cnt={0} duration={1}", cnt, sw.ElapsedMilliseconds);

            //grad.StartUse();
            //sw.Restart();
            //qu = Query6t(grad);
            //cnt = qu.Count();
            //sw.Stop();
            //Console.WriteLine("query5t ok. cnt={0} duration={1}", cnt, sw.ElapsedMilliseconds);
                    
        }
        public static IEnumerable<TPack> Query5t(IGraph g)
        {
            ObjectVariants[] row = new ObjectVariants[7];
            ObjectVariants _prodFeature = new OV_index(0), _produc = new OV_index(1), _productLabel = new OV_index(2), _origProperty1 = new OV_index(3), _simProperty1 = new OV_index(4);
            ObjectVariants _origProperty2 = new OV_index(5), _simProperty2 = new OV_index(6);
            ObjectVariants iri1 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer1/Product12"),
                iri2 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productFeature"),
                iri3 = new OV_iri("http://www.w3.org/2000/01/rdf-schema#label"),
                iri4 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productPropertyNumeric1"),
                iri5 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productPropertyNumeric2");
            var quer = Enumerable.Repeat<TPack>(new TPack(row, g), 1)
                .spo(iri1, iri2, _prodFeature)
                .spo(_produc, iri2, _prodFeature)
                .Where(pack => ! pack.Get(_produc).Equals(iri1))
                .spo(_produc, iri3, _productLabel)
                .spo(iri1, iri4, _origProperty1)
                .spo(_produc, iri4, _simProperty1)
                .Where(pack =>
                {
                    int sp1 = ((OV_int)pack.Get(_simProperty1)).value;
                    int op1 = ((OV_int)pack.Get(_origProperty1)).value;
                    return sp1 < (op1 + 120) && sp1 > (op1 - 120);
                })
                .spo(iri1, iri5, _origProperty2)
                .spo(_produc, iri5, _simProperty2)
                .Where(pack => 
                {
                    int sp2 = ((OV_int)pack.Get(_simProperty2)).value;
                    int op2 = ((OV_int)pack.Get(_origProperty2)).value;
                    return (sp2 < (op2 + 170) && sp2 > (op2 - 170));
                })
                ;
            return quer;
        }
        public static IEnumerable<TPack> Query5p(IGraph g, string productXYZ)
        {
            ObjectVariants[] row = new ObjectVariants[7];
            ObjectVariants _prodFeature = new OV_index(0), _produc = new OV_index(1), _productLabel = new OV_index(2), _origProperty1 = new OV_index(3), _simProperty1 = new OV_index(4);
            ObjectVariants _origProperty2 = new OV_index(5), _simProperty2 = new OV_index(6);
            ObjectVariants iri1 = new OV_iri(productXYZ),
                iri2 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productFeature"),
                iri3 = new OV_iri("http://www.w3.org/2000/01/rdf-schema#label"),
                iri4 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productPropertyNumeric1"),
                iri5 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productPropertyNumeric2");
            var quer = Enumerable.Repeat<TPack>(new TPack(row, g), 1)
                .spo(iri1, iri2, _prodFeature)
                .spo(_produc, iri2, _prodFeature)
                .Where(pack => !pack.Get(_produc).Equals(iri1))
                .spo(_produc, iri3, _productLabel)
                .spo(iri1, iri4, _origProperty1)
                .spo(_produc, iri4, _simProperty1)
                .Where(pack =>
                {
                    int sp1 = ((OV_int)pack.Get(_simProperty1)).value;
                    int op1 = ((OV_int)pack.Get(_origProperty1)).value;
                    return sp1 < (op1 + 120) && sp1 > (op1 - 120);
                })
                .spo(iri1, iri5, _origProperty2)
                .spo(_produc, iri5, _simProperty2)
                .Where(pack =>
                {
                    int sp2 = ((OV_int)pack.Get(_simProperty2)).value;
                    int op2 = ((OV_int)pack.Get(_origProperty2)).value;
                    return (sp2 < (op2 + 170) && sp2 > (op2 - 170));
                })
                ;
            return quer;
        }
        public static IEnumerable<TPack> Query1t(IGraph g)
        {
            ObjectVariants[] row = new ObjectVariants[3];
            ObjectVariants _product = new OV_index(0), _label = new OV_index(1), _value1 = new OV_index(2);
            ObjectVariants bsbm_productFeature = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productFeature"),
                bsbm_inst_ProductFeature19 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/ProductFeature19"),
                bsbm_inst_ProductFeature8 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/ProductFeature8"),
                bsbm_inst_ProductType1 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/ProductType1"),
                a = new OV_iri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"),
                label = new OV_iri("http://www.w3.org/2000/01/rdf-schema#label"),
                iri4 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productPropertyNumeric1");
            var quer = Enumerable.Repeat<TPack>(new TPack(row, g), 1)
                .spo(_product, bsbm_productFeature, bsbm_inst_ProductFeature19)
                .spo(_product, bsbm_productFeature, bsbm_inst_ProductFeature8)
                .spo(_product, a, bsbm_inst_ProductType1)
                .spo(_product, iri4, _value1)
                .spo(_product, label, _label)
                .Where(pack => ((OV_int)pack.Get(_value1)).value > 10)
                ;
            return quer;
        }
        public static IEnumerable<TPack> Query1p(IGraph g, string tp, string pf1, string pf2, int x)
        {
            ObjectVariants[] row = new ObjectVariants[3];
            ObjectVariants _product = new OV_index(0), _label = new OV_index(1), _value1 = new OV_index(2);
            ObjectVariants bsbm_productFeature = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productFeature"),
                bsbm_inst_ProductFeature19 = new OV_iri(pf1),
                bsbm_inst_ProductFeature8 = new OV_iri(pf2),
                bsbm_inst_ProductType1 = new OV_iri(tp),
                a = new OV_iri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"),
                label = new OV_iri("http://www.w3.org/2000/01/rdf-schema#label"),
                iri4 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productPropertyNumeric1");
            var quer = Enumerable.Repeat<TPack>(new TPack(row, g), 1)
                .spo(_product, bsbm_productFeature, bsbm_inst_ProductFeature19)
                .spo(_product, bsbm_productFeature, bsbm_inst_ProductFeature8)
                .spo(_product, a, bsbm_inst_ProductType1)
                .spo(_product, iri4, _value1)
                .spo(_product, label, _label)
                .Where(pack => ((OV_int)pack.Get(_value1)).value > x)
                ;
            return quer;
        }
        public static IEnumerable<TPack> Query2p(IGraph g, string productXYZ)
        {
            ObjectVariants[] row = new ObjectVariants[11];
            ObjectVariants _label = new OV_index(0), _comment = new OV_index(1), _p = new OV_index(2), _producer = new OV_index(3), _f = new OV_index(4);
            ObjectVariants _productFeature = new OV_index(5);
            ObjectVariants _protertyTextual1 = new OV_index(6);
            ObjectVariants _protertyTextual2 = new OV_index(7);
            ObjectVariants _protertyTextual3 = new OV_index(8);
            ObjectVariants _protertyNumeric1 = new OV_index(9);
            ObjectVariants _protertyNumeric2 = new OV_index(10);
            ObjectVariants XYZ = new OV_iri(productXYZ),
                pF = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productFeature"),
                label = new OV_iri("http://www.w3.org/2000/01/rdf-schema#label"),
                comment = new OV_iri("http://www.w3.org/2000/01/rdf-schema#comment"),
                producer = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/producer"),
                publisher = new OV_iri("http://purl.org/dc/elements/1.1/publisher"),
                t1 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productPropertyTextual1"),
                t2 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productPropertyTextual1"),
                t3 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productPropertyTextual1"),
                n1 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productPropertyNumeric1"),
                n2 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productPropertyNumeric1");
            var quer = Enumerable.Repeat<TPack>(new TPack(row, g), 1)
                .spo(XYZ, label, _label)
                .spo(XYZ, comment, _comment)
                .spo(XYZ, producer, _p)
                .spo(_p, label, _producer)
                .spo(XYZ, publisher, _p)
                .spo(XYZ, pF, _f)
                .spo(_f, label, _productFeature)
                .spo(XYZ, t1, _protertyTextual1)
                .spo(XYZ, t2, _protertyTextual2)
                .spo(XYZ, t3, _protertyTextual3)
                .spo(XYZ, n1, _protertyNumeric1)
                .spo(XYZ, n2, _protertyNumeric2)
                // Еще опционы должны быть
                ;
            return quer;
        }
    }

    public class TPackGraph : IGraph
    {
        private bool tocache = false;
        private int nspo = 0, nSPO = 0, nsPO = 0, nspO = 0, nSpO = 0, nSpo = 0, nSPo = 0;
        public void StartCount() { nspo = 0; nSPO = 0; nsPO = 0; nspO = 0; nSpO = 0; nSpo = 0; nSPo = 0; }
        public void PrintCount() { Console.WriteLine("nspo={0} nSPO={1} nsPO={2} nspO={3} nSpO={4} nSpo={5} nSPo={6};", nspo, nSPO, nsPO, nspO, nSpO, nSpo, nSPo); }
        public void StartCache()
        {
            tocache = true;
        }

        protected IGra<PaEntry> go;
        public TPackGraph(IGra<PaEntry>go)
        {
            this.go = go;
        }
        public virtual bool Contains(ObjectVariants subj, ObjectVariants pred, ObjectVariants obj)
        {
            nspo++;
            return go.Contains(subj.WritableValue, pred.WritableValue, obj);
        }
        public virtual IEnumerable<Triple> GetTriples()
        {
            nSPO++;
            return go.GetTriples().Select(ent => 
            {
                object[] va = go.Dereference(ent);
                return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
            });
        }
        public virtual IEnumerable<Triple> GetTriplesWithSubject(ObjectVariants subj)
        {
            nsPO++;
            return go.GetTriplesWithSubject(((OV_iri)subj).UriString)
                .Select(ent => 
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        Dictionary<string, Triple[]> cache_sp = new Dictionary<string, Triple[]>();
        public virtual IEnumerable<Triple> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            Triple[] res = null;
            string subjpred = null; 
            if (tocache)
            {
                subjpred = ((OV_iri)subj).Name + "|" + ((OV_iri)pred).Name;
                if (cache_sp.TryGetValue(subjpred, out res))
                {
                    return res;
                }
            }
            nspO++;
            res = go.GetTriplesWithSubjectPredicate(((OV_iri)subj).UriString, ((OV_iri)pred).UriString)
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                }).ToArray();
            if (tocache) cache_sp.Add(subjpred, res);
            return res;
        }
        public virtual IEnumerable<Triple> GetTriplesWithPredicate(ObjectVariants pred)
        {
            nSpO++;
            return go.GetTriplesWithPredicate(((OV_iri)pred).UriString)
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        private Dictionary<Tuple<ObjectVariants, ObjectVariants>, Triple[]> cache_po = new Dictionary<Tuple<ObjectVariants, ObjectVariants>, Triple[]>();
        public virtual IEnumerable<Triple> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            Triple[] res = null;
            Tuple<ObjectVariants, ObjectVariants> subjpred = null;
            if (tocache)
            {
                subjpred = new Tuple<ObjectVariants,ObjectVariants>(pred, obj);
                if (cache_po.TryGetValue(subjpred, out res))
                {
                    return res;
                }
            }
            nSpo++;
            res = go.GetTriplesWithPredicateObject(((OV_iri)pred).UriString, obj)
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                }).ToArray();
            if (tocache) cache_po.Add(subjpred, res);
            return res;
        }
        public virtual IEnumerable<Triple> GetTriplesWithObject(ObjectVariants obj)
        {
            nSPo++;
            return go.GetTriplesWithObject(obj)
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
    }
    public class TPackGraph_Diag : TPackGraph
    {
        //private IGra<PaEntry> go;
        public TPackGraph_Diag(IGra<PaEntry> go) : base(go)
        {
            //this.go = go;
        }
        // Определения для выполнения диагностики в стиле - один просчет накапливает данные по сделанным запросам к памяти триплетов, второй использует 
        private List<object> solutions = new List<object>();
        private int position = 0;
        private bool accumulate = false;
        private bool use = false;
        public void StartAccumulate()
        {
            use = false;
            accumulate = true;
            solutions = new List<object>();
        }
        public void StartUse()
        {
            accumulate = false;
            use = true;
            position = 0;
        }

        public override bool Contains(ObjectVariants subj, ObjectVariants pred, ObjectVariants obj)
        {
            bool solution;
            if (use)
            {
                solution = (bool)solutions[position];
                position++;
            }
            else
            {
                solution = go.Contains(subj.WritableValue, pred.WritableValue, obj);
                if (accumulate) solutions.Add(solution);
            }
            return solution;
        }
        public override IEnumerable<Triple> GetTriples()
        {
            PaEntry[] solution = null;
            if (use)
            {
                solution = (PaEntry[])solutions[position];
                position++;
            }
            else
            {
                solution = go.GetTriples().ToArray();
                if (accumulate) solutions.Add(solution);
            }
            return solution.Select(ent =>
            {
                object[] va = go.Dereference(ent);
                return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
            });
        }
        public override IEnumerable<Triple> GetTriplesWithSubject(ObjectVariants subj)
        {
            PaEntry[] solution = null;
            if (use)
            {
                solution = (PaEntry[])solutions[position];
                position++;
            }
            else
            {
                solution = go.GetTriplesWithSubject(((OV_iri)subj).UriString).ToArray();
                if (accumulate) solutions.Add(solution);
            }
            return solution
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        public override IEnumerable<Triple> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            PaEntry[] solution = null;
            if (use)
            {
                solution = (PaEntry[])solutions[position];
                position++;
            }
            else
            {
                solution = go.GetTriplesWithSubjectPredicate(((OV_iri)subj).UriString, ((OV_iri)pred).UriString).ToArray();
                if (accumulate) solutions.Add(solution);
            }
            return solution
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        public override IEnumerable<Triple> GetTriplesWithPredicate(ObjectVariants pred)
        {
            PaEntry[] solution = null;
            if (use)
            {
                solution = (PaEntry[])solutions[position];
                position++;
            }
            else
            {
                solution = go.GetTriplesWithPredicate(((OV_iri)pred).UriString).ToArray();
                if (accumulate) solutions.Add(solution);
            }
            return solution
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        public override IEnumerable<Triple> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            PaEntry[] solution = null;
            if (use)
            {
                solution = (PaEntry[])solutions[position];
                position++;
            }
            else
            {
                solution = go.GetTriplesWithPredicateObject(((OV_iri)pred).UriString, obj).ToArray();
                if (accumulate) solutions.Add(solution);
            }
            return solution
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        public override IEnumerable<Triple> GetTriplesWithObject(ObjectVariants obj)
        {
            PaEntry[] solution = null;
            if (use)
            {
                solution = (PaEntry[])solutions[position];
                position++;
            }
            else
            {
                solution = go.GetTriplesWithObject(obj).ToArray();
                if (accumulate) solutions.Add(solution);
            }
            return solution
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
    }
}
