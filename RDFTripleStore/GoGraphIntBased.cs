using System;
using System.Collections.Generic;
using System.Linq;
using PolarDB;
using RDFCommon;
using RDFTripleStore.Comparer;
using RDFTripleStore.OVns;
using Task15UniversalIndex;

namespace RDFTripleStore
{
    public class GoGraphIntBased : IGraph
    {
        private TableView table;

        private IndexDynamic<Comparer.Comparer, IndexViewImmutable<Comparer.Comparer>> spo_ind;
        private IndexDynamic<Comparer.Comparer, IndexViewImmutable<Comparer.Comparer>> po_ind;
        private IndexDynamic<Comparer.Comparer, IndexViewImmutable<Comparer.Comparer>> os_ind;
        protected NodeGeneratorInt ng;
        public GoGraphIntBased(string path)
        {
            PType tp_tabelement = new PTypeRecord(
             new NamedType("subject", new PType(PTypeEnumeration.integer)),
             new NamedType("predicate", new PType(PTypeEnumeration.integer)),
             new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            ng = new NodeGeneratorInt(path+"coding");
            Func<object, Comparer.Comparer> spokeyproducer = v =>
                {                   
                    object[] va = (object[])((object[])v)[1];
                    return new Comparer3((int)va[0], (int)va[1], va[2].ToOVariant().ToComparable()); //.ToComparable()
                };
            Func<object, Comparer.Comparer> pokeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new Comparer2((int)va[1], va[2].ToOVariant().ToComparable());
            };
            Func<object, Comparer2> oskeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new Comparer2(va[2].ToOVariant().ToComparable(),(int)va[0]);
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
        
            ng.Build();
            generator.Start(list =>
            {
                IEnumerable<string> ids = list.SelectMany(tri =>
                {
                    IEnumerable<string> iris = new string[] { tri.Subject, tri.Predicate };
                    if (tri.Object.Variant == ObjectVariantEnum.Iri)
                        iris = iris.Concat(new string[] { ((OV_iri)tri.Object).UriString });
                    return iris;
                });
                var dictionary = ng.coding_table.InsertPortion(ids);
                foreach (var tri in list)
                {
                    int isubj = dictionary[tri.Subject];
                    int ipred = dictionary[tri.Predicate];
                    ObjectVariants ov = tri.Object;
                    if (ov.Variant == ObjectVariantEnum.Iri)
                    {
                        int iobj = dictionary[((OV_iri)ov).UriString];
                        ov = new OV_iriint(iobj, ng.coding_table);
                    }
                    table.TableCell.Root.AppendElement(new object[] { false, new object[] { isubj, ipred, ov.ToWritable() } });
                }
                });

            spo_ind.IndexArray.Build();
            po_ind.IndexArray.Build();
            os_ind.IndexArray.Build(); spo_ind.Build();
            po_ind.Build();
            os_ind.Build();
        }

    
        public IGraphNode Name { get { return ng.CreateUriNode("g"); } }
        public INodeGenerator NodeGenerator { get { return ng; }} 

        /// <summary>
        /// Create Triple
        /// </summary>
        /// <param name="ent">массив с объектным представлением триплета</param>
        /// <param name="subject">если null или не указан, то будет вычислен из объекта ent</param>
        /// <param name="predicate">если null или не указан, то будет вычислен из объекта ent</param>
        /// <param name="objectNode">если null или не указан, то будет вычислен из объекта ent</param>
        private Triple<ISubjectNode, IPredicateNode, IObjectNode> CTle(object[] row, ISubjectNode subject = null,
            IPredicateNode predicate = null, IObjectNode objectNode = null)
        {
            return new Triple<ISubjectNode, IPredicateNode, IObjectNode>(subject ?? ng.GetCoded((int)row[0])
                , predicate ?? ng.GetCoded((int)row[1])
                , objectNode ?? row[2].ToOVariant());
        }



        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithObject(IObjectNode o)
        {
            return os_ind.GetAllByKey(new Comparer.Comparer(((ObjectVariants)o).ToComparable()))
                .Select(entry => entry.Get())
                .ReadWritableTriples()
                .Select(ent => CTle(ent, objectNode: o));
        }



        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithPredicate(IPredicateNode p)
        {
            return po_ind.GetAllByKey(new Comparer.Comparer((((OV_iriint)p)).code))
                   .Select(entry => entry.Get())
                .ReadWritableTriples()
                .Select(ent => CTle(ent, predicate: p));
        }

        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithSubject(ISubjectNode s)
        {
            return spo_ind.GetAllByKey(new Comparer.Comparer((((OV_iriint)s)).code))
                 .Select(entry => entry.Get())
                .ReadWritableTriples()
                                .Select(ent => CTle(ent, subject: s));
        }

