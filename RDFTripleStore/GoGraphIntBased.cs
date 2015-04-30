﻿using System;
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

        private IndexDynamic<SPO_Troyka, IndexHalfkeyImmutable<SPO_Troyka>> spo_ind;
        private IndexDynamic<PO_Pair, IndexHalfkeyImmutable<PO_Pair>> po_ind;
        private IndexDynamic<PO_Pair, IndexHalfkeyImmutable<PO_Pair>> so_ind;
        private IndexDynamic<SP_Pair, IndexHalfkeyImmutable<SP_Pair>> sp_ind;
        private IndexDynamic<int, IndexKeyImmutable<int>> s_ind;
        private IndexDynamic<int, IndexKeyImmutable<int>> p_ind;
        private IndexDynamic<Comparer.Comparer, IndexViewImmutable<Comparer.Comparer>> o_ind;
        protected NodeGeneratorInt ng;
        public GoGraphIntBased(string path)
        {
            PType tp_tabelement = new PTypeRecord(
             new NamedType("subject", new PType(PTypeEnumeration.integer)),
             new NamedType("predicate", new PType(PTypeEnumeration.integer)),
             new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            ng = new NodeGeneratorInt(path+"coding");
            Func<object, SPO_Troyka> spokeyproducer = v =>
                {                   
                    object[] va = (object[])((object[])v)[1];
                    return new SPO_Troyka((int)va[0], (int)va[1], va[2].ToOVariant(ng.coding_table)); //.ToComparable()
                };
            Func<object, PO_Pair> pokeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new PO_Pair((int)va[1], va[2].ToOVariant(ng.coding_table));
            };
            Func<object, PO_Pair> oskeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new PO_Pair((int)va[0], va[2].ToOVariant(ng.coding_table));
            };
            Func<object, SP_Pair> spkeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new SP_Pair((int)va[0], (int) va[1]);
            };     
             
            Func<object, int> skeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return (int) va[0];
            };
            Func<object, int> pkeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return (int)va[1];
            };
            Func<object, Comparer.Comparer> okeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return va[2].ToOVariant(ng.coding_table).ToComparable();
            };     
            // Опорная таблица
            table = new TableView(path + "stable", tp_tabelement);
            // Индекс spo

            spo_ind = new IndexDynamic<SPO_Troyka, IndexHalfkeyImmutable<SPO_Troyka>>(false)
            {
                Table = table,
                IndexArray = new IndexHalfkeyImmutable<SPO_Troyka>(path + "spo_ind")
                {
                    Table = table,
                    KeyProducer = spokeyproducer  ,
                    HalfProducer = sp => sp.GetHashCode()
                },
                KeyProducer = spokeyproducer ,
            };
            po_ind = new IndexDynamic<PO_Pair, IndexHalfkeyImmutable<PO_Pair>>(false)
            {
                Table = table,
                IndexArray = new IndexHalfkeyImmutable<PO_Pair>(path + "po_ind")
                {
                    Table = table,
                    KeyProducer = pokeyproducer  ,
                    HalfProducer = sp => sp.GetHashCode()
                },
                KeyProducer = pokeyproducer
            };
            so_ind = new IndexDynamic<PO_Pair, IndexHalfkeyImmutable<PO_Pair>>(false)
            {
                Table = table,
                IndexArray = new IndexHalfkeyImmutable<PO_Pair>(path + "so_ind")
                {
                    Table = table,
                    KeyProducer = oskeyproducer,
                    HalfProducer = sp => sp.GetHashCode()    
                },
                KeyProducer = pokeyproducer
            };
            sp_ind = new IndexDynamic<SP_Pair, IndexHalfkeyImmutable<SP_Pair>>(false)
            {
                Table = table,
                IndexArray = new IndexHalfkeyImmutable<SP_Pair>(path + "sp_ind")
                {
                    Table = table,
                    KeyProducer = spkeyproducer,
                    HalfProducer = sp => sp.GetHashCode()
                },
                KeyProducer = spkeyproducer
            };
            s_ind = new IndexDynamic<int, IndexKeyImmutable<int>>(false)
            {
                Table = table,
                IndexArray = new IndexKeyImmutable<int>(path + "s_ind")
                {
                    Table = table,
                    KeyProducer = skeyproducer,
                },
                KeyProducer = skeyproducer
            };
            p_ind = new IndexDynamic<int, IndexKeyImmutable<int>>(false)
            {
                Table = table,
                IndexArray = new IndexKeyImmutable<int>(path + "p_ind")
                {
                    Table = table,
                    KeyProducer = pkeyproducer,
                },
                KeyProducer = pkeyproducer
            };
            o_ind = new IndexDynamic<Comparer.Comparer, IndexViewImmutable<Comparer.Comparer>>(false)
            {
                Table = table,
                IndexArray = new IndexViewImmutable<Comparer.Comparer>(path + "o_ind")
                {
                    Table = table,                                             
                    KeyProducer = okeyproducer,
                },
                KeyProducer = okeyproducer
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
                    if (tri.Object.Variant == ObjectVariantEnum.OtherIntType)
                        iris = iris.Concat(new string[] { ((OV_typed)tri.Object). DataType});
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
               
            ng.coding_table.BuildScale();
            table.TableCell.Flush();
            spo_ind.IndexArray.Build();
            po_ind.IndexArray.Build();
            so_ind.IndexArray.Build();
            sp_ind.IndexArray.Build();
            s_ind.IndexArray.Build();
            p_ind.IndexArray.Build();
            o_ind.IndexArray.Build();
            spo_ind.Build();
            po_ind.Build();
            so_ind.Build();
            sp_ind.Build();
            s_ind.Build();
            p_ind.Build();
            o_ind.Build();
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
                , objectNode ?? row[2].ToOVariant(ng.coding_table));
        }



        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithObject(IObjectNode o)
        {                                                
            return o_ind.GetAllByKey(((ObjectVariants)o).ToComparable())
                .Select(entry => entry.Get())
                .ReadWritableTriples()
                .Select(ent => CTle(ent, objectNode: o));
        }



        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithPredicate(IPredicateNode p)
        {
            return p_ind.GetAllByKey(((OV_iriint) p).code)
                .Select(entry => entry.Get())
                .ReadWritableTriples()
                .Select(ent => CTle(ent, predicate: p));
        }

        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithSubject(ISubjectNode s)
        {
            return s_ind.GetAllByKey(((OV_iriint) s).code)
                .Select(entry => entry.Get())
                .ReadWritableTriples()
                .Select(ent => CTle(ent, subject: s));
        }

        public IEnumerable<IObjectNode> GetTriplesWithSubjectPredicate(ISubjectNode subject, IPredicateNode predicate)
        {
            IEnumerable<PaEntry> entities = sp_ind.GetAllByKey(new SP_Pair(((OV_iriint)subject).code, ((OV_iriint)predicate).code));
            return entities
                 .Select(entry => entry.Get())
                .ReadWritableTriples()
                .Select(row => row[2].ToOVariant(ng.coding_table));
        }

        public IEnumerable<IPredicateNode> GetTriplesWithSubjectObject(ISubjectNode subject, IObjectNode obj)
        {
            int ssubj = (((OV_iriint)subject)).code;
            var objVar = (((ObjectVariants)obj));
            var key_triple = new PO_Pair(ssubj, objVar);
            IEnumerable<PaEntry> entities = so_ind.GetAllByKey(key_triple);
            return entities
                .Select(entry => entry.Get())
                .ReadWritableTriples()
                .Select(row => ng.GetCoded((int)row[1]));
        }

        public IEnumerable<ISubjectNode> GetTriplesWithPredicateObject(IPredicateNode predicate, IObjectNode obj)
        {
            int pred = (((OV_iriint)predicate)).code;
            var objVar = (((ObjectVariants)obj));
            var key_triple = new PO_Pair(pred, objVar);
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
            var key_triple = new SPO_Troyka(ssubj, pred, objVar);
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
                var key_triple = new SPO_Troyka(ssubj, pred, objVar);
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
        }

    }
    public class SP_Pair : IComparable
    {
        int s, p;
        public SP_Pair(int subject, int predicate) { this.s = subject; this.p = predicate; }
        //int S { get; set; }
        //int P { get; set; }
        public int CompareTo(object another)
        {
            SP_Pair ano = (SP_Pair)another;
            int cmp = this.GetHashCode().CompareTo(ano.GetHashCode());
            if (cmp == 0)
            {
                cmp = this.s.CompareTo(ano.s);
            }
            if (cmp == 0)
            {
                cmp = this.p.CompareTo(ano.p);
            }
            return cmp;
        }
        public override int GetHashCode()
        {
            //return s.GetHashCode() ^ p.GetHashCode();
            return (3001 ^ s.GetHashCode()) * (1409 ^ p.GetHashCode());
        }
    }
    public class SPO_Troyka : IComparable
    {
        int s, p; ObjectVariants ov;
        public SPO_Troyka(int subject, int predicate, ObjectVariants ov) { this.s = subject; this.p = predicate; this.ov = ov; }
        public int CompareTo(object another)
        {
            SPO_Troyka ano = (SPO_Troyka)another;
            int cmp = this.GetHashCode().CompareTo(ano.GetHashCode());
            if (cmp == 0)
            {
                cmp = this.s.CompareTo(ano.s);
            }
            if (cmp == 0 )
            {
                cmp = this.p.CompareTo(ano.p);
            }
            if (cmp == 0 )
            {
                cmp = this.ov.ToComparable().CompareTo(ano.ov.ToComparable());
            }
            return cmp;
        }
        public override int GetHashCode()
        {
            return (2 ^ s.GetHashCode()) * (3 ^ p.GetHashCode()) * (7 ^ ov.GetHashCode());
        }
    }
    public class PO_Pair : IComparable
    {
        int p; ObjectVariants ov;
        public PO_Pair(int predicate, ObjectVariants ov) { this.p = predicate; this.ov = ov; }
        public int CompareTo(object another)
        {
            PO_Pair ano = (PO_Pair)another;
            int cmp = this.GetHashCode().CompareTo(ano.GetHashCode());
            if (cmp == 0)
            {
                cmp = this.p.CompareTo(ano.p);
            }
            if (cmp == 0 )
            {
                cmp = this.ov.ToComparable().CompareTo(ano.ov.ToComparable());
            }
            return cmp;
        }
        public override int GetHashCode()
        {
            //return p.GetHashCode() + 7777 * ov.GetHashCode();
            return unchecked((2 ^ p.GetHashCode()) * (3 ^ ov.GetHashCode()));
        }
    }
}