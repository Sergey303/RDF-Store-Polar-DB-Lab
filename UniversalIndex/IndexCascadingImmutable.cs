﻿using System;
using System.Collections.Generic;
using System.Linq;
using PolarDB;

namespace Task15UniversalIndex
{
    public class IndexCascadingImmutable<Tkey> : IIndexCommon where Tkey : IComparable
    {
        // Tkey - тип второго ключа, int - тип первого ключа
        private PaCell index_cell;
        public PaCell IndexCell { get { return index_cell; } }
        private PaCell groups_index;
        public IndexCascadingImmutable(string path_name)
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
        public Func<Tkey, int> Half2Producer { get; set; } // Второй ключ -> полуключ (возможно тождественное), если Half2Producer==null, значит используется ключ
        public IBearingTableImmutable Table { get; set; }

        //internal class GroupElement : IComparable
        //{
        //    public int Key1 { get { return this.key1; } }
        //    public int HKey2 { get { return this.hkey2; } }
        //    private int key1;
        //    private int hkey2;
        //    private Func<Tkey> GetKey2 = null;
        //    internal GroupElement(int key1, int hkey2, Func<Tkey> getkey2)
        //    {
        //        this.key1 = key1;
        //        this.hkey2 = hkey2;
        //        this.GetKey2 = getkey2;
        //    }
        //    private bool key2exists = false;
        //    private Tkey key2;
        //    private Tkey Key2
        //    {
        //        get
        //        {
        //            if (!key2exists)
        //            {
        //                key2exists = true;
        //                key2 = GetKey2();
        //            }
        //            return key2;
        //        }
        //    }
        //    public int CompareTo(object obj)
        //    {
        //        GroupElement rec = (GroupElement)obj;
        //        int cmp = key1.CompareTo(rec.Key1);
        //        if (cmp != 0) return cmp;
        //        cmp = hkey2.CompareTo(rec.HKey2);
        //        if (cmp != 0) return cmp;
        //        //Tkey key2 = GetKey2();
        //        //cmp = key2.CompareTo(rec.GetKey2());
        //        cmp = Key2.CompareTo(rec.Key2);
        //        return cmp;
        //    }
        //}

        // Если не найден, то будет Diapason.Empty
        public Diapason GetDiapasonByKey1(int key1)
        {
            Tuple<Diapason, ScaleInMemory> tup;
            if (gr_discale.TryGetValue(key1, out tup))
            {
                return tup.Item1;
            }
            else return Diapason.Empty;
        }
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
        public IEnumerable<object> GetAllByKeys(int key1, Tkey key2)
        {
            Diapason dia = GetLocalDiapason(key1, key2);
             return GetAllInDiap(dia, key2);
        }
        public IEnumerable<int> GetKey1All()
        {
            return gr_discale.Keys;
        }
        /// <summary>
        /// Получение триплетов из диапазона, соответствующих заданному второму ключу
        /// </summary>
        /// <param name="dia">Диапазон в индексном массиве. Есть специальные требования к диапазону</param>
        /// <param name="key2">Ключ2 по которому фильтруется результарующее множество</param>
        /// <returns>Возвращает поток триплетов в объектной форме</returns>
        public IEnumerable<object> GetAllInDiap(Diapason dia, Tkey key2)
        {
            if (dia.IsEmpty()) return Enumerable.Empty<object>();
            int hkey = Half2Producer(key2);
            PaEntry entry = Table.Element(0);
            var query1 = index_cell.Root.BinarySearchAll(dia.start, dia.numb, ent =>
            {
                object[] va = (object[])ent.Get();
                int hk = (int)va[2];
                int cmp = hk.CompareTo(hkey);
                return cmp;
            }).Select(ent => ent.Get())
                .Where(va => (int)((object[])va)[2] == hkey);
            var query2 = index_cell.Root.ElementValues(dia.start, dia.numb)
                .Where(va => (int)((object[])va)[2] == hkey);
            IEnumerable<object> query = dia.numb > 30 ? query1 : query2;

            return query
                .Select(va =>
                {
                    long off = (long)((object[])va)[0];
                    entry.offset = off;
                    return entry.Get();
                })
                .Where(two => !(bool)((object[])two)[0] && Key2Producer(two).CompareTo(key2) == 0)
                .Select(two => ((object[])((object[])two)[1]));
        }
        public IEnumerable<object> GetAllInDiap(Diapason dia)
        {
            if (dia.IsEmpty()) return Enumerable.Empty<object>();
            PaEntry entry = Table.Element(0);

            var query2 = index_cell.Root.ElementValues(dia.start, dia.numb)
                .Select(va =>
                {
                    long off = (long)((object[])va)[0];
                    entry.offset = off;
                    return entry.Get();
                })
                .Where(two => !(bool)((object[])two)[0])
                .Select(two => ((object[])((object[])two)[1]));
            return query2;
        }
        // Получение потока всех элементов в отсортированном виде
        // Альтернатива - выдача всех записей из таблицы Table
        public IEnumerable<object> GetAll()
        {
            PaEntry entry = Table.Element(0);
            return index_cell.Root.ElementValues()
                .Select(va =>
                {
                    long off = (long)((object[])va)[0];
                    entry.offset = off;
                    return entry.Get();
                })
                .Where(two => !(bool)((object[])two)[0]) // Проверка на неуничтоженность
                .Select(two => ((object[])((object[])two)[1]));
        }