        public IEnumerable<IObjectNode> GetTriplesWithSubjectPredicate(ISubjectNode subject, IPredicateNode predicate)
        {
            IEnumerable<PaEntry> entities = spo_ind.GetAllByKey(new Comparer2((((OV_iriint)subject)).code, (((OV_iriint)predicate)).code));
            return entities
                 .Select(entry => entry.Get())
                .ReadWritableTriples()
                .Select(row => row[2].ToOVariant());
        }

        public IEnumerable<IPredicateNode> GetTriplesWithSubjectObject(ISubjectNode subject, IObjectNode obj)
        {
            int ssubj = (((OV_iriint)subject)).code;
            var objVar = (((ObjectVariants)obj));
            Comparer2 key_triple = new Comparer2(objVar.ToComparable(), ssubj);
            IEnumerable<PaEntry> entities = os_ind.GetAllByKey(key_triple);
            return entities
                .Select(entry => entry.Get())
                .ReadWritableTriples()
                .Select(row => ng.GetCoded((int)row[1]));
        }

        public IEnumerable<ISubjectNode> GetTriplesWithPredicateObject(IPredicateNode predicate, IObjectNode obj)
        {
            int pred = (((OV_iriint)predicate)).code;
            var objVar = (((ObjectVariants)obj));
            Comparer2 key_triple = new Comparer2(pred, objVar.ToComparable());
            IEnumerable<PaEntry> entities = po_ind.GetAllByKey(key_triple);
            return entities
                  .Select(entry => entry.Get())
                  .ReadWritableTriples()
                  .Select(row => ng.GetCoded((int)row[0]));
        }


        public bool Contains(ISubjectNode subject, IPredicateNode predicate, IObjectNode obj)
        {
            int ssubj = (((OV_iriint)subject)).code;
            int pred = (((OV_iriint)predicate)).code;
            var objVar = (((ObjectVariants)obj));
            Comparer3 key_triple = new Comparer3(ssubj, pred, objVar.ToComparable());
            IEnumerable<PaEntry> entities = spo_ind.GetAllByKey(key_triple);
            return entities
                  .Select(entry => entry.Get())
                  .ReadWritableTriples()
                  .Any();
        }
        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriples()
        {
            return table.TableCell.Root.ElementValues()
                .ReadWritableTriples()
                .Select(ent => CTle(ent));
        }

        public void Add(ISubjectNode s, IPredicateNode p, IObjectNode o)
        {
            table.AppendValue(new object[] { ((OV_iriint)s).code, ((OV_iriint)p).code, ((ObjectVariants)o).WritableValue });
        }
        public void Clear()
        {
            table.Clear();
        }
        public void Insert(IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples)
        {
            foreach (var triple in triples)
                Add(triple);
        }

        public void Add(Triple<ISubjectNode, IPredicateNode, IObjectNode> t)
        {
            Add(t.Subject, t.Predicate, t.Object);
        }



        public void Delete(IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples)
        {
            foreach (var triple in triples)
            {
                int ssubj = (((OV_iriint)triple.Subject)).code;
                int pred = (((OV_iriint)triple.Predicate)).code;
                var objVar = (((ObjectVariants)triple.Object));
                Comparer3 key_triple = new Comparer3(ssubj, pred, objVar.ToComparable());
                IEnumerable<PaEntry> entities = spo_ind.GetAllByKey(key_triple);
                foreach (var ent in entities)
                    table.DeleteEntry(ent);
            }
        }

        public IEnumerable<ISubjectNode> GetAllSubjects()
        {
            return new HashSet<int>(table.TableCell.Root.ElementValues()
                .ReadWritableTriples()
                .Select(t => t[0])
                .Cast<int>())
                .Select(s => ng.GetCoded(s)); 
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
