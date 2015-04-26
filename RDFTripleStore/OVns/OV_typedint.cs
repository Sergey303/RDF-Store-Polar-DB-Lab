using System;
using RDFCommon;
using Task15UniversalIndex;

namespace RDFTripleStore.OVns
{
    public class OV_typedint : ObjectVariants, ILiteralNode
    {
        private readonly string value; public readonly int curi;
        private readonly NameTableUniversal nameTable;

        public OV_typedint(string value, int curi, NameTableUniversal nameTable)
        {
            this.value = value;
            this.curi = curi;
            this.nameTable = nameTable;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.OtherIntType; }
        }

        public override object WritableValue
        {
            get { return new object[] { value, curi }; }
        }
        public override IComparable ToComparable()
        {
            return new Comparer3(Variant, curi, value);
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = ((OV_typedint)obj);
            return value == other.value && curi.Equals(other.curi);
        }

      
        public override int GetHashCode()
        {
            return value.GetHashCode() + 37 * curi.GetHashCode();
        }

        public override string ToString()
        {
            return "\"" + value + "\"^^<" + DataType + ">";
        }

        public dynamic Content { get { return value; }}
        public string DataType { get { return nameTable.GetStringByCode(curi); } }
    }
}