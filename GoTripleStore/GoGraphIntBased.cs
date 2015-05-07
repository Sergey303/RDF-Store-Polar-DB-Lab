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
    public class GoGraphIntBased : IGraph<TripleIntOV>
    {
        internal TableView table;
        private NameTableUniversal coding_table;
        private IndexKeyImmutable<int> index_s_arr;
        private IndexDynamic<int, IndexKeyImmutable<int>> index_s;
        private IndexHalfkeyImmutable<SP_Pair> index_sp_arr;
        private IndexDynamic<SP_Pair, IndexHalfkeyImmutable<SP_Pair>> index_sp;
        private IndexHalfkeyImmutable<SPO_Troyka> index_spo_arr;
        private IndexDynamic<SPO_Troyka, IndexHalfkeyImmutable<SPO_Troyka>> index_spo;
        // Альтернативы (парами):
        private IndexHalfkeyImmutable<PO_Pair> index_po_arr;
        private IndexDynamic<PO_Pair, IndexHalfkeyImmutable<PO_Pair>> index_po;
        private IndexViewImmutable<PO_Pair> index_po_arr_base;
        private IndexDynamic<PO_Pair, IndexViewImmutable<PO_Pair>> index_po_base;
        private bool ishalfkey_index_po = false; // Выбор альтернативы
        
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
            // Индекс po
            if (ishalfkey_index_po)
            {
                index_po_arr = new IndexHalfkeyImmutable<PO_Pair>(path + "po_")
                {
                    Table = table,
                    KeyProducer = v =>
                    {
                        object[] va = (object[])((object[])v)[1];
                        return new PO_Pair((int)va[1], ((object[])va[2]).Writeble2OVariant());
                    },
                    Scale = null,
                    HalfProducer = po => po.GetHashCode()
                };
                index_po_arr.Scale = new ScaleCell(path + "po_index") { IndexCell = index_po_arr.IndexCell };
                index_po = new IndexDynamic<PO_Pair, IndexHalfkeyImmutable<PO_Pair>>(false)
                {
                    Table = table,
                    IndexArray = index_po_arr,
                    KeyProducer = v =>
                    {
                        object[] va = (object[])((object[])v)[1];
                        return new PO_Pair((int)va[1], ((object[])va[2]).Writeble2OVariant());
                    }
                };
            }
            else
            {
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
        }
        private int _portion = 500000;
        public int Portion { set { _portion = value; } }
        public void Build(IEnumerable<TripleStrOV> triples)
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
                        ov = new OV_iriint(iobj, coding_table.GetStringByCode);
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

        public void Build(IGenerator<List<TripleStrOV>> generator)
        {
            table.Clear();
            table.TableCell.Fill(new object[0]);
            coding_table.Clear();
            coding_table.Fill(new string[0]);
            coding_table.BuildIndexes();
            generator.Start(buffer =>
            {
                IEnumerable<string> ids = buffer.SelectMany(tri =>
                {
                    IEnumerable<string> iris = new string[] { tri.Subject, tri.Predicate };
                    if (tri.Object.Variant == ObjectVariantEnum.Iri)
                        iris = iris.Concat(new string[] { ((OV_iri)tri.Object).UriString });
                    return iris;
                });
                var dictionary = coding_table.InsertPortion(ids);
                foreach (var tri in buffer)
                {
                    int isubj = dictionary[tri.Subject];
                    int ipred = dictionary[tri.Predicate];
                    ObjectVariants ov = tri.Object;
                    if (ov.Variant == ObjectVariantEnum.Iri)
                    {
                        int iobj = dictionary[((OV_iri)ov).UriString];
                        ov = new OV_iriint(iobj, coding_table.GetStringByCode);
                    }
                    table.TableCell.Root.AppendElement(new object[] { false, new object[] { isubj, ipred, ov.ToWritable() } });
                }    
            });
        
            coding_table.BuildScale();
            table.TableCell.Flush();

            BuildIndexes();
        }

        public void Warmup()
        {
            table.Warmup();
            coding_table.Warmup();
            index_s_arr.Warmup();
            index_sp_arr.Warmup();
            index_spo_arr.Warmup();
            if (ishalfkey_index_po)
            {
                index_po_arr.Warmup();
            }
            else
            {
                index_po_arr_base.Warmup();
            }
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

            if (ishalfkey_index_po)
            {
                index_po_arr.Build();
                Console.WriteLine("index_po_arr ok.");
                index_po_arr.Statistics();
            }
            else
            {
                index_po_arr_base.Build();
                Console.WriteLine("index_po_arr_base ok.");
            }
        }
        public IEnumerable<TripleIntOV> Search(object subject = null, object predicate = null, ObjectVariants obj = null)
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
                    var o = ((object[])three[2]).Writeble2OVariant(coding_table.GetStringByCode);
                    return new TripleIntOV(s, p, o);
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
                    var o = ((object[])three[2]).Writeble2OVariant(coding_table.GetStringByCode);
                    return new TripleIntOV(s, p, o);
                });
                return qu;
            }
            else if (subject != null && predicate != null && obj != null)
            {
                int isubj = subject is string ? coding_table.GetCodeByString((string)subject) : -1;
                int ipred = subject is string ? coding_table.GetCodeByString((string)predicate) : -1;
                ObjectVariants ov = (ObjectVariants)obj;
                if (ov is OV_iri)
                {
                    string iri = ((OV_iri)ov).Name;
                    int code = coding_table.GetCodeByString(iri);
                    ov = new OV_iriint(code, coding_table.GetStringByCode);
                }
                SPO_Troyka troyka = new SPO_Troyka(isubj, ipred, ov);
                var query = index_spo.GetAllByKey(troyka);
                var qu = query.Select(ent =>
                {
                    object[] three = (object[])((object[])ent.Get())[1];
                    int s = (int)three[0];
                    int p = (int)three[1];
                    var o = ((object[])three[2]).Writeble2OVariant(coding_table.GetStringByCode);
                    return new TripleIntOV(s, p, o);
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
                    ov = new OV_iriint(code, coding_table.GetStringByCode);
                }
                PO_Pair pop = new PO_Pair(ipred, ov);
                //var query = index_po.GetAllByKey(pop);
                var query = ishalfkey_index_po ? index_po.GetAllByKey(pop) : index_po_base.GetAllByKey(pop);
                //query.Count();
                //return Enumerable.Empty<TripleIntOV>();
                var qu = query.Select(ent =>
                {
                    object[] three = (object[])((object[])ent.Get())[1];
                    int s = (int)three[0];
                    int p = (int)three[1];
                    var o = ((object[])three[2]).Writeble2OVariant(coding_table.GetStringByCode);
                    return new TripleIntOV(s, p, o);
                });
                return qu;
            }
            throw new NotImplementedException();
        }
        
        // =============== Поиск в стиле IGraph ================
        public IEnumerable<PaEntry> GetTriplesWithSubject(int isubj)
        {
            return index_s.GetAllByKey(isubj);
        }
        public IEnumerable<PaEntry> GetTriplesWithSubjectPredicate(int isubj, int ipred)
        {
            return index_sp.GetAllByKey(new SP_Pair(isubj, ipred));
        }
        public bool Contains(int isubj, int ipred, ObjectVariants ov)
        {
            return index_spo.GetAllByKey(new SPO_Troyka(isubj, ipred, ov)).Any();
        }
        public IEnumerable<PaEntry> GetTriplesWithPredicateObject(int ipred, ObjectVariants ov)
        {
            if (ishalfkey_index_po) return index_po.GetAllByKey(new PO_Pair(ipred, ov));
            else return index_po_base.GetAllByKey(new PO_Pair(ipred, ov));
        }

        // =============== конец методов поиска ================

        public int Code(string s)
        {
            return coding_table.GetCodeByString(s);
        }
        public string Decode(int cod)
        {
            return coding_table.GetStringByCode(cod);
        }

    }
}
