using System;
using System.Collections.Generic;
using System.Linq;
using PolarDB;

namespace Task15UniversalIndex
{
    public class IndexCascading<Tkey> /*: IIndexImmutable<Tkey>*/ where Tkey : IComparable
    {
        // Tkey - тип второго ключа
        private PaCell index_cell;
        public PaCell IndexCell { get { return index_cell; } }
        private PaCell groups_index;
        private bool usekey = true;
        public IndexCascading(string path_name)
        {
            PType tp_record = new PTypeRecord(
                new NamedType("offset", new PType(PTypeEnumeration.longinteger)),
                new NamedType("key1", new PType(PTypeEnumeration.integer)),
                new NamedType("keyhkey2", new PType(PTypeEnumeration.integer)));
            index_cell = new PaCell(new PTypeSequence(tp_record),
                path_name + "_2.pac", false);
            groups_index = new PaCell(new PTypeSequence(new PType(PTypeEnumeration.integer)),
                path_name + "_g.pac", false);

        }
        public Func<object, int> Key1Producer { get; set; }
        public Func<object, Tkey> Key2Producer { get; set; }
        public Func<Tkey, int> Half2Producer { get; set; } // Второй ключ -> полуключ
        public IBearingTableImmutable Table { get; set; }
        public void Build()
        {
            index_cell.Clear();
            index_cell.Fill(new object[0]);
            if (Key1Producer == null) throw new Exception("Err: Key1Producer not defined");
            if (Key2Producer == null) throw new Exception("Err: Key2Producer not defined");
            Table.Scan((offset, o) =>
            {
                var k1 = Key1Producer(o);
                var k2 = Key2Producer(o);
                int hk2 = Half2Producer(k2);
                index_cell.Root.AppendElement(new object[] { offset, k1, hk2 });
                return true;
            });
            index_cell.Flush();

            PaEntry entry = Table.Element(0);
            index_cell.Root.SortByKey<GroupElement>(ob => 
                new GroupElement((int)((object[])ob)[1], (int)((object[])ob)[2], () => 
                {
                    long off = (long)((object[])ob)[0];
                    entry.offset = off;
                    return Key2Producer(entry.Get());
                }));
            BuildGroupsIndexSpecial();
        }

        private void BuildGroupsIndexSpecial()
        {
            groups_index.Clear();
            groups_index.Fill(new object[0]);
            int key1 = Int32.MinValue;
            int i = 0; // Теоретически, здесь есть проблема в том, что элементы могут выдаватьс не по индексу.
            foreach (object[] va in index_cell.Root.ElementValues())
            {
                int k1 = (int)va[1];
                if (k1 > key1)
                {
                    groups_index.Root.AppendElement(i);
                    key1 = k1;
                }
                i++;
            }
            groups_index.Flush();
            //CreateGroupDictionary();
            CreateDiscaleDictionary();
        }
        //// Первый вариант группового словаря: на каждый key1 получаем диапазон в index_cell "его" значений индекса
        //private Dictionary<int, Diapason> gr_dic = null;
        //public void CreateGroupDictionary() 
        //{
        //    gr_dic = new Dictionary<int, Diapason>();
        //    PaEntry entry = Table.Element(0);
        //    long start0 = -1;
        //    long start = -1;
        //    int key = -1;
        //    foreach (int ind in groups_index.Root.ElementValues()) 
        //    {
        //        start = ind;
        //        long off = (long)index_cell.Root.Element(ind).Field(0).Get();
        //        entry.offset = off;
        //        if (key != -1)
        //        {
        //            gr_dic.Add(key, new Diapason() { start = start0, numb = start - start0 });
        //        }
        //        key = Key1Producer(entry.Get());
        //        start0 = start;
        //    }
        //    gr_dic.Add(key, new Diapason() { start = start0, numb = groups_index.Root.Count() - start0 });
        //}
        // Второй вариант группового словаря: получаем пару - диапазон и ScaleInMemory
        private Dictionary<int, Tuple<Diapason, ScaleInMemory>> gr_discale = null;
        public void CreateDiscaleDictionary()
        {
            gr_discale = new Dictionary<int, Tuple<Diapason, ScaleInMemory>>();
            PaEntry entry = Table.Element(0);
            long start0 = -1;
            long start = -1;
            int key = -1;
            long sta = -1, num = -1, nscale = -1;
            foreach (int ind in groups_index.Root.ElementValues())
            {
                start = ind;
                long off = (long)index_cell.Root.Element(ind).Field(0).Get();
                entry.offset = off;
                if (key != -1)
                {
                    sta = start0;
                    num = start - start0; 
                    nscale = num / 32;
                    ScaleInMemory sim = new ScaleInMemory(index_cell.Root, sta, num, ob => (int)((object[])ob)[2], (int)nscale);
                    sim.Build();
                    gr_discale.Add(key, new Tuple<Diapason, ScaleInMemory> ( 
                        new Diapason() { start = sta, numb = num }, sim));
                }
                key = Key1Producer(entry.Get());
                start0 = start;
            }
            sta = start0;
            num = groups_index.Root.Count() - start0;
            nscale = num / 32;
            ScaleInMemory sim0 = new ScaleInMemory(index_cell.Root, sta, num, ob => (int)((object[])ob)[2], (int)nscale);
            sim0.Build();
            gr_discale.Add(key, new Tuple<Diapason, ScaleInMemory>(
                new Diapason() { start = sta, numb = num }, sim0));
        }

