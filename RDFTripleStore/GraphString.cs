using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PolarDB;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using RDFTripleStore.Comparer;
using RDFTripleStore.parsers;
using RDFTripleStore.parsers.RDFTurtle;
using Task15UniversalIndex;

namespace RDFTripleStore
{
    public class GraphString// : IGraph
    {
        private TableView table;

        private IndexDynamic<Comparer.Comparer, IndexViewImmutable<Comparer.Comparer>> spo_ind;
        private IndexDynamic<Comparer.Comparer, IndexViewImmutable<Comparer.Comparer>> po_ind;
        private IndexDynamic<Comparer.Comparer, IndexViewImmutable<Comparer.Comparer>> os_ind;
        protected NodeGenerator ng = new NodeGenerator();
        public GraphString(string path)
        {
            PType tp_tabelement = new PTypeRecord(
                new NamedType("subject", new PType(PTypeEnumeration.sstring)),
                new NamedType("predicate", new PType(PTypeEnumeration.sstring)),
                new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            Func<object, Comparer.Comparer> spokeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new Comparer3((string)va[0], (string)va[1], va[2].ToOVariant()); //.ToComparable()
            };
            Func<object, Comparer.Comparer> pokeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new Comparer2((string)va[1], va[2].ToOVariant());
            };
            Func<object, Comparer2> oskeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new Comparer2(va[2].ToOVariant(), (string)va[0]);
            };
            // Опорная таблица
            table = new TableView(path + "stable", tp_tabelement);
            // Индекс spo

