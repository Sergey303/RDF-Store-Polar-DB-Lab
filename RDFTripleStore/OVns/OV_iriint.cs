using System;
using Task15UniversalIndex;

namespace RDFTripleStore.OVns
{
    public class OV_iriint : ObjectVariants
    {
        public readonly int code;
        private readonly NameTableUniversal nameTable;

        public OV_iriint(int code, NameTableUniversal nameTable)
        {
            this.code = code;
            this.nameTable = nameTable;
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
            return unchecked((Variant.GetHashCode() << 4) + code );
        }
        public override string  ToString(){ return nameTable.GetStringByCode(code); }
        public override Comparer ToComparable()
        {
            return new Comparer2(Variant, code);
        }
    }
}