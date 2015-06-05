using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using Task15UniversalIndex;
using PolarDB;


namespace GoTripleStore
{
    public class GaGraphStringBased : IGra<PaEntry> // IGraph<Triple<string, string, ObjectVariants>>
    {
        private TableView table;
        // Универсальные индексы
        private IndexViewImmutable<TripleSPOu> spo_ind_arr;
        private IndexDynamic<TripleSPOu, IndexViewImmutable<TripleSPOu>> spo_ind;
        private IndexViewImmutable<DuplePOu> po_ind_arr;
        private IndexDynamic<DuplePOu, IndexViewImmutable<DuplePOu>> po_ind;
        private IndexViewImmutable<MonopleOu> o_ind_arr;
        private IndexDynamic<MonopleOu, IndexViewImmutable<MonopleOu>> o_ind;
        
        private IndexHalfkeyImmutable<DupleSP> sp_index_arr;
        private IndexDynamic<DupleSP, IndexHalfkeyImmutable<DupleSP>> sp_index;

        private IndexHalfkeyImmutable<TripleSPO> spo_index_arr;
        private IndexDynamic<TripleSPO, IndexHalfkeyImmutable<TripleSPO>> spo_index;

        //// Забракованный вариант, остается в комментариях
        //private IndexHalfkeyImmutable<DuplePO> po_index_arr;
        //private IndexDynamic<DuplePO, IndexHalfkeyImmutable<DuplePO>> po_index;

        //// Отвергнутый вариант индекса по s с полуключем
        //private IndexHalfkeyImmutable<MonopleS> s_index_arr;
        //private IndexDynamic<MonopleS, IndexHalfkeyImmutable<MonopleS>> s_index;

        public GaGraphStringBased(string path)
        {
            PType tp_tabelement = new PTypeRecord(
                new NamedType("subject", new PType(PTypeEnumeration.sstring)),
                new NamedType("predicate", new PType(PTypeEnumeration.sstring)),
                new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            Func<object, TripleSPOu> uSPOkeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new TripleSPOu()
                {
                    triple = new Tuple<string, string, ObjectVariants>((string)va[0], (string)va[1],
                        ((object[])va[2]).Writeble2OVariant())
                };
            };
            Func<object, DuplePOu> uPOkeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new DuplePOu()
                {
                    tuple = new Tuple<string, ObjectVariants>((string)va[1],
                        ((object[])va[2]).Writeble2OVariant())
                };
            };
            Func<object, MonopleOu> uOkeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new MonopleOu()
                {
                    tuple = new Tuple<ObjectVariants>(((object[])va[2]).Writeble2OVariant())
                };
            };
            // Опорная таблица
            table = new TableView(path + "stable", tp_tabelement);
            // Индекс spo
            spo_ind_arr = new IndexViewImmutable<TripleSPOu>(path + "spo_ind")
            {
                Table = table,
                KeyProducer = uSPOkeyproducer
            };
            spo_ind = new IndexDynamic<TripleSPOu, IndexViewImmutable<TripleSPOu>>(false)
            {
                Table = table,
                IndexArray = spo_ind_arr,
                KeyProducer = uSPOkeyproducer
            };

            // Индекс po
            po_ind_arr = new IndexViewImmutable<DuplePOu>(path + "po_ind")
            {
                Table = table,
                KeyProducer = uPOkeyproducer
            };
            po_ind = new IndexDynamic<DuplePOu, IndexViewImmutable<DuplePOu>>(false)
            {
                Table = table,
                IndexArray = po_ind_arr,
                KeyProducer = uPOkeyproducer
            };
            // Индекс o
            o_ind_arr = new IndexViewImmutable<MonopleOu>(path + "o_ind")
            {
                Table = table,
                KeyProducer = uOkeyproducer
            };
            o_ind = new IndexDynamic<MonopleOu, IndexViewImmutable<MonopleOu>>(false)
            {
                Table = table,
                IndexArray = o_ind_arr,
                KeyProducer = uOkeyproducer
            };

            //// Отвергнуты вариант индекса по s с полуключем
            //// Индекс s
            //Func<object, MonopleS> Skeyproducer = v =>
            //{
            //    object[] va = (object[])((object[])v)[1];
            //    return new MonopleS()
            //    {
            //        tuple = new Tuple<string>((string)va[0])
            //    };
            //};
            //s_index_arr = new IndexHalfkeyImmutable<MonopleS>(path + "s_index")
            //{
            //    Table = table,
            //    KeyProducer = Skeyproducer,
            //    HalfProducer = s => s.GetHashCode(),
            //    Scale = null
            //};
            //s_index_arr.Scale = new ScaleCell(path + "s_index") { IndexCell = s_index_arr.IndexCell };
            //s_index = new IndexDynamic<MonopleS, IndexHalfkeyImmutable<MonopleS>>(false)
            //{
            //    Table = table,
            //    IndexArray = s_index_arr,
            //    KeyProducer = Skeyproducer
            //};

