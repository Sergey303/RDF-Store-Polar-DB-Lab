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
    }
}
