using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFTripleStore;
using RDFTripleStore.OVns;
using Task15UniversalIndex;
using PolarDB;

namespace GoTripleStore
{
    public class GoGraphIntBased : IGraph<Triple<int, int, ObjectVariants>>
    {
        internal TableView table;
        private NameTableUniversal coding_table;
        private IndexKeyImmutable<int> index_s_arr;
        private IndexDynamic<int, IndexKeyImmutable<int>> index_s;
        private IndexHalfkeyImmutable<SP_Pair> index_sp_arr;
        private IndexDynamic<SP_Pair, IndexHalfkeyImmutable<SP_Pair>> index_sp;
        private IndexHalfkeyImmutable<SPO_Troyka> index_spo_arr;
        private IndexDynamic<SPO_Troyka, IndexHalfkeyImmutable<SPO_Troyka>> index_spo;
        //private IndexHalfkeyImmutable<PO_Pair> index_po_arr;
        //private IndexDynamic<PO_Pair, IndexHalfkeyImmutable<PO_Pair>> index_po;
        private IndexViewImmutable<PO_Pair> index_po_arr_base;
        private IndexDynamic<PO_Pair, IndexViewImmutable<PO_Pair>> index_po_base;
        public GoGraphIntBased(string path)
        {
            PType tp_tabelement = new PTypeRecord(
                new NamedType("subject", new PType(PTypeEnumeration.integer)),
                new NamedType("predicate", new PType(PTypeEnumeration.integer)),
                new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            // Опорная таблица
            table = new TableView(path + "stable", tp_tabelement);
            // Таблица имен (таб. кодирования)
            coding_table = new NameTableUniversal(path);
            // Индекс s
            index_s_arr = new IndexKeyImmutable<int>(path + "s_")
            {
                Table = table,
                KeyProducer = v =>
                    {
                        object[] va = (object[])((object[])v)[1];
                        return (int)va[0];
                    },
                Scale = null
            };
            index_s = new IndexDynamic<int, IndexKeyImmutable<int>>(false)
            {
                Table = table,
                KeyProducer = v =>
                {
                    object[] va = (object[])((object[])v)[1];
                    return (int)va[0];
                },
                IndexArray = index_s_arr
            };
            // Индекс sp
            index_sp_arr = new IndexHalfkeyImmutable<SP_Pair>(path + "sp_")
            {
                Table = table, 
                KeyProducer = v =>
                {
                    object[] va = (object[])((object[])v)[1];
                    return new SP_Pair((int)va[0], (int)va[1]);
                },
                Scale = null,
                HalfProducer = sp => sp.GetHashCode()
            };
            index_sp_arr.Scale = new ScaleCell(path + "sp_index") { IndexCell = index_sp_arr.IndexCell };
            index_sp = new IndexDynamic<SP_Pair, IndexHalfkeyImmutable<SP_Pair>>(false)
            {
                Table = table,
                IndexArray = index_sp_arr,
                KeyProducer = v =>
                {
                    object[] va = (object[])((object[])v)[1];
                    return new SP_Pair((int)va[0], (int)va[1]);
                }
            };
            // Индекс spo
            index_spo_arr = new IndexHalfkeyImmutable<SPO_Troyka>(path + "spo_")
            {
                Table = table,
                KeyProducer = v =>
                {
                    object[] va = (object[])((object[])v)[1];
                    return new SPO_Troyka((int)va[0], (int)va[1], ((object[])va[2]).Writeble2OVariant());
                },
                Scale = null,
                HalfProducer = spo => spo.GetHashCode()
            };
            index_spo_arr.Scale = new ScaleCell(path + "spo_index") { IndexCell = index_spo_arr.IndexCell };
            index_spo = new IndexDynamic<SPO_Troyka, IndexHalfkeyImmutable<SPO_Troyka>>(true)
            {
                Table = table,
                IndexArray = index_spo_arr,
                KeyProducer = v =>
                {
                    object[] va = (object[])((object[])v)[1];
                    return new SPO_Troyka((int)va[0], (int)va[1], ((object[])va[2]).Writeble2OVariant());
                }
            };
            //// Индекс po
            //index_po_arr = new IndexHalfkeyImmutable<PO_Pair>(path + "po_")
            //{
            //    Table = table,
            //    KeyProducer = v =>
            //    {
            //        object[] va = (object[])((object[])v)[1];
            //        return new PO_Pair((int)va[1], ((object[])va[2]).Writeble2OVariant());
            //    },
            //    Scale = null,
            //    HalfProducer = po => po.GetHashCode()
            //};
            //index_po_arr.Scale = new ScaleCell(path + "po_index") { IndexCell = index_po_arr.IndexCell };
            //index_po = new IndexDynamic<PO_Pair, IndexHalfkeyImmutable<PO_Pair>>(false)
            //{
            //    Table = table,
            //    IndexArray = index_po_arr,
            //    KeyProducer = v =>
            //    {
            //        object[] va = (object[])((object[])v)[1];
            //        return new PO_Pair((int)va[1], ((object[])va[2]).Writeble2OVariant());
            //    }
            //};
            index_po_arr_base = new IndexViewImmutable<PO_Pair>(path + "po_base_")
            {
                Table = table,
                KeyProducer = v =>
                {
                    object[] va = (object[])((object[])v)[1];
                    return new PO_Pair((int)va[1], ((object[])va[2]).Writeble2OVariant());
                }
            };
            index_po_base = new IndexDynamic<PO_Pair, IndexViewImmutable<PO_Pair>>(false)
            {
                Table = table,
                IndexArray = index_po_arr_base,
                KeyProducer = v =>
                {
                    object[] va = (object[])((object[])v)[1];
                    return new PO_Pair((int)va[1], ((object[])va[2]).Writeble2OVariant());
                }
            };
        }
        private int _portion = 500000;
        public int Portion { set { _portion = value; } }
        public void Build(IEnumerable<Triple<string, string, ObjectVariants>> triples)
        {
            table.Clear();
            table.TableCell.Fill(new object[0]);
            coding_table.Clear();
            coding_table.Fill(new string[0]);
            coding_table.BuildIndexes();
            // Определим буфер
            Func<List<Triple<string, string, ObjectVariants>>, bool> processList = list =>
            {
                IEnumerable<string> ids = list.SelectMany(tri =>
                {
                    IEnumerable<string> iris = new string[] { tri.Subject, tri.Predicate };
                    if (tri.Object.Variant == ObjectVariantEnum.Iri) 
                        iris = iris.Concat(new string[] {((OV_iri)tri.Object).UriString});
                    return iris;
                });
                var dictionary = coding_table.InsertPortion(ids);
                foreach (var tri in list)
                {
                    int isubj = dictionary[tri.Subject];
                    int ipred = dictionary[tri.Predicate];
                    ObjectVariants ov = tri.Object;
                    if (ov.Variant == ObjectVariantEnum.Iri)
                    {
                        int iobj = dictionary[((OV_iri)ov).UriString];
                        ov = new OV_iriint(iobj, coding_table);
                    }
                    table.TableCell.Root.AppendElement(new object[] { false, new object[] { isubj, ipred, ov.ToWritable() } });
                }
                return true;
            };
            List<Triple<string, string, ObjectVariants>> tr_list = new List<Triple<string, string, ObjectVariants>>();
            foreach (Triple<string, string, ObjectVariants> tr in triples)
            {
                tr_list.Add(tr);
                if (tr_list.Count >= _portion)
                {
                    processList(tr_list);                    
                    tr_list.Clear();
                }
            }
            if (tr_list.Count > 0)
            {
                processList(tr_list);
            }
            coding_table.BuildScale();
            table.TableCell.Flush();

            BuildIndexes();
        }

        public void Build(IEnumerable<Triple<int, int, ObjectVariants>> triples)
        {
            throw new NotImplementedException();
        }

        public void Build(IGenerator<List<Triple<string, string, ObjectVariants>>> generator)
        {
            throw new NotImplementedException();
        }
        public void BuildIndexes()
        {
            index_s_arr.Build();
            Console.WriteLine("index_s_arr ok.");

            index_sp_arr.Build();
            Console.WriteLine("index_sp_arr ok.");
            index_sp_arr.Statistics();

            index_spo_arr.Build();
            Console.WriteLine("index_spo_arr ok.");
            index_spo_arr.Statistics();

            //index_po_arr.Build();
            //Console.WriteLine("index_po_arr ok.");
            //index_po_arr.Statistics();

            index_po_arr_base.Build();
            Console.WriteLine("index_po_arr_base ok.");

        }
        public IEnumerable<Triple<int, int, ObjectVariants>> Search(object subject = null, object predicate = null, ObjectVariants obj = null)
        {
            if (subject != null && predicate == null && obj == null)
            {
                int isubj = subject is string ? coding_table.GetCodeByString((string)subject) : -1;
                var query_by_subject = index_s.GetAllByKey(isubj);
                //Console.WriteLine("count={0}", query.Count());
                var query = query_by_subject.Select(ent =>
                {
                    object[] three = (object[])((object[])ent.Get())[1];
                    int s = (int)three[0];
                    int p = (int)three[1];
                    var o = ((object[])three[2]).Writeble2OVariant(coding_table);
                    return new Triple<int, int, ObjectVariants>(s, p, o);
                });
                return query;
            }
            else if (subject != null && predicate != null && obj == null)
            {
                int isubj = subject is string ? coding_table.GetCodeByString((string)subject) : -1;
                int ipred = subject is string ? coding_table.GetCodeByString((string)predicate) : -1;
                SP_Pair pair = new SP_Pair(isubj, ipred);
                var query = index_sp.GetAllByKey(pair);
                var qu = query.Select(ent =>
                {
                    object[] three = (object[])((object[])ent.Get())[1];
                    int s = (int)three[0];
                    int p = (int)three[1];
                    var o = ((object[])three[2]).Writeble2OVariant(coding_table);
                    return new Triple<int, int, ObjectVariants>(s, p, o);
                });
                return qu;
            }
            else if (subject == null && predicate != null && obj != null)
            {
                //int isubj = subject is string ? coding_table.GetCodeByString((string)subject) : -1;
                int ipred = predicate is string ? coding_table.GetCodeByString((string)predicate) : -1;
                ObjectVariants ov = (ObjectVariants)obj;
                if (ov is OV_iri) 
                {
                    string iri = ((OV_iri)ov).Name;
                    int code = coding_table.GetCodeByString(iri);
                    //if (code < 0) throw new Exception("RRRRRR No code for iri");
                    ov = new OV_iriint(code, coding_table);
                }
                PO_Pair pop = new PO_Pair(ipred, ov);
                //var query = index_po.GetAllByKey(pop);
                var query = index_po_base.GetAllByKey(pop);
                var qu = query.Select(ent =>
                {
                    object[] three = (object[])((object[])ent.Get())[1];
                    int s = (int)three[0];
                    int p = (int)three[1];
                    var o = ((object[])three[2]).Writeble2OVariant(coding_table);
                    return new Triple<int, int, ObjectVariants>(s, p, o);
                });
                return qu;
            }
            throw new NotImplementedException();
        }
        public int Code(string s)
        {
            return coding_table.GetCodeByString(s);
        }
        public string Decode(int cod)
        {
            return coding_table.GetStringByCode(cod);
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
                return ( 3001^ s.GetHashCode() ) * (1409 ^ p.GetHashCode());
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
                if (cmp == 0)
                {
                    cmp = this.p.CompareTo(ano.p);
                }
                if (cmp == 0)
                {
                   cmp = this.ov.CompareTo(ano.ov);
                }
                return cmp;
            }
            public override int GetHashCode()
            {
                return (2 ^ s.GetHashCode() ) * (3 ^ p.GetHashCode()) * (7 ^ ov.GetHashCode());
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
                if (cmp == 0)
                {
                    cmp = this.ov.CompareTo(ano.ov);
                }
                return cmp;
            }
            public override int GetHashCode()
            {
                //return p.GetHashCode() + 7777 * ov.GetHashCode();
                return unchecked((2 ^p.GetHashCode()) * (3 ^ov.GetHashCode()));
                //return unchecked(ov.GetHashCode() + 77777 * p.GetHashCode()); 
                //int v = ov.Variant.GetHashCode();
                //return unchecked(ov.GetHashCode() + p.GetHashCode() * 77777); 
            }
        }
    }
}
