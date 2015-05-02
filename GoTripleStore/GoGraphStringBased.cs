using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using RDFTripleStore;
using RDFTripleStore.parsers;
using Task15UniversalIndex;
using PolarDB;


namespace GoTripleStore
{
    public class GoGraphStringBased : IGra<PaEntry> // IGraph<Triple<string, string, ObjectVariants>>
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
            Func<object, TripleSPO> keyproducer = v =>
                {
                    object[] va = (object[])((object[])v)[1];
                    return new TripleSPO()
                    {
                        triple = new Tuple<string, string, ObjectVariants>((string)va[0], (string)va[1],
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

        public void Build(IEnumerable<Tuple<string, string, ObjectVariants>> triples)
        {
            table.Clear();
            table.Fill(triples.Select(tr => new object[] { tr.Item1, tr.Item2, tr.Item3.ToWritable() }));
            spo_ind_arr.Build();
        }

        public Func<PaEntry, object[]> Dereference { get { return en => new object[3]; } }

        // Структуры, играющие роль ключа
        public class TripleSPO : IComparable
        {
            public Tuple<string, string, ObjectVariants> triple { get; set; }
            public int CompareTo(object another)
            {
                if (!(another is TripleSPO)) throw new Exception("kdjfk");
                TripleSPO ano = (TripleSPO)another;
                int cmp = triple.Item1.CompareTo(ano.triple.Item1);
                if (cmp == 0 && ano.triple.Item2 != null)
                {
                    cmp = triple.Item2.CompareTo(ano.triple.Item2);
                    if (cmp == 0) cmp = triple.Item3.CompareTo(ano.triple.Item3);
                }
                return cmp;
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
            throw new NotImplementedException();
        }

        public IEnumerable<PaEntry> GetTriplesWithSubject(object osubj)
        {
            string subj = (string)osubj;
            return Enumerable.Empty<PaEntry>();
        }

        public IEnumerable<PaEntry> GetTriplesWithSubjectPredicate(object subj, object pred)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object subj, object pred, object obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PaEntry> GetTriplesWithPredicate(object pred)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PaEntry> GetTriplesWithPredicateObject(object pred, object obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PaEntry> GetTriplesWithObject(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
