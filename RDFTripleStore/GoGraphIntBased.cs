using System;
using System.Collections.Generic;
using System.Linq;
using PolarDB;
using RDFCommon;
using RDFTripleStore.OVns;
using Task15UniversalIndex;

namespace RDFTripleStore
{
    public class GoGraphIntBased : IGraph
    {
        private TableView table;

        private IndexDynamic<Comparer, IndexViewImmutable<Comparer>> spo_ind;
        private IndexDynamic<Comparer, IndexViewImmutable<Comparer>> po_ind;
        private IndexDynamic<Comparer, IndexViewImmutable<Comparer>> os_ind;
        protected NodeGeneratorInt ng;
        public GoGraphIntBased(string path)
        {
            PType tp_tabelement = new PTypeRecord(
             new NamedType("subject", new PType(PTypeEnumeration.integer)),
             new NamedType("predicate", new PType(PTypeEnumeration.integer)),
             new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            ng = new NodeGeneratorInt(path+"coding");
            Func<object, Comparer> spokeyproducer = v =>
                {                   
                    object[] va = (object[])((object[])v)[1];
                    return new Comparer3((int)va[0], (int)va[1], va[2].ToOVariant().ToComparable()); //.ToComparable()
                };
            Func<object, Comparer> pokeyproducer = v =>
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

            spo_ind = new IndexDynamic<Comparer, IndexViewImmutable<Comparer>>(false)
            {
                Table = table,
                IndexArray = new IndexViewImmutable<Comparer>(path + "spo_ind")
                {
                    Table = table,
                    KeyProducer = spokeyproducer
                },
                KeyProducer = spokeyproducer
            };
            po_ind = new IndexDynamic<Comparer, IndexViewImmutable<Comparer>>(false)
            {
                Table = table,
                IndexArray = new IndexViewImmutable<Comparer>(path + "po_ind")
                {
                    Table = table,
                    KeyProducer = pokeyproducer
                },
                KeyProducer = pokeyproducer
            };
            os_ind = new IndexDynamic<Comparer, IndexViewImmutable<Comparer>>(false)
            {
                Table = table,
                IndexArray = new IndexViewImmutable<Comparer>(path + "os_ind")
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

        public void Clear()
        {
            table.Clear();       
        }

        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithObject(IObjectNode o)
        {
            var objVar = (((ObjectVariants)o));
      
            IEnumerable<PaEntry> entities = os_ind.GetAllByKey(new Comparer(objVar.ToComparable()));
            return entities.Select(ent =>
            {
                object[] three1 = (object[])(((object[])ent.Get())[1]);
                return new Triple<ISubjectNode, IPredicateNode, IObjectNode>(ng.GetCoded((int)three1[0]), ng.GetCoded((int)three1[1]), o); 

            });
        }

        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithPredicate(IPredicateNode p)
        {
            IEnumerable<PaEntry> entities = po_ind.GetAllByKey(new Comparer(((OV_iriint)p).code));
            return entities.Select(ent =>
            {
                object[] three1 = (object[])(((object[])ent.Get())[1]);
                return new Triple<ISubjectNode, IPredicateNode, IObjectNode>(ng.GetCoded((int)three1[0]), p, three1[2].ToOVariant()); 
            });
        }

        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithSubject(ISubjectNode s)
        {
            IEnumerable<PaEntry> entities = spo_ind.GetAllByKey(new Comparer(((OV_iriint)s).code));
            return entities.Select(ent =>
            {
                object[] three1 = (object[])(((object[])ent.Get())[1]);
                return new Triple<ISubjectNode, IPredicateNode, IObjectNode>(s, ng.GetCoded((int)three1[1]), three1[2].ToOVariant());
            });
        }

        public IEnumerable<IObjectNode> GetTriplesWithSubjectPredicate(ISubjectNode subject, IPredicateNode predicate)
        {
            int ssubj = ((OV_iriint)subject).code;
            int spred = ((OV_iriint)predicate).code;
          
            IEnumerable<PaEntry> entities = spo_ind.GetAllByKey(new Comparer2(ssubj, spred));
            return entities.Select(ent =>
            {
                object[] three1 = (object[])(((object[])ent.Get())[1]);
                return (IObjectNode)((object[])three1[2]).Writeble2OVariant();
            });
        }

        public IEnumerable<IPredicateNode> GetTriplesWithSubjectObject(ISubjectNode subject, IObjectNode obj)
        {
            int ssubj = (((OV_iriint)subject)).code;
            var objVar = (((ObjectVariants)obj));
            Comparer2 key_triple = new Comparer2(objVar.ToComparable(), ssubj);
            IEnumerable<PaEntry> entities = os_ind.GetAllByKey(key_triple);
            return entities.Select(ent =>
            {
                object[] three1 = (object[])(((object[])ent.Get())[1]);
                return (IPredicateNode)ng.GetCoded((int)three1[1]);
            });
        }

        public IEnumerable<ISubjectNode> GetTriplesWithPredicateObject(IPredicateNode predicate, IObjectNode obj)
        {
            int pred = (((OV_iriint)predicate)).code;
            var objVar = (((ObjectVariants)obj));
            Comparer2 key_triple = new Comparer2(pred, objVar.ToComparable());
            IEnumerable<PaEntry> entities = po_ind.GetAllByKey(key_triple);
            return entities.Select(ent =>
            {
                object[] three1 = (object[])(((object[])ent.Get())[1]);
                return (ISubjectNode)ng.GetCoded((int)three1[0]);
            });
        }

        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriples()
        {
            throw new NotImplementedException();
        }

        public void Add(ISubjectNode s, IPredicateNode p, IObjectNode o)
        {
            throw new NotImplementedException();
        }

        public void Insert(IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples)
        {
            throw new NotImplementedException();
        }

        public void Add(Triple<ISubjectNode, IPredicateNode, IObjectNode> t)
        {
            throw new NotImplementedException();
        }

        public bool Contains(ISubjectNode subject, IPredicateNode predicate, IObjectNode obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISubjectNode> GetAllSubjects()
        {
            throw new NotImplementedException();
        }

        public long GetTriplesCount()
        {
            throw new NotImplementedException();
        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public void FromTurtle(string path)
        {         
            table.Clear();
            Build(new TripleGeneratorBufferedParallel(path, "g"));
            
        //    table.Fill(ReadTripleStringsFromTurtle.LoadGraph(path).Select(tr => new object[] { tr.Subject.ToLower(), tr.Predicate.ToLower(), tr.Object.ToWritable() }));
        }

        // Структуры, играющие роль ключа
        public class TripleSPO : IComparable
        {
            public TripleSPO(string s, string p, ObjectVariants toComparable)
            {
             triple=new Triple<string, string, ObjectVariants>(s,p,toComparable);
            }

            public Triple<string, string, ObjectVariants> triple { get; set; }
            public int CompareTo(object another)
            {
                if (!(another is TripleSPO)) throw new Exception("kdjfk");
                TripleSPO ano = (TripleSPO)another;
                int cmp = triple.Subject.CompareTo(ano.triple.Subject);
                if (cmp == 0 && ano.triple.Predicate != null)
                {
                    cmp = triple.Predicate.CompareTo(ano.triple.Predicate);
                    //if (cmp == 0) cmp = triple.Object.CompareTo(ano.triple.Object);
                }
                return cmp;
            }
        }
    }
}
