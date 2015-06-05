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
        public IndexCascading(string path_name)
        {
            index_cell = new PaCell(new PTypeSequence(new PType(PTypeEnumeration.longinteger)),
                path_name + "_2.pac", false);
            groups_index = new PaCell(new PTypeSequence(new PType(PTypeEnumeration.integer)),
                path_name + "_g.pac", false);
        }
        public Func<object, int> Key1Producer { get; set; }
        public Func<object, Tkey> Key2Producer { get; set; }
        public IBearingTableImmutable Table { get; set; }
        public void Build()
        {
            index_cell.Clear();
            index_cell.Fill(new object[0]);
            if (Key1Producer == null) throw new Exception("Err: Key1Producer not defined");
            if (Key2Producer == null) throw new Exception("Err: Key2Producer not defined");
            Table.Scan((offset, o) =>
            {
                //var key = KeyProducer(o);
                index_cell.Root.AppendElement(offset);
                return true;
            });
            index_cell.Flush();
            PaEntry entry = Table.Element(0);
            index_cell.Root.SortByKey(o =>
            {
                long off = (long)o;
                entry.offset = off;
                object ob = entry.Get();
                int k1 = Key1Producer(ob);
                Tkey k2 = Key2Producer(ob);
                return new TwoKeys(k1, k2);
            });
            groups_index.Clear();
            groups_index.Fill(new object[0]);
            int key1 = Int32.MinValue;
            for (int i=0; i<index_cell.Root.Count(); i++)
            {
                long off = (long)index_cell.Root.Element(i).Get();
                entry.offset = off;
                int k1 = Key1Producer(entry.Get());
                if (k1 > key1)
                {
                    groups_index.Root.AppendElement(i);
                }
            }
            groups_index.Flush();
        }
        public IEnumerable<PaEntry> GetAll()
        {
            PaEntry entry = Table.Element(0);
            return index_cell.Root.Elements()
                .Select(ent =>
                {
                    long off = (long)ent.Get();
                    entry.offset = off;
                    return entry;
                });
        }
        // Если не найден, то будет Diapason.Empty
        public Diapason GetDiapasonByKey1(int key1)
        {
            if (Table.Count() == 0) return Diapason.Empty;
            PaEntry entry = Table.Element(0);
            PaEntry gr_ent0 = groups_index.Root.Element(0);
            int gr_el_size = gr_ent0.Type.HeadSize; // размер элементов массива групп
            long gr_off0 = gr_ent0.offset; // оффсет нулевого элемента массива групп
            // Вычисление диапазона
            //PaEntry gr_entry = groups_index.Root.BinarySearchFirst(ent =>
            //{
            //    int sta = (int)ent.Get();
            //    long off = (long)index_cell.Root.Element(sta).Get();
            //    object ob = entry.Get();
            //    int cmp = Key1Producer(ob).CompareTo(key1);
            //    return cmp;
            //});
            PaEntry gr_entry = PaEntry.Empty;
            for (int nd = 0; nd < groups_index.Root.Count(); nd++)
            {
                PaEntry ent = groups_index.Root.Element(nd);

                int k1 = Key1Producer(
            }
            if (gr_entry.IsEmpty) return Diapason.Empty; // Видимо нет первого ключа
            int ind = (int)((gr_entry.offset - gr_off0) / gr_el_size);
            int nextind = ind++;
            int start = (int)gr_entry.Get();
            int nextstart = (long)nextind < groups_index.Root.Count() ?
                (int)groups_index.Root.Element(nextind).Get() :
                (int)index_cell.Root.Count();
            int number = nextstart - start;
            return new Diapason() { start = start, numb = number };
        }
        public IEnumerable<PaEntry> GetAllByKeys(int key1, Tkey key2)
        {
            Diapason dia = GetDiapasonByKey1(key1);
            if (dia.IsEmpty()) return Enumerable.Empty<PaEntry>();
            PaEntry entry = Table.Element(0);
            var query = index_cell.Root.BinarySearchAll(dia.start, dia.numb, ent =>
            {
                long off = (long)ent.Get();
                entry.offset = off;
                object ob = entry.Get();
                int cmp = Key2Producer(ob).CompareTo(key2);
                return cmp;
            });
            return query;
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
    }
}
