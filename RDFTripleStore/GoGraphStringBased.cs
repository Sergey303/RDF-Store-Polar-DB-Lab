using System;
using System.Collections.Generic;
using System.Linq;
using PolarDB;
using Task15UniversalIndex;

namespace RDFTripleStore
{
    public class GoGraphStringBased : IGraph<Triple<string, string, ObjectVariants>>
    {
        private TableView table;
        private IndexViewImmutable<TripleSPO> spo_ind_arr;
        private IndexDynamic<TripleSPO, IndexViewImmutable<TripleSPO>> spo_ind;
        public GoGraphStringBased(string path)
        {
            PType tp_tabelement = new PTypeRecord(
                new NamedType("subject", new PType(PTypeEnumeration.sstring)),
                new NamedType("predicate", new PType(PTypeEnumeration.sstring)),
                new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            Func<object,TripleSPO> keyproducer = v =>
                {
                    object[] va = (object[])((object[])v)[1];
                    return new TripleSPO()
                    {
                        triple = new Triple<string, string, ObjectVariants>((string)va[0], (string)va[1],
                            ObjectVariants.CreateLiteralNode(false))
                    };
                };
            // Опорная таблица
            table = new TableView(path + "stable", tp_tabelement);
            // Индекс spo
            spo_ind_arr = new IndexViewImmutable<TripleSPO>(path + "spo_ind")
            {
                Table = table,
                KeyProducer = keyproducer 
            };
            spo_ind = new IndexDynamic<TripleSPO, IndexViewImmutable<TripleSPO>>(false)
            {
                Table = table,
                IndexArray = spo_ind_arr,                    
                KeyProducer = keyproducer
            };
        }

        public void Build(IEnumerable<Triple<string, string, ObjectVariants>> triples)
        {   
                table.Fill(triples.Select(triple2Writable));

            spo_ind_arr.Build();

        }

        private object[] triple2Writable(Triple<string, string, ObjectVariants> tr)
        {
            return new object[] {tr.Subject, tr.Predicate, tr.Object.ToWritable()};
        }

        public void Build(IGenerator<List<Triple<string, string, ObjectVariants>>> generator)
        {
            generator.Start(list =>
            {
                foreach (var triple in list)
                    table.AppendValue(triple2Writable(triple));
            });
            spo_ind_arr.Build();
        }

        public IEnumerable<Triple<string, string, ObjectVariants>> Search(object subject = null, object predicate = null, ObjectVariants obj = null)
        {
            if (subject != null)
            {
                string ssubj = (string)subject;
                string spred = (string)predicate;
                TripleSPO key_triple = new TripleSPO() { triple = new Triple<string,string,ObjectVariants>(ssubj, spred, null) };
                PaEntry tab_entity = table.TableCell.Root.Element(0);
                IEnumerable<PaEntry> entities = spo_ind.GetAllByKey(key_triple);
                var ou_triples = entities.Select(ent =>
                {
                    object[] three = (object[])(((object[])ent.Get())[1]);
                    return new Triple<string, string, ObjectVariants>((string)three[0], (string)three[1], null);
                });
                return ou_triples;
            }
            else throw new NotImplementedException();
        }

        // Структуры, играющие роль ключа
        public class TripleSPO : IComparable
        {
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
