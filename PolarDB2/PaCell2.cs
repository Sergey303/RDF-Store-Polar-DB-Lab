using System;
using System.Collections.Generic;
using System.Linq;
using PolarDB;


namespace PolarDB2
{
    public class PaCell2 : PCell
    {
        // Статус
        bool activated = false;

        public PaCell2(PType typ, string filePath, bool readOnly = true)
            : base(typ, false, filePath, readOnly) 
        {
        }
        public PaEntry2 Root 
        { 
            get 
            {
                if (this.IsEmpty) throw new Exception("Root of empty PaCell is undefined");
                return new PaEntry2(this.Type, this.dataStart, this);
            }
        }

        public void Fill(object valu)
        {
            if (!this.IsEmpty) throw new Exception("PaCell2 is not empty");
            this.Restart();
            this.Append(this.Type, valu);
            this.freespace = this.fs.Position; // Это нужно для операции AppendElement
            this.Flush();
        }

        // ============ Чтения данных ============
        public string ReadString(long off, out long offout)
        {
            this.SetOffset(off);
            int len = this.br.ReadInt32();
            char[] chrs = this.br.ReadChars(len);
            offout = this.GetOffset();
            return new string(chrs);
        }
        public long ReadLong(long off)
        {
            this.SetOffset(off);
            long l = this.br.ReadInt64();
            return l;
        }
        public int ReadByte(long off)
        {
            this.SetOffset(off);
            int v = this.br.ReadByte();
            return v;
        }
        /// <summary>
        /// Читает P-объект из бинарного ридера, начиная с текущего места
        /// </summary>
        /// <param name="typ"></param>
        /// <param name="br"></param>
        /// <returns></returns>
        private static object GetPObject(PType typ, System.IO.BinaryReader br)
        {
            switch (typ.Vid)
            {
                case PTypeEnumeration.none: return null;
                case PTypeEnumeration.boolean: return br.ReadBoolean();
                case PTypeEnumeration.integer: return br.ReadInt32();
                case PTypeEnumeration.longinteger: return br.ReadInt64();
                case PTypeEnumeration.real: return br.ReadDouble();
                case PTypeEnumeration.@byte: return br.ReadByte();
                case PTypeEnumeration.fstring:
                    {
                        //int len = ((PTypeFString)typ).Length;
                        int size = ((PTypeFString)typ).Size;
                        byte[] arr = new byte[size];
                        arr = br.ReadBytes(size);
                        string s = System.Text.Encoding.Unicode.GetString(arr);
                        return s;
                    }
                case PTypeEnumeration.sstring:
                    {
                        int len = br.ReadInt32();
                        char[] chrs = br.ReadChars(len);
                        return new string(chrs);
                    }
                case PTypeEnumeration.record:
                    {
                        PTypeRecord r_tp = (PTypeRecord)typ;
                        object[] fields = new object[r_tp.Fields.Length];
                        for (int i = 0; i < r_tp.Fields.Length; i++)
                        {
                            fields[i] = GetPObject(r_tp.Fields[i].Type, br);
                        }
                        return fields;
                    }
                case PTypeEnumeration.sequence:
                    {
                        PTypeSequence mts = (PTypeSequence)typ;
                        PType tel = mts.ElementType;
                        long llen = br.ReadInt64();
                        object[] els = new object[llen];
                        for (long ii = 0; ii < llen; ii++) els[ii] = GetPObject(tel, br);
                        return els;
                    }
                case PTypeEnumeration.union:
                    {
                        PTypeUnion mtu = (PTypeUnion)typ;
                        int v = br.ReadByte();
                        PType mt = mtu.Variants[v].Type;
                        return new object[] { v, GetPObject(mt, br) };
                    }

                default: throw new Exception("Err in TPath Get(): type is not implemented " + typ.Vid);
            }
        }
    }
}