            // Индекс sp
            Func<object, DupleSP> SPkeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new DupleSP()
                {
                    tuple = new Tuple<string, string>((string)va[0], (string)va[1])
                };
            };
            sp_index_arr = new IndexHalfkeyImmutable<DupleSP>(path + "sp_index")
            {
                Table = table,
                KeyProducer = SPkeyproducer,
                HalfProducer = sp => sp.GetHashCode(),
                Scale = null
            };
            sp_index_arr.Scale = new ScaleCell(path + "sp_index") { IndexCell = sp_index_arr.IndexCell };
            sp_index = new IndexDynamic<DupleSP, IndexHalfkeyImmutable<DupleSP>>(false)
            {
                Table = table,
                IndexArray = sp_index_arr,
                KeyProducer = SPkeyproducer
            };

            // Индекс spo
            Func<object, TripleSPO> SPOkeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new TripleSPO()
                {
                    tuple = new Tuple<string, string, ObjectVariants>((string)va[0], (string)va[1], (ObjectVariants)va[2].ToOVariant())
                };
            };
            spo_index_arr = new IndexHalfkeyImmutable<TripleSPO>(path + "spo_index")
            {
                Table = table,
                KeyProducer = SPOkeyproducer,
                HalfProducer = sp => sp.GetHashCode(),
                Scale = null
            };
            spo_index_arr.Scale = new ScaleCell(path + "spo_index") { IndexCell = spo_index_arr.IndexCell };
            spo_index = new IndexDynamic<TripleSPO, IndexHalfkeyImmutable<TripleSPO>>(false)
            {
                Table = table,
                IndexArray = spo_index_arr,
                KeyProducer = SPOkeyproducer
            };

            //// Индекс po - вариант с полуключем
            //Func<object, DuplePO> POkeyproducer = v =>
            //{
            //    object[] va = (object[])((object[])v)[1];
            //    return new DuplePO()
            //    {
            //        tuple = new Tuple<string, ObjectVariants>((string)va[1], (ObjectVariants)va[2].ToOVariant())
            //    };
            //};
            //po_index_arr = new IndexHalfkeyImmutable<DuplePO>(path + "po_index")
            //{
            //    Table = table,
            //    KeyProducer = POkeyproducer,
            //    HalfProducer = sp => sp.GetHashCode(),
            //    Scale = null
            //};
            ////po_index_arr.Scale = new ScaleCell(path + "po_index_scale") { IndexCell = po_index_arr.IndexCell };
            //po_index = new IndexDynamic<DuplePO, IndexHalfkeyImmutable<DuplePO>>(false)
            //{
            //    Table = table,
            //    IndexArray = po_index_arr,
            //    KeyProducer = POkeyproducer
            //};
        }

        public void Clear() { table.Clear(); }

        public void Build(IEnumerable<Tuple<string, string, ObjectVariants>> triples)
        {
            table.Clear();
            table.Fill(triples.Select(tr => new object[] { tr.Item1, tr.Item2, tr.Item3.ToWritable() }));
            Console.WriteLine("table fill ok.");
            
            spo_ind_arr.Build();
            Console.WriteLine("spo_ind_arr build ok.");
            
            po_ind_arr.Build();
            Console.WriteLine("po_ind_arr build ok.");

            o_ind_arr.Build();
            Console.WriteLine("o_ind_arr build ok.");
            
            //s_index_arr.Build();
            //Console.WriteLine("s_index_arr build ok.");
            //s_index_arr.Statistics();

            sp_index_arr.Build();
            Console.WriteLine("sp_index_arr build ok.");
            sp_index_arr.Statistics();

            spo_index_arr.Build();
            Console.WriteLine("spo_index_arr build ok.");
            spo_index_arr.Statistics();

            //po_index_arr.Build(); // варинат с полуключем
            //Console.WriteLine("po_index_arr build ok.");
            //po_index_arr.Statistics();
        }

        public void Build(IGenerator<List<TripleStrOV>> generator)
        {
            table.Clear();
            table.Fill(new Object[0]);
            generator.Start(list =>
            {
                foreach (var t in list)
                    table.AppendValue(new object[]{t.Subject, t.Predicate, t.Object.ToWritable()});
            });
            Console.WriteLine("table fill ok.");

            spo_ind_arr.Build();
            Console.WriteLine("spo_ind_arr build ok.");

            po_ind_arr.Build();
            Console.WriteLine("po_ind_arr build ok.");

            o_ind_arr.Build();
            Console.WriteLine("o_ind_arr build ok.");

            //s_index_arr.Build();
            //Console.WriteLine("s_index_arr build ok.");
            //s_index_arr.Statistics();

            sp_index_arr.Build();
            Console.WriteLine("sp_index_arr build ok.");
            sp_index_arr.Statistics();

            spo_index_arr.Build();
            Console.WriteLine("spo_index_arr build ok.");
            spo_index_arr.Statistics();

            //po_index_arr.Build(); // варинат с полуключем
            //Console.WriteLine("po_index_arr build ok.");
            //po_index_arr.Statistics();
        }

        public void Warmup()
        {
            table.Warmup();
            spo_ind.Warmup();
            po_ind.Warmup();
            o_ind.Warmup();
            //s_index_arr.Warmup();
            sp_index_arr.Warmup();
            spo_index_arr.Warmup();
            //po_index_arr.Warmup(); // вариант с полуключем
        }
        public Func<PaEntry, object[]> Dereference { get 
        {
            return en =>
            {
                //var v = en.Get();
                return (object[])en.Get();
            };
        } }

        // Структуры, играющие роль ключа
        public class TripleSPOu : IComparable
        {
            public Tuple<string, string, ObjectVariants> triple { get; set; }
            public int CompareTo(object another)
            {
                if (!(another is TripleSPOu)) throw new Exception("kdjfk");
                TripleSPOu ano = (TripleSPOu)another;
                int cmp = triple.Item1.CompareTo(ano.triple.Item1);
                if (cmp == 0 && ano.triple.Item2 != null)
                {
                    cmp = triple.Item2.CompareTo(ano.triple.Item2);
                    if (cmp == 0 && ano.triple.Item3 != null) cmp = triple.Item3.CompareTo(ano.triple.Item3);
                }
                return cmp;
            }
        }
        public class DuplePOu : IComparable
        {
            public Tuple<string, ObjectVariants> tuple { get; set; }
            public int CompareTo(object another)
            {
                //if (!(another is DupleSP)) throw new Exception("kdjfk");
                DuplePOu ano = (DuplePOu)another;
                int cmp = tuple.Item1.CompareTo(ano.tuple.Item1);
                if (cmp == 0 && ano.tuple.Item2 != null) // Второе условие позволяет задавать null во втором поле another
                {
                    cmp = tuple.Item2.CompareTo(ano.tuple.Item2);
                }
                return cmp;
            }
        }
        public class MonopleOu : IComparable
        {
            public Tuple<ObjectVariants> tuple { get; set; }
            public int CompareTo(object another)
            {
                MonopleOu ano = (MonopleOu)another;
                int cmp = tuple.Item1.CompareTo(ano.tuple.Item1);
                return cmp;
            }
        }
        public class MonopleS : IComparable
        {
            public Tuple<string> tuple { get; set; }
            public int CompareTo(object another)
            {
                MonopleS ano = (MonopleS)another;
                int cmp = this.GetHashCode().CompareTo(ano.GetHashCode());
                if (cmp == 0) cmp = tuple.Item1.CompareTo(ano.tuple.Item1);
                return cmp;
            }
            public override int GetHashCode()
            {
                return tuple.Item1.GetHashCode();
            }
        }
        public class DupleSP : IComparable
        {
            public Tuple<string, string> tuple { get; set; }
            public int CompareTo(object another)
            {
                DupleSP ano = (DupleSP)another;
                int cmp = this.GetHashCode().CompareTo(ano.GetHashCode());
                if (cmp == 0) cmp = tuple.Item1.CompareTo(ano.tuple.Item1);
                if (cmp == 0) cmp = tuple.Item2.CompareTo(ano.tuple.Item2);
                return cmp;
            }
            public override int GetHashCode()
            {
                return unchecked(tuple.Item1.GetHashCode() * 77777 + tuple.Item2.GetHashCode());
            }
        }
        public class TripleSPO : IComparable
        {
            public Tuple<string, string, ObjectVariants> tuple { get; set; }
            public int CompareTo(object another)
            {
                TripleSPO ano = (TripleSPO)another;
                int cmp = this.GetHashCode().CompareTo(ano.GetHashCode());
                if (cmp == 0) cmp = tuple.Item1.CompareTo(ano.tuple.Item1);
                if (cmp == 0) cmp = tuple.Item2.CompareTo(ano.tuple.Item2);
                if (cmp == 0) cmp = tuple.Item3.CompareTo(ano.tuple.Item3);
                return cmp;
            }
            public override int GetHashCode()
            {
                return unchecked((tuple.Item3.GetHashCode() * 11111 + tuple.Item2.GetHashCode()) * 77777 + tuple.Item1.GetHashCode());
            }
        }
        public class DuplePO : IComparable
        {
            public Tuple<string, ObjectVariants> tuple { get; set; }
            public int CompareTo(object another)
            {
                DuplePO ano = (DuplePO)another;
                int cmp = this.GetHashCode().CompareTo(ano.GetHashCode());
                if (cmp == 0) cmp = tuple.Item1.CompareTo(ano.tuple.Item1);
                if (cmp == 0) cmp = tuple.Item2.CompareTo(ano.tuple.Item2);
                return cmp;
            }
            public override int GetHashCode()
            {
                return unchecked(tuple.Item1.GetHashCode() * 77777 + tuple.Item2.GetHashCode());
            }
        }


        public object CodeIRI(string iri)
        {
            return iri;
        }

        public string DecodeIRI(object oiri)
        {
            return (string)oiri;
        }

        public object CodeOV(ObjectVariants ov)
        {
            return ov.ToWritable();
        }

        public ObjectVariants DecodeOV(object oov)
        {
            return oov.ToOVariant();
        }

        public IEnumerable<PaEntry> GetTriples()
        {
            return table.TableCell.Root.Elements()
                .Where(ent => ! (bool)ent.Field(0).Get())
                .Select(ent => ent.Field(1));
        }

        public IEnumerable<PaEntry> GetTriplesWithSubject(object osubj)
        {
            string subj = (string)osubj;
            var query = spo_ind.GetAllByKey(new TripleSPOu() // универсальный индекс
            {
                triple = new Tuple<string, string, ObjectVariants>(subj, null, null)
            });
            //var query = s_index.GetAllByKey(new MonopleS() // полуключевой индекс
            //{
            //    tuple = new Tuple<string>(subj)
            //});
            return query.Select(en => en.Field(1));
        }

        public IEnumerable<PaEntry> GetTriplesWithSubjectPredicate(object osubj, object opred)
        {
            string subj = (string)osubj;
            string pred = (string)opred;
            //var query = spo_ind.GetAllByKey(new TripleSPO() // универсальны вариант
            //{
            //    triple = new Tuple<string, string, ObjectVariants>(subj, pred, null)
            //}).Select(en => en.Field(1));
            var query = sp_index.GetAllByKey(new DupleSP()
            {
                tuple = new Tuple<string, string>(subj, pred)
            }).Select(en => en.Field(1));
            return query;
        }

        public bool Contains(object osubj, object opred, object oobj)
        {
            string subj = (string)osubj;
            string pred = (string)opred;
            ObjectVariants obj = (ObjectVariants)oobj;
            //var query = spo_ind.GetAllByKey(new TripleSPOu() // Универсальный вариант
            //{
            //    triple = new Tuple<string, string, ObjectVariants>(subj, pred, obj)
            //});
            var query = spo_index.GetAllByKey(new TripleSPO()
            {
                tuple = new Tuple<string, string, ObjectVariants>(subj, pred, obj)
            }); 
            return query.Any();
        }

        public IEnumerable<PaEntry> GetTriplesWithPredicate(object opred)
        {
            string pred = (string)opred;
            var query = po_ind.GetAllByKey(new DuplePOu()
            {
                tuple = new Tuple<string, ObjectVariants>(pred, null)
            }).Select(en => en.Field(1));
            return query;
        }

        public IEnumerable<PaEntry> GetTriplesWithPredicateObject(object opred, object oobj)
        {
            string pred = (string)opred;
            ObjectVariants obj = (ObjectVariants)oobj;
            var query = po_ind.GetAllByKey(new DuplePOu()
            {
                tuple = new Tuple<string, ObjectVariants>(pred, obj)
            }).Select(en => en.Field(1)); //.ToArray();
            //var query = po_index.GetAllByKey(new DuplePO() // вариант с полуключем
            //{
            //    tuple = new Tuple<string, ObjectVariants>(pred, obj)
            //}).Select(en => en.Field(1));
            return query;
        }

        public IEnumerable<PaEntry> GetTriplesWithObject(object oobj)
        {
            ObjectVariants obj = (ObjectVariants)oobj;
            var query = o_ind.GetAllByKey(new MonopleOu()
            {
                tuple = new Tuple<ObjectVariants>(obj)
            }).Select(en => en.Field(1));
            return query;
        }
    }
}
