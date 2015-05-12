using System;
using System.Collections.Generic;
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
            bool toload = true;
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
            int cnt;

            sw.Restart();
            qu = Query6t(gra);
            cnt = qu.Count();
            sw.Stop();
            Console.WriteLine("query6t ok. cnt={0} duration={1}", cnt, sw.ElapsedMilliseconds);

            //TPackGraph_Diag grad = new TPackGraph_Diag(g);

            //grad.StartAccumulate();
            //sw.Restart();
            //qu = Query6t(grad);
            //cnt = qu.Count();
            //sw.Stop();
            //Console.WriteLine("query6t ok. cnt={0} duration={1}", cnt, sw.ElapsedMilliseconds);

            //grad.StartUse();
            //sw.Restart();
            //qu = Query6t(grad);
            //cnt = qu.Count();
            //sw.Stop();
            //Console.WriteLine("query6t ok. cnt={0} duration={1}", cnt, sw.ElapsedMilliseconds);
                    
        }
        public static IEnumerable<TPack> Query6t(IGraph g)
        {
            ObjectVariants[] row = new ObjectVariants[7];
            ObjectVariants _prodFeature = new OV_index(0), _produc = new OV_index(1), _productLabel = new OV_index(2), _origProperty1 = new OV_index(3), _simProperty1 = new OV_index(4);
            int _origProperty2 = 5, _simProperty2 = 6;
            ObjectVariants iri1 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer1/Product12"),
                iri2 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productFeature"),
                iri3 = new OV_iri("http://www.w3.org/2000/01/rdf-schema#label"),
                iri4 = new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productPropertyNumeric1");
            var quer = Enumerable.Repeat<TPack>(new TPack(row, g), 1)
                .spo(iri1, iri2, _prodFeature)
                .spo(_produc, iri2, _prodFeature)
                .Where(pack => ! pack.Get(_produc).Equals(iri1))
                .spo(_produc, iri3, _productLabel)
                .spo(iri1, iri4, _origProperty1)
                .spo(_produc, iri4, _simProperty1)
                //.Where(pack => 
                //{
                //    int sp1 = ((OV_int)pack.Get(_simProperty1)).value;
                //    int op1 = ((OV_int)pack.Get(_origProperty1)).value;
                //    return sp1 < (op1 + 120) && sp1 > (op1 - 120);
                //})

                //.spD("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/datafromproducer1/product12",
                //    "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productpropertynumeric2",
                //    _origProperty2)
                //.spD(_produc, "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productpropertynumeric2",
                //    _simProperty2)
                //.Where(pack => pack.Vai(_simProperty2) < (pack.Vai(_origProperty2) + 170) &&
                //    pack.Vai(_simProperty2) > (pack.Vai(_origProperty2) - 170))
                ;
            return quer;
        }
    }

    public class TPackGraph : IGraph
    {
        protected IGra<PaEntry> go;
        public TPackGraph(IGra<PaEntry>go)
        {
            this.go = go;
        }
        public virtual bool Contains(ObjectVariants subj, ObjectVariants pred, ObjectVariants obj)
        {
            return go.Contains(subj.WritableValue, pred.WritableValue, obj);
        }
        public virtual IEnumerable<Triple> GetTriples()
        {
            return go.GetTriples().Select(ent => 
            {
                object[] va = go.Dereference(ent);
                return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
            });
        }
        public virtual IEnumerable<Triple> GetTriplesWithSubject(ObjectVariants subj)
        {
            return go.GetTriplesWithSubject(((OV_iri)subj).UriString)
                .Select(ent => 
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        public virtual IEnumerable<Triple> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            //var qq = go.GetTriplesWithSubjectPredicate(((OV_iri)subj).UriString, ((OV_iri)pred).UriString).ToArray();
            return go.GetTriplesWithSubjectPredicate(((OV_iri)subj).UriString, ((OV_iri)pred).UriString)
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        public virtual IEnumerable<Triple> GetTriplesWithPredicate(ObjectVariants pred)
        {
            return go.GetTriplesWithPredicate(((OV_iri)pred).UriString)
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        public virtual IEnumerable<Triple> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            return go.GetTriplesWithPredicateObject(((OV_iri)pred).UriString, obj)
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        public virtual IEnumerable<Triple> GetTriplesWithObject(ObjectVariants obj)
        {
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
