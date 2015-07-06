using System;
using System.Collections.Generic;
using System.Linq;
using PolarDB;

namespace PolarDB2
{
    public struct PaEntry2
    {
        public long offset;
        private PType tp;
        public PType Type { get { return tp; } }
        private PaCell2 cell;
        
        // Пустое значение. Часто используется в качестве указания, что вход не найден: 
        // var found = entry.Where(en => predicate).DefaultIfEmpty(PaEntry2.Empty).First(); if (found.IsEmpty()) ...
        private static PaEntry2 _empty = new PaEntry2() { cell = null, offset = Int64.MinValue, tp = null };
        public static PaEntry2 Empty { get { return _empty; } }
        public bool IsEmpty { get { return offset == Int64.MinValue; } }
        
        public PaEntry2(PType tp, long offset, PaCell2 cell)
        {
            this.tp = tp;
            this.offset = offset;
            this.cell = cell;
        }

        // ===============================================
        // ============== Методы доступа =================
        // ===============================================

        // ========== Для записей =========
        public PaEntry2 Field(int index)
        {
            if (tp.Vid != PTypeEnumeration.record) throw new Exception("Err in TPath formula: Field can't be applied to structure of vid " + tp.Vid);
            PTypeRecord mtr = (PTypeRecord)tp;
            if (index >= mtr.Fields.Length) throw new Exception("Err in TPath formula: index of Field is too large " + index);

            long pos = this.offset;
            for (int i = 0; i < index; i++)
            {
                PType t = mtr.Fields[i].Type;
                if (t.HasNoTail) pos += t.HeadSize;
                else
                {
                    pos = Skip(t, pos);
                }
            }
            return new PaEntry2(mtr.Fields[index].Type, pos, cell);
        }
        // ========== Для последовательностей =========
        // Подсчитывает число элементов последовательности
        public long Count()
        {
            if (tp.Vid != PTypeEnumeration.sequence) throw new Exception("Err in TPath formula: Count() can't be applyed to structure of vid " + tp.Vid);
            // Для внешних последовательностей длину берем из объекта cell
            if (this.offset == this.cell.Root.offset) return this.cell.NElements;
            // Для остальных - двойное целое вначале
            return cell.ReadLong(this.offset);
        }
        public PaEntry2 Element(long index)
        {
            if (tp.Vid != PTypeEnumeration.sequence) throw new Exception("Err in TPath formula: Element() can't be applyed to structure of vid " + tp.Vid);
            PTypeSequence mts = (PTypeSequence)tp;
            PType t = mts.ElementType;
            long llen = this.Count();
            if (index < 0 || index >= llen) throw new Exception("Err in TPath formula: wrong index of Element " + index);
            // для внешних равноэлементных последовательностей - специальная формула
            if (t.HasNoTail || index == 0) return new PaEntry2(t, this.offset + 8 + index * t.HeadSize, cell);
            //cell.SetOffset(this.offset); //Убрал
            long pos = this.offset + 8;
            for (long ii = 0; ii < index; ii++)
            {
                pos = Skip(t, pos);
            }
            return new PaEntry2(t, pos, cell);
        }
        // TODO: Рассмотреть целесообразность этого метода
        private PaEntry2 ElementUnchecked(long index)
        {
            PType t = ((PTypeSequence)tp).ElementType;
            if (t.HasNoTail) return new PaEntry2(t, this.offset + 8 + index * t.HeadSize, cell);
            long pos = this.offset + 8;
            for (long ii = 0; ii < index; ii++)
            {
                pos = Skip(t, pos);
            }
            return new PaEntry2(t, pos, cell);
        }
        public IEnumerable<PaEntry2> Elements()
        {
            return Elements(0, this.Count());
        }
        public IEnumerable<PaEntry2> Elements(long start, long number)
        {
            if (number > 0)
            {
                if (tp.Vid != PTypeEnumeration.sequence) throw new Exception("Err in TPath formula: Elements() can't be applyed to structure of vid " + tp.Vid);
                PTypeSequence mts = (PTypeSequence)tp;
                PType t = mts.ElementType;
                PaEntry2 element = this.Element(start);
                if (t.HasNoTail)
                {
                    int size = t.HeadSize;
                    for (long ii = 0; ii < number; ii++)
                    {
                        yield return element;
                        element.offset += size;
                    }
                }
                else
                {
                    long offset = element.offset;
                    for (long ii = 0; ii < number; ii++)
                    {
                        element.offset = offset;
                        if (ii < number - 1) offset = element.Skip(t, offset);
                        yield return element;
                    }
                }
            }
        }
        // ========== Для объединений =========
        public int Tag()
        {
            if (tp.Vid != PTypeEnumeration.union) throw new Exception("Err: Tag() needs union");
            return cell.ReadByte(this.offset);
        }
        public PaEntry2 UElement()
        {
            int tag = this.Tag();
            PTypeUnion ptu = ((PTypeUnion)this.tp);
            if (tag < 0 || tag >= ptu.Variants.Length) throw new Exception("Err: tag is out of bound");
            PType tel = ptu.Variants[tag].Type;
            return new PaEntry2(tel, offset + 1, cell);
        }
        public PaEntry2 UElementUnchecked(int tag)
        {
            PTypeUnion ptu = ((PTypeUnion)this.tp);
            if (tag < 0 || tag >= ptu.Variants.Length) throw new Exception("Err: tag is out of bound");
            PType tel = ptu.Variants[tag].Type;
            return new PaEntry2(tel, offset + 1, cell);
        }

