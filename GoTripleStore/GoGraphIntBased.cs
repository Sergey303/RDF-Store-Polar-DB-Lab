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
        private TableView table;
        private NameTableUniversal coding_table;
        private IndexKeyImmutable<int> index_s_arr;
        private IndexDynamic<int, IndexKeyImmutable<int>> index_s;
        private IndexHalfkeyImmutable<SP_Pair> index_sp_arr;
        private IndexDynamic<SP_Pair, IndexHalfkeyImmutable<SP_Pair>> index_sp;
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
        private void BuildIndexes()
        {
            index_s_arr.Build();
            index_sp_arr.Build();
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

                if (predicate != null)
                {
                    int ipred = subject is string ? coding_table.GetCodeByString((string)predicate) : -1;
                    query = query.Where(tri => tri.Predicate == ipred);
                }
                else // if (predicate == null)
                { 
                }
                return query;
            }
            else if (subject != null && predicate != null && obj == null)
            {
                int isubj = subject is string ? coding_table.GetCodeByString((string)subject) : -1;
                int ipred = subject is string ? coding_table.GetCodeByString((string)predicate) : -1;
                SP_Pair pair = new SP_Pair(isubj, ipred);
                var query = index_sp.GetAllByKey(pair).ToArray();
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
        }
    }
}
