using System;
using System.Collections.Generic;
using System.Linq;
using RDFTripleStore;
using Task15UniversalIndex;
using PolarDB;


namespace GoTripleStore
{
    public class GoGraphStringBased : IGraph<Triple<string, string, ObjectVariants>>
    {
        private TableView table;
        public GoGraphStringBased(string path)
        {
            PType tp_tabelement = new PTypeRecord(
                new NamedType("subject", new PType(PTypeEnumeration.sstring)),
                new NamedType("predicate", new PType(PTypeEnumeration.sstring)),
                new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            table = new TableView(path + "stable", tp_tabelement);
        }

        public void Build(IEnumerable<Triple<string, string, ObjectVariants>> triples)
        {
            //var query = triples.Select(tr => new object[] { tr.Subject, tr.Predicate, tr.Object.ToWritable() })
            //    .Take(10).ToArray();
            //table.Fill(query);
            table.Fill(triples.Select(tr => new object[] { tr.Subject, tr.Predicate, tr.Object.ToWritable() }));
        }

        public IEnumerable<Triple<string, string, ObjectVariants>> Search(object subject = null, object predicate = null, ObjectVariants obj = null)
        {
            throw new NotImplementedException();
        }
    }
}