        public PValue GetValue()
        {
            return new PValue(this.tp, this.offset, Get());
        }
        public object Get()
        {
            return cell.GetPObject(tp, this.offset);
        }
        // Следующие два метода не стоит применять в режимах отложенных вычислений. Есть побочный эффект
        public IEnumerable<object> ElementValues()
        {
            if (tp.Vid != PTypeEnumeration.sequence) throw new Exception("Err in TPath formula: ElementValues() can't be applyed to structure of vid " + tp.Vid);
            PType t = ((PTypeSequence)tp).ElementType;
            long ll = this.Count();
            if (ll == 0) yield break;
            PaEntry2 first = this.Element(0);
            this.cell.SetOffset(first.offset);
            for (long ii = 0; ii < ll; ii++)
            {
                yield return GetPObject(t, this.cell.br);
            }
        }
        public IEnumerable<object> ElementValues(long start, long number)
        {
            if (tp.Vid != PTypeEnumeration.sequence) throw new Exception("Err in TPath formula: ElementValues() can't be applyed to structure of vid " + tp.Vid);
            PType t = ((PTypeSequence)tp).ElementType;
            //if (!t.HasNoTail) throw new Exception("Method ElementValues() can't be applied to tail types");
            long ll = this.Count();
            // Надо проверить соответствие диапазона количеству элементов
            if (start < 0 || start + number > ll) throw new Exception("Err: Diapason is out of range");
            if (number == 0) yield break;
            PaEntry2 first = this.Element(start);
            this.cell.SetOffset(first.offset);
            for (long ii = 0; ii < number; ii++)
            {
                yield return GetPObject(t, this.cell.br);
            }
        }
        // depricated: недодуманная идея, использовать не рекомендуется
        public void Scan(Func<object, bool> handler)
        {
            if (tp.Vid != PTypeEnumeration.sequence) throw new Exception("Err in TPath formula: ElementValues() can't be applyed to structure of vid " + tp.Vid);
            PTypeSequence mts = (PTypeSequence)tp;
            PType t = mts.ElementType;
            long ll = this.Count();
            if (ll == 0) return;
            PaEntry2 first = this.Element(0);
            this.cell.SetOffset(first.offset);
            for (long ii = 0; ii < ll; ii++)
            {
                object pvalue = GetPObject(t, this.cell.br);
                bool ok = handler(pvalue);
                if (!ok) throw new Exception("Scan handler catched 'false' at element " + ii);
            }
        }
        // Возможно, уже лучше: передавать нужно и offset
        public void Scan(Func<long, object, bool> handler)
        {
            if (tp.Vid != PTypeEnumeration.sequence) throw new Exception("Err in TPath formula: ElementValues() can't be applyed to structure of vid " + tp.Vid);
            PTypeSequence mts = (PTypeSequence)tp;
            PType t = mts.ElementType;
            long ll = this.Count();
            if (ll == 0) return;
            PaEntry2 first = this.Element(0);
            this.cell.SetOffset(first.offset);
            for (long ii = 0; ii < ll; ii++)
            {
                long off = this.cell.GetOffset(); //this.offset;
                object pvalue = GetPObject(t, this.cell.br);
                bool ok = handler(off, pvalue);
                if (!ok) throw new Exception("Scan handler catched 'false' at element " + ii);
            }
        }

        // Техническая процедура Пропускает поле, выдает адрес, следующий за ним. Указатель никуда не установлен 
        private long Skip(PType tp, long off)
        {
            if (tp.HasNoTail) return off + tp.HeadSize;
            if (tp.Vid == PTypeEnumeration.sstring)
            {
                //cell.SetOffset(off);
                //int len = cell.br.ReadInt32();
                //char[] chrs = cell.br.ReadChars(len);
                //return cell.fs.Position;
                long offout;
                cell.ReadString(off, out offout);
                return offout;
            }
            if (tp.Vid == PTypeEnumeration.record)
            {
                long field_offset = off;
                PTypeRecord mtr = (PTypeRecord)tp;
                foreach (var pa in mtr.Fields)
                {
                    field_offset = Skip(pa.Type, field_offset);
                }
                return field_offset;
            }
            if (tp.Vid == PTypeEnumeration.sequence)
            {
                PTypeSequence mts = (PTypeSequence)tp;
                PType tel = mts.ElementType;
                //cell.SetOffset(off);
                //long llen = cell.br.ReadInt64();
                long llen = cell.ReadLong(off);
                if (tel.HasNoTail) return off + 8 + llen * tel.HeadSize;
                long element_offset = off + 8;
                for (long ii = 0; ii < llen; ii++) element_offset = Skip(tel, element_offset);
                return element_offset;
            }
            if (tp.Vid == PTypeEnumeration.union)
            {
                PTypeUnion mtu = (PTypeUnion)tp;
                //cell.SetOffset(off);
                //int v = cell.br.ReadByte();
                int v = cell.ReadByte(off);
                if (v < 0 || v >= mtu.Variants.Length) throw new Exception("Err in Skip (TPath-formula): wrong variant for union " + v);
                PType mt = mtu.Variants[v].Type;
                return Skip(mt, off + 1);
            }
            throw new Exception("Assert err: 2874");
        }
    }
}