        public IEnumerable<PaEntry> GetAll()
        {
            PaEntry entry = Table.Element(0);
            return index_cell.Root.Elements()
                .Select(ent =>
                {
                    long off = (long)ent.Field(0).Get();
                    entry.offset = off;
                    return entry;
                });
        }
        //// Если не найден, то будет Diapason.Empty
        //private Diapason GetDiapasonByKey1(int key1)
        //{
        //    Diapason dia;
        //    if (gr_dic.TryGetValue(key1, out dia)) return dia;
        //    else return Diapason.Empty;
        //}
        private Diapason GetLocalDiapason(int key1, Tkey key2)
        {
            Tuple<Diapason, ScaleInMemory> tup;
            if (gr_discale.TryGetValue(key1, out tup))
            {
                int hk = Half2Producer(key2);
                return tup.Item2.GetDiapason(hk);
            }
            else return Diapason.Empty;
        }
        public IEnumerable<PaEntry> GetAllByKeys(int key1, Tkey key2)
        {
            //Diapason dia = GetDiapasonByKey1(key1);
            Diapason dia = GetLocalDiapason(key1, key2);
            return GetAllInDiap(dia, key2);
        }
        public IEnumerable<PaEntry> GetAllInDiap(Diapason dia, Tkey key2)
        {
            if (dia.IsEmpty()) return Enumerable.Empty<PaEntry>();
            int hkey = Half2Producer(key2);
            PaEntry entry = Table.Element(0);
            var query = index_cell.Root.BinarySearchAll(dia.start, dia.numb, ent =>
            {
                object[] va = (object[])ent.Get();
                int hk = (int)va[2];
                int cmp = hk.CompareTo(hkey);
                if (cmp != 0) return cmp;
                long off = (long)va[0];
                entry.offset = off;
                object ob = entry.Get();
                cmp = Key2Producer(ob).CompareTo(key2);
                return cmp;
            })
            .ToArray()
            ;
            return query.Select(ent =>
            {
                long off = (long)ent.Field(0).Get();
                entry.offset = off;
                return entry;
            });
        }
        internal class TwoKeys : IComparable 
        {
            public int Key1 { get { return this.key1; } }
            public Tkey Key2 { get { return this.key2; } }
            private int key1;
            private Tkey key2;
            internal TwoKeys(int key1, Tkey key2)
            {
                this.key1 = key1;
                this.key2 = key2;
            }
            public int CompareTo(object obj)
            {
                TwoKeys tk = (TwoKeys)obj;
                int cmp = key1.CompareTo(tk.Key1);
                if (cmp == 0) cmp = key2.CompareTo(tk.Key2);
                return cmp;
            }
        }
        internal class GroupElement : IComparable
        {
            public int Key1 { get { return this.key1; } }
            public int HKey2 { get { return this.hkey2; } }
            private int key1;
            private int hkey2;
            private Func<Tkey> GetKey2 = null;
            internal GroupElement(int key1, int hkey2, Func<Tkey> getkey2)
            {
                this.key1 = key1;
                this.hkey2 = hkey2;
                this.GetKey2 = getkey2;
            }
            public int CompareTo(object obj)
            {
                GroupElement rec = (GroupElement)obj;
                int cmp = key1.CompareTo(rec.Key1);
                if (cmp != 0) return cmp;
                cmp = hkey2.CompareTo(rec.HKey2);
                if (cmp != 0) return cmp;
                Tkey key2 = GetKey2();
                cmp = key2.CompareTo(rec.GetKey2());
                return cmp;
            }
        }
    }
}
