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
        public TableView Table { get { return table; } }
        private IndexCascading<int> ps_index;
        private IndexCascading<ObjectVariants> po_index;
        public TripleSetInt(string path)
        {
            PType tp_triple = new PTypeRecord(
                new NamedType("subj", new PType(PTypeEnumeration.integer)),
                new NamedType("pred", new PType(PTypeEnumeration.integer)),
                new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            nametable = new NameTableUniversal(path);
            table = new TableView(path + "triples", tp_triple);
            ps_index = new IndexCascading<int>(path + "ps_index") 
            {
                Table = table,
                Key1Producer = ob => (int)((object[])((object[])ob)[1])[1],
                Key2Producer = ob => (int)((object[])((object[])ob)[1])[0],
                Half2Producer = i => i
            };
            po_index = new IndexCascading<ObjectVariants>(path + "po_index")
            {
                Table = table,
                Key1Producer = ob => (int)((object[])((object[])ob)[1])[1],
                Key2Producer = ob => ((object[])((object[])ob)[1])[2].ToOVariant(),
                Half2Producer = ov => ov.GetHashCode()
            };
        }
        public void Start() 
        { 
            //ps_index.CreateGroupDictionary();
            ps_index.CreateDiscaleDictionary();
            po_index.CreateDiscaleDictionary();
        }
        public int Code(string s) { return nametable.GetCodeByString(s); }
        public string Decode(int c) { return nametable.GetStringByCode(c); }
        
        public void Clear() { table.Clear(); }
        public void Warmup() { table.Warmup(); }

        public void Build(IEnumerable<Tuple<string, string, ObjectVariants>> triples)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
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
            if (buff.Count > 0) ProcessPortion(buff);
            table.TableCell.Flush();

            nametable.BuildScale();

            sw.Stop();
            Console.WriteLine("Load data and nametable ok. Duration={0}", sw.ElapsedMilliseconds);
            sw.Restart();

            ps_index.Build();

            sw.Stop();
            Console.WriteLine("ps_index.Build() ok. Duration={0}", sw.ElapsedMilliseconds);
            sw.Restart();

            po_index.Build();
            
            sw.Stop();
            Console.WriteLine("Build index ok. Duration={0}", sw.ElapsedMilliseconds);
            sw.Restart();
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
        public IEnumerable<PaEntry> GetTriples()
        {
            var qu = ps_index.GetAll();
            return qu;
        }
        public IEnumerable<PaEntry> GetTriplesWithPredicateSubject(int pred, int subj)
        {
            var qu = ps_index.GetAllByKeys(pred, subj);
            return qu;
        }
        public IEnumerable<PaEntry> GetTriplesWithPredicateObject(int pred, ObjectVariants obj)
        {
            var qu = po_index.GetAllByKeys(pred, obj);
            return qu;
        }
        public object Dereference(PaEntry ent) { return ((object[])ent.Get())[1]; }
    }
}
