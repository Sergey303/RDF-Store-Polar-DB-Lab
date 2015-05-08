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
        public static void Main3()
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

            var fl = g.GetTriplesWithSubjectPredicate(
                "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/datafromproducer1/product12",
                "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productpropertynumeric1");
            int n = fl.Count();
            Console.WriteLine("n={0}", n);

            sw.Stop();
            var qu = Query6t(gra);
            int cnt = qu.Count();
            Console.WriteLine("query6t ok. cnt={0}", cnt);
            
                    
        }
        public static IEnumerable<TPack> Query6t(IGraph g)
        {
            ObjectVariants[] row = new ObjectVariants[7];
            int _prodFeature = 0, _produc = 1, _productLabel = 2, _origProperty1 = 3, _simProperty1 = 4;
            int _origProperty2 = 5, _simProperty2 = 6;
            var quer = Enumerable.Repeat<TPack>(new TPack(row, g), 1)
                .spo(new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/datafromproducer1/product12"),
                    new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productpropertynumeric1"),
                    new OV_index(_origProperty1))
                //.spD("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/datafromproducer1/product12",
                //    "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productpropertynumeric2",
                //    _origProperty2)
                //.spO("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/datafromproducer1/product12",
                //    "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productfeature", _prodFeature)
                //.Spo(_produc, bsbm + "productfeature", _prodFeature)
                //.Where(pack => pack.Val(_produc) != "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/datafromproducer1/product12")
                //.spD(_produc, rdfs + "label", _productLabel)
                //.spD(_produc, "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/productpropertynumeric1",
                //    _simProperty1)
                //.Where(pack => pack.Vai(_simProperty1) < (pack.Vai(_origProperty1) + 120) &&
                //    pack.Vai(_simProperty1) > (pack.Vai(_origProperty1) - 120))
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
        private IGra<PaEntry> go;
        public TPackGraph(IGra<PaEntry>go)
        {
            this.go = go;
        }
        public bool Contains(ObjectVariants subj, ObjectVariants pred, ObjectVariants obj)
        {
            return go.Contains(subj.WritableValue, pred.WritableValue, obj);
        }
        public IEnumerable<Triple> GetTriples()
        {
            return go.GetTriples().Select(ent => 
            {
                object[] va = go.Dereference(ent);
                return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
            });
        }
        public IEnumerable<Triple> GetTriplesWithSubject(ObjectVariants subj)
        {
            return go.GetTriplesWithSubject(((OV_iri)subj).UriString)
                .Select(ent => 
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        public IEnumerable<Triple> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            var qq = go.GetTriplesWithSubjectPredicate(((OV_iri)subj).UriString, ((OV_iri)pred).UriString).ToArray();
            return go.GetTriplesWithSubjectPredicate(((OV_iri)subj).UriString, ((OV_iri)pred).UriString)
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        public IEnumerable<Triple> GetTriplesWithPredicate(ObjectVariants pred)
        {
            return go.GetTriplesWithPredicate(((OV_iri)pred).UriString)
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        public IEnumerable<Triple> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            return go.GetTriplesWithPredicateObject(((OV_iri)pred).UriString, obj)
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
        public IEnumerable<Triple> GetTriplesWithObject(ObjectVariants obj)
        {
            return go.GetTriplesWithObject(obj)
                .Select(ent =>
                {
                    object[] va = go.Dereference(ent);
                    return new Triple(new OV_iri((string)va[0]), new OV_iri((string)va[1]), va[2].ToOVariant());
                });
        }
    }
}
