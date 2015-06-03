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
        public IndexCascading(string path_name)
        {
            index_cell = new PaCell(new PTypeSequence(new PType(PTypeEnumeration.longinteger)),
                path_name + "_2.pac", false);
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
        }
        internal class TwoKeys : IComparable
        {
            public int Key1 { get { return this.key1; } }
            public Tkey Key2 { get { return this.key2; } }
            private int key1;
            private Tkey key2;
            TwoKeys(int key1, Tkey key2) 
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
