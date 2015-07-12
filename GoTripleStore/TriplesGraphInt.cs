using System;
using System.Collections.Generic;
using System.Linq;
using PolarDB;
using Task15UniversalIndex;
using RDFCommon.OVns;

namespace GoTripleStore
{
    public class TriplesGraphInt : IGraph
    {
        public bool Contains(ObjectVariants subj, ObjectVariants pred, ObjectVariants obj)
        {
            return GetTriplesWithSubjectPredicate(subj, pred)
                .Any(tr => tr.Obj.CompareTo(obj) == 0);
        }
        private Triple ToTriple(object rec)
        {
            object[] r = (object[])rec;
            return new Triple(
                    new OV_iriint((int)r[0], Decode),
                    new OV_iriint((int)r[1], Decode),
                    r[2].ToOVariant());
        }
        public IEnumerable<Triple> GetTriples()
        {
            return ps_index.GetRecordsAll()
                .Select(rec => ToTriple(rec));
        }
        public IEnumerable<Triple> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            return ps_index.GetRecordsWithKeys(((OV_iriint)pred).code, ((OV_iriint)subj).code)
                .Select(rec => ToTriple(rec));
        }
        public IEnumerable<Triple> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            return po_index.GetRecordsWithKeys(((OV_iriint)pred).code, obj)
                .Select(rec => ToTriple(rec));
        }

        public IEnumerable<Triple> GetTriplesWithSubject(ObjectVariants subj)
        {
            return ps_index.GetRecordsWithKey2(((OV_iriint)subj).code)
                .Select(rec => ToTriple(rec));
        }
        public IEnumerable<Triple> GetTriplesWithPredicate(ObjectVariants pred)
        {
            return ps_index.GetRecordsWithKey1(((OV_iriint)pred).code)
                .Select(rec => ToTriple(rec));
        }
        public IEnumerable<Triple> GetTriplesWithObject(ObjectVariants obj)
        {
            return po_index.GetRecordsWithKey2(obj)
                .Select(rec => ToTriple(rec));
        }


        private NameTableUniversal nametable;
        private TableView table;
        public TableView Table { get { return table; } }
        private IndexCascadingDynamic<int> ps_index;
        private IndexCascadingDynamic<ObjectVariants> po_index;
        public TriplesGraphInt(string path)
        {
            PType tp_triple = new PTypeRecord(
                new NamedType("subj", new PType(PTypeEnumeration.integer)),
                new NamedType("pred", new PType(PTypeEnumeration.integer)),
                new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            nametable = new NameTableUniversal(path);
            table = new TableView(path + "triples", tp_triple);
            ps_index = new IndexCascadingDynamic<int>(path + "ps_index",
                table,
                ob => (int)((object[])((object[])ob)[1])[1],
                ob => (int)((object[])((object[])ob)[1])[0],
                i => i);
            po_index = new IndexCascadingDynamic<ObjectVariants>(path + "po_index",
                table,
                ob => (int)((object[])((object[])ob)[1])[1],
                ob => ((object[])((object[])ob)[1])[2].ToOVariant(),
                ov => ov.GetHashCode());
        }
        public void Start() 
        { 
            ps_index.CreateDiscaleDictionary();
            po_index.CreateDiscaleDictionary();
        }
        public void ActivateCache()
        {
            nametable.ActivateCache();
            table.ActivateCache();
            ps_index.ActivateCache();
            po_index.ActivateCache();
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
    }
}
