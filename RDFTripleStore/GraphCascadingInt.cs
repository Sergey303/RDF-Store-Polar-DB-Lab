using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PolarDB;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using RDFTurtleParser;
using Task15UniversalIndex;


namespace RDFTripleStore
{
    public class GraphCascadingInt :  IGraph
    {
        private readonly NodeGeneratorInt ng;

        public void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            throw new NotImplementedException();
        }
        public bool Contains(ObjectVariants subj, ObjectVariants pred, ObjectVariants obj)
        {
            return GetTriplesWithSubjectPredicate(subj, pred)
                .Any(o => o.CompareTo(obj) == 0);
        }

        public void Delete(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetAllSubjects()
        {
            throw new NotImplementedException();
        }

        public long GetTriplesCount()
        {
            throw new NotImplementedException();
        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public void FromTurtle(string gString)
        {
            Build(new TripleGeneratorBufferedParallel(gString,null));
        }

        public void FromTurtle(Stream inputStream)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetTriples<T>(Func<ObjectVariants,ObjectVariants,ObjectVariants, T> returns)
        {
            return ps_index.GetRecordsAll()
                .Cast<object[]>()
                .Select(rec => returns(new OV_iriint((int)rec[0], ng.coding_table.GetStringByCode), new OV_iriint((int)rec[1], ng.coding_table.GetStringByCode), rec[2].ToOVariant(ng.coding_table.GetStringByCode)));
        }

  
        public IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants subj, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return ps_index.GetRecordsWithKey2(((OV_iriint)subj).code)
                .Cast<object[]>()
                 .Select(rec => createResult( new OV_iriint((int) rec[1], ng.coding_table.GetStringByCode),rec[2].ToOVariant(ng.coding_table.GetStringByCode)));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            return ps_index.GetRecordsWithKeys(((OV_iriint)pred).code, ((OV_iriint)subj).code)
                .Cast<object[]>()
                .Select(rec => rec[2].ToOVariant(ng.coding_table.GetStringByCode));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            return po_index.GetRecordsWithKeys(((OV_iriint) pred).code, obj)
                .Cast<object[]>()
                .Select(rec => new OV_iriint((int) rec[0], ng.coding_table.GetStringByCode));
        }
            public IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants obj, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return po_index.GetRecordsWithKey2(obj)
                .Cast<object[]>()
              .Select(rec => createResult(new OV_iriint((int) rec[0], ng.coding_table.GetStringByCode), new OV_iriint((int) rec[1], ng.coding_table.GetStringByCode)));
        }

        public IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants pred, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return ps_index.GetRecordsWithKey1(((OV_iriint)pred).code)
                .Cast<object[]>()
                .Select(rec => createResult(new OV_iriint((int) rec[0], ng.coding_table.GetStringByCode),rec[2].ToOVariant(ng.coding_table.GetStringByCode)));
        }
    

      
        private TableView table;
        public TableView Table { get { return table; } }
        private IndexCascadingDynamic<int> ps_index;
        private IndexCascadingDynamic<ObjectVariants> po_index;
        public GraphCascadingInt(string path)
        {
            PType tp_triple = new PTypeRecord(
                new NamedType("subj", new PType(PTypeEnumeration.integer)),
                new NamedType("pred", new PType(PTypeEnumeration.integer)),
                new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            table = new TableView(path + "triples", tp_triple);
            ng = NodeGeneratorInt.Create(path, table.TableCell.IsEmpty);
            ps_index = new IndexCascadingDynamic<int>(path + "ps_index",
                table,
                ob => (int)((object[])((object[])ob)[1])[1],
                ob => (int)((object[])((object[])ob)[1])[0],
                i => i);
            po_index = new IndexCascadingDynamic<ObjectVariants>(path + "po_index",
                table,
                ob => (int)((object[])((object[])ob)[1])[1],
                ob => ((object[])((object[])ob)[1])[2].ToOVariant(ng.coding_table.GetStringByCode),
                ov => ov.GetHashCode());
        }
        public void Start() 
        { 
            ps_index.CreateDiscaleDictionary();
            po_index.CreateDiscaleDictionary();
        }
        public int Code(string s) { return ng.coding_table.GetCodeByString(s); }

        public string Name { get; private set; }
        public NodeGenerator NodeGenerator { get { return ng; } }
        public void Clear() { table.Clear(); }
      

        public void Warmup() { table.Warmup(); }

        public void Build(IGenerator<List<TripleStrOV>> generator)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
          ng.Clear();
            table.Clear();
            table.Fill(new object[0]);
            
            generator.Start(ProcessPortion);
            table.TableCell.Flush();

            ng.Build();

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

        private void ProcessPortion(List<TripleStrOV> buff)
        {
            // Пополнение таблицы имен
            var dic = ng.coding_table.InsertPortion(buff.SelectMany(t =>
            {
                ObjectVariants ov = t.Object;
                if (ov.Variant == ObjectVariantEnum.Iri)
                {
                    return new string[] { t.Subject, t.Predicate, ((OV_iri)ov).Name };
                }
                else
                {
                    return new string[] { t.Subject, t.Predicate };
                }
            }));
            // Пополнение триплетов
            table.Add(buff.Select(t =>
            {
                ObjectVariants ov = t.Object;
                if (ov.Variant == ObjectVariantEnum.Iri)
                {
                    OV_iriint ovi = new OV_iriint(dic[((OV_iri)ov).Name], ng.coding_table.GetStringByCode);
                    return new object[] { dic[t.Subject], dic[t.Predicate], ovi.ToWritable() };
                }
                else
                {
                    return new object[] { dic[t.Subject], dic[t.Predicate], ov.ToWritable() };
                }
            }));
        }
        public void Build(IEnumerable<TripleStrOV> triples)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            ng.Clear();

            table.Clear();
            table.Fill(new object[0]);
            int portion = 1000000;
            List<TripleStrOV> buff = new List<TripleStrOV>();
            foreach (TripleStrOV tri in triples)
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

            ng.Build();

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
    }
}