        internal class HKeyKey : IComparable
        {
            public int HKey2 { get { return this.hkey2; } }
            private int hkey2;
            private Func<Tkey> GetKey2 = null;
            internal HKeyKey(int hkey2, Func<Tkey> getkey2)
            {
                this.hkey2 = hkey2;
                this.GetKey2 = getkey2;
            }
            private bool key2exists = false;
            private Tkey key2;
            private Tkey Key2
            {
                get
                {
                    if (!key2exists)
                    {
                        key2exists = true;
                        key2 = GetKey2();
                    }
                    return key2;
                }
            }
            public int CompareTo(object obj)
            {
                HKeyKey rec = (HKeyKey)obj;
                int cmp;
                cmp = hkey2.CompareTo(rec.HKey2);
                if (cmp != 0) return cmp;
                //Tkey key2 = GetKey2();
                //cmp = key2.CompareTo(rec.GetKey2());
                cmp = Key2.CompareTo(rec.Key2);
                return cmp;
            }
        }
        public void Build()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();            sw.Restart();
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
            sw.Stop(); Console.WriteLine("Формирование index_cell ok. Duration={0}", sw.ElapsedMilliseconds);

            sw.Restart();
            PaEntry entry = Table.Element(0);
            // Сортировка по ключу1
            index_cell.Root.SortByKey<int>(ob => (int)((object[])ob)[1]);
            sw.Stop(); Console.WriteLine("Сортировака по первому ключу ok. Duration={0}", sw.ElapsedMilliseconds);

            sw.Restart();
            groups_index.Clear();
            groups_index.Fill(new object[0]);
            int key1 = Int32.MinValue;
            int i = 0; // Теоретически, здесь есть проблема в том, что элементы могут выдаватьс не по индексу.
            int previous_i = -1;
            foreach (object[] va in index_cell.Root.ElementValues())
            {
                int k1 = (int)va[1];
                if (k1 > key1)
                {
                    groups_index.Root.AppendElement(i);
                    key1 = k1;
                    if (i > 0)
                    {
                        if (typeof(Tkey) == 33.GetType())
                            index_cell.Root.SortByKey<int>(previous_i, i - previous_i, ob => (int)((object[])ob)[2], null);
                        else
                            index_cell.Root.SortByKey<HKeyKey>(previous_i, i - previous_i,
                                (object ob) => new HKeyKey((int)((object[])ob)[2], 
                                    () =>
                                    {
                                        long off = (long)((object[])ob)[0];
                                        entry.offset = off;
                                        return Key2Producer(entry.Get());
                                    }),
                                    null);
                    }
                    previous_i = i;
                }
                i++;
            }
            if (typeof(Tkey) == 33.GetType())
                index_cell.Root.SortByKey<int>(previous_i, i - previous_i, ob => (int)((object[])ob)[2], null);
            else
                index_cell.Root.SortByKey<HKeyKey>(previous_i, i - previous_i,
                    (object ob) => new HKeyKey((int)((object[])ob)[2],
                        () =>
                        {
                            long off = (long)((object[])ob)[0];
                            entry.offset = off;
                            return Key2Producer(entry.Get());
                        }),
                        null);
            groups_index.Flush();
            index_cell.Flush();
            sw.Stop(); Console.WriteLine("Создание groups_index ok. Duration={0}", sw.ElapsedMilliseconds);

            CreateDiscaleDictionary();
        }
        void Warmup() { throw new NotImplementedException("in IndexCascadingImmutable"); }
        public void ActivateCache() { index_cell.ActivateCache(); groups_index.ActivateCache(); }

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
                    gr_discale.Add(key, new Tuple<Diapason, ScaleInMemory>(
                        new Diapason() { start = sta, numb = num }, sim));
                }
                key = Key1Producer(entry.Get());
                start0 = start;
            }
            sta = start0;
            num = index_cell.Root.Count() - start0;
            nscale = num / 32;
            ScaleInMemory sim0 = new ScaleInMemory(index_cell.Root, sta, num, ob => (int)((object[])ob)[2], (int)nscale);
            sim0.Build();
            gr_discale.Add(key, new Tuple<Diapason, ScaleInMemory>(
                new Diapason() { start = sta, numb = num }, sim0));
        }

        public void OnAppendElement(PolarDB.PaEntry entry) { throw new NotImplementedException(); }
        public void DropIndex() { throw new NotImplementedException(); }
    }
}