            spo_ind = new IndexDynamic<Comparer.Comparer, IndexViewImmutable<Comparer.Comparer>>(false)
            {
                Table = table,
                IndexArray = new IndexViewImmutable<Comparer.Comparer>(path + "spo_ind")
                {
                    Table = table,
                    KeyProducer = spokeyproducer
                },
                KeyProducer = spokeyproducer
            };
            po_ind = new IndexDynamic<Comparer.Comparer, IndexViewImmutable<Comparer.Comparer>>(false)
            {
                Table = table,
                IndexArray = new IndexViewImmutable<Comparer.Comparer>(path + "po_ind")
                {
                    Table = table,
                    KeyProducer = pokeyproducer
                },
                KeyProducer = pokeyproducer
            };
            os_ind = new IndexDynamic<Comparer.Comparer, IndexViewImmutable<Comparer.Comparer>>(false)
            {
                Table = table,
                IndexArray = new IndexViewImmutable<Comparer.Comparer>(path + "os_ind")
                {
                    Table = table,
                    KeyProducer = oskeyproducer
                },
                KeyProducer = pokeyproducer
            };

        }

        public void Build(IGenerator<List<Triple<string, string, ObjectVariants>>> generator)
        {
            table.Clear();
            table.Fill(new object[0]);
            generator.Start(list =>
            {
                foreach (var tr in list)
                    table.AppendValue(new object[] { tr.Subject, tr.Predicate, tr.Object.ToWritable() });
            });

            spo_ind.IndexArray.Build();
            po_ind.IndexArray.Build();
            os_ind.IndexArray.Build();
        }


        public ObjectVariants Name { get { return ng.CreateUriNode("g"); } }
        public INodeGenerator NodeGenerator { get { return ng; } }

        /// <summary>
        /// Create Triple
        /// </summary>
        /// <param name="ent">массив с объектным представлением триплета</param>
        /// <param name="subject">если null или не указан, то будет вычислен из объекта ent</param>
        /// <param name="predicate">если null или не указан, то будет вычислен из объекта ent</param>
        /// <param name="objectNode">если null или не указан, то будет вычислен из объекта ent</param>
        private Triple<ObjectVariants, ObjectVariants, ObjectVariants> CTle(object[] row, ObjectVariants subject = null,
            ObjectVariants predicate = null, ObjectVariants objectNode = null)
        { 
            return new Triple<ObjectVariants, ObjectVariants, ObjectVariants>(subject ?? ng.GetUri((string) row[0])
                , predicate ?? ng.GetUri((string) row[1])
                , objectNode ?? row[2].ToOVariant());
        }

    

        public IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> GetTriplesWithObject(ObjectVariants o)
        {
            return os_ind.GetAllByKey(new Comparer.Comparer(((ObjectVariants)o)))
                .Select(entry => entry.Get())
                .ReadWritableTriples()
                .Select(ent => CTle(ent, objectNode: o));
        }

     

        public IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> GetTriplesWithPredicate(ObjectVariants p)
        {
            return po_ind.GetAllByKey(new Comparer.Comparer((((IIriNode)p)).UriString))
                   .Select(entry => entry.Get())
                .ReadWritableTriples()
                .Select(ent => CTle(ent, predicate: p));
        }

        public IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> GetTriplesWithSubject(ObjectVariants s)
        {
            var count = table.Count();
            Console.WriteLine(count);
            return spo_ind.GetAllByKey(new Comparer.Comparer((((IIriNode)s)).UriString))
                 .Select(entry => entry.Get())
                .ReadWritableTriples()
                                .Select(ent => CTle(ent, subject: s));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subject, ObjectVariants predicate)
        {
            IEnumerable<PaEntry> entities = spo_ind.GetAllByKey(new Comparer2((((IIriNode)subject)).UriString, (((IIriNode)predicate)).UriString));
            return entities
                 .Select(entry => entry.Get())
                .ReadWritableTriples()
                .Select(row => row[2].ToOVariant());
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subject, ObjectVariants obj)
        {
            string ssubj = (((IIriNode)subject)).UriString;
            var objVar = (((ObjectVariants)obj));
            Comparer2 key_triple = new Comparer2(objVar, ssubj);
            IEnumerable<PaEntry> entities = os_ind.GetAllByKey(key_triple);
            return entities
                .Select(entry => entry.Get())
                .ReadWritableTriples()
                .Select(row => ng.CreateUriNode((string)row[1]));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants predicate, ObjectVariants obj)
        {
            string pred = (((IIriNode)predicate)).UriString;
            var objVar = (((ObjectVariants)obj));
            Comparer2 key_triple = new Comparer2(pred, objVar);
            IEnumerable<PaEntry> entities = po_ind.GetAllByKey(key_triple);
            return entities
                  .Select(entry => entry.Get())
                  .ReadWritableTriples()
                  .Select(row => ng.CreateUriNode((string)row[0]));
        }

    
        public bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            string ssubj = (((IIriNode)subject)).UriString;
            string pred = (((IIriNode)predicate)).UriString;
            var objVar = (((ObjectVariants)obj));
            Comparer3 key_triple = new Comparer3(ssubj, pred, objVar);
            IEnumerable<PaEntry> entities = spo_ind.GetAllByKey(key_triple);
            return entities
                  .Select(entry => entry.Get())
                  .ReadWritableTriples()
                  .Any();
        }
        public IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> GetTriples()
        {
            return table.TableCell.Root.ElementValues()
                .ReadWritableTriples()
                .Select(ent => CTle(ent));
        }

        public void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            table.AppendValue(new object[] { ((OV_iriint)s).code, ((OV_iriint)p).code, (( ObjectVariants )o).ToWritable() });
        }
        public void Clear()
        {
            table.Clear();
        }
        public void Insert(IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> triples)
        {
            foreach (var triple in triples)
                Add(triple);
        }

        public void Add(Triple<ObjectVariants, ObjectVariants, ObjectVariants> t)
        {
            Add(t.Subject, t.Predicate, t.Object);
        }

     

        public void Delete(IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> triples)
        {
            foreach (var triple in triples)
            {
                string ssubj = (((IIriNode)triple.Subject)).UriString;
                string pred = (((IIriNode)triple.Predicate)).UriString;
                var objVar = (((ObjectVariants)triple.Object));
                Comparer3 key_triple = new Comparer3(ssubj, pred, objVar);
                IEnumerable<PaEntry> entities = spo_ind.GetAllByKey(key_triple);
                foreach (var ent in entities)
                    table.DeleteEntry(ent);
            }                          
        }

        public IEnumerable<ObjectVariants> GetAllSubjects()
        {
            return new HashSet<string>(table.TableCell.Root.ElementValues()
                .ReadWritableTriples()
                .Select(t => t[0])
                .Cast<string>())
                .Select(s => ng.GetUri(s));


        }

        public long GetTriplesCount()
        {
            return table.Count();
        }

        public bool Any()
        {
            return table.Count() > 0;
        }

        public void FromTurtle(string path)
        {
            table.Clear();
            Build(new TripleGeneratorBufferedParallel(path, "g"));
            //    table.Fill(ReadTripleStringsFromTurtle.LoadGraph(path).Select(tr => new object[] { tr.Subject.ToLower(), tr.Predicate.ToLower(), tr.Object.ToWritable() }));
            spo_ind.Build();
            po_ind.Build();
            os_ind.Build();
        }

   
    }
}
