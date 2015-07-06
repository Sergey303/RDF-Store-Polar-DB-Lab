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
        // var found = entry.Where(en => predicate).DefaultIfEmpty(PaEntry.Empty).First(); if (found.IsEmpty()) ...
        private static PaEntry2 _empty = new PaEntry2() { cell = null, offset = Int64.MinValue, tp = null };
        public static PaEntry2 Empty { get { return _empty; } }
        public bool IsEmpty { get { return offset == Int64.MinValue; } }
        
        public PaEntry2(PType tp, long offset, PaCell2 cell)
        {
            this.tp = tp;
            this.offset = offset;
            this.cell = cell;
        }
        // Пропускает поле, выдает адрес, следующий за ним. Указатель никуда не установлен 
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
