using System;
using System.Collections.Generic;
using System.Linq;
using PolarDB;
using RDFCommon.OVns;
using Task15UniversalIndex;

namespace GoTripleStore
{
    public class TripleSetInt
    {
        private NameTableUniversal nametable;
        private TableView table;
        private IndexCascading<int> ps_index;
        public TripleSetInt(string path)
        {
            PType tp_triple = new PTypeRecord(
                new NamedType("subj", new PType(PTypeEnumeration.integer)),
                new NamedType("pred", new PType(PTypeEnumeration.integer)),
                new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            nametable = new NameTableUniversal(path);
            table = new TableView(path + "triples", tp_triple);
            ps_index = new IndexCascading<int>(path + "ps") 
            {
                Table = table,
                Key1Producer = ob => (int)((object[])((object[])ob)[1])[1],
                Key2Producer = ob => (int)((object[])((object[])ob)[1])[0]
            };
        }
        public int Code(string s) { return nametable.GetCodeByString(s); }
        public string Decode(int c) { return nametable.GetStringByCode(c); }
        
        public void Clear() { table.Clear(); }
        public void Warmup() { table.Warmup(); }

        public void Build(IEnumerable<Tuple<string, string, ObjectVariants>> triples)
        {
            nametable.Clear();
            nametable.Fill(new string[0]);
            nametable.BuildIndexes();
            table.Clear();
            table.Fill(new object[0]);
            int portion = 1000000;
            List<Tuple<string, string, ObjectVariants>> buff = new List<Tuple<string, string, ObjectVariants>>();
            foreach (Tuple<string, string, ObjectVariants> tri in triples)
            {
                buff.Add(tri);
                if (buff.Count >= portion)
                {
                    ProcessPortion(buff);
                    buff.Clear();
                }
            }
            if (buff.Count >= portion)
            {
                ProcessPortion(buff);
            }
            table.TableCell.Flush();
            nametable.BuildScale();
            table.BuildIndexes();
            Console.WriteLine("table fill ok.");
        }

        private void ProcessPortion(List<Tuple<string, string, ObjectVariants>> buff)
        {
            // Пополнение таблицы имен
            var dic = nametable.InsertPortion(buff.SelectMany(t =>
            {
                ObjectVariants ov = t.Item3;
                if (ov.Variant == ObjectVariantEnum.Iri)
                {
                    return new string[] { t.Item1, t.Item2, ((OV_iri)ov).Name };
                }
                else
                {
                    return new string[] { t.Item1, t.Item2 };
                }
            }));
            // Пополнение триплетов
            table.Add(buff.Select(t =>
            {
                ObjectVariants ov = t.Item3;
                if (ov.Variant == ObjectVariantEnum.Iri)
                {
                    OV_iriint ovi = new OV_iriint(dic[((OV_iri)ov).Name], ii => ii.ToString());
                    return new object[] { dic[t.Item1], dic[t.Item2], ovi.ToWritable() };
                }
                else
                {
                    return new object[] { dic[t.Item1], dic[t.Item2], ov.ToWritable() };
                }

            }));
        }
    }
}
