using System;
using System.Collections.Generic;
using System.Linq;
using RDFTripleStore;
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
                        iris = iris.Concat(new string[] {((OV_iri)tri.Object).full_id});
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
                        int iobj = dictionary[((OV_iri)ov).full_id];
                        ov = new OV_iriint(iobj);
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
        public void Build(IGenerator<List<Triple<string, string, ObjectVariants>>> generator)
        {
            throw new NotImplementedException();
        }
        private void BuildIndexes()
        {
            index_s_arr.Build();
        }
        public IEnumerable<Triple<int, int, ObjectVariants>> Search(object subject = null, object predicate = null, ObjectVariants obj = null)
        {
            if (subject != null)
            {
                int isubj = subject is string ? coding_table.GetCodeByString((string)subject) : -1;
                var query_by_subject = index_s.GetAllByKey(isubj);
                //Console.WriteLine("count={0}", query.Count());
                var query = query_by_subject.Select(ent =>
                {
                    object[] three = (object[])((object[])ent.Get())[1];
                    int s = (int)three[0];
                    int p = (int)three[1];
                    var o = ((object[])three[2]).Writeble2OVariant();
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
    }
}
