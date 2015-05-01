using System;
using RDFCommon;
using RDFTripleStore.Comparer;
using Task15UniversalIndex;

namespace RDFTripleStore.OVns
{
    public class OV_iriint : ObjectVariants, IIriNode
    {
        public readonly int code;
        private readonly NameTableUniversal nameTable;
        private string originalString;

        public OV_iriint(int code, NameTableUniversal nameTable)
        {
            this.code = code;
            this.nameTable = nameTable;
        }

        public OV_iriint(int code, string uri)
        {
            this.code = code;
            originalString = uri;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.IriInt; }
        }

        public override object WritableValue
        {
            get { return code; }
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return code == ((OV_iriint)obj).code;

        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return unchecked((11 ^ code) * (13 ^ Variant.GetHashCode()));
            //int v = Variant.GetHashCode();
            //int c = v << 27 | (code & ((1 << 27) - 1));
            //return c; 
        }
        public override string  ToString()
        {
            return UriString;
        }
        public Comparer.Comparer ToComparable()
        {
            return new Comparer2(Variant, code);
        }

        public override dynamic Content
        {
            get { return code; }
        }

        public string UriString { get { return code==-1 ? originalString : nameTable.GetStringByCode(code); } }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_iriint)obj;
            return code.CompareTo(otherTyped.code);
        }
    }
}