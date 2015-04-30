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
            return unchecked((89 ^ value.GetHashCode()) * (97 ^ curi.GetHashCode()) * (227 * Variant.GetHashCode()));
        }

        public override string ToString()
        {
            return "\"" + value + "\"^^<" + DataType + ">";
        }

        public override dynamic Content { get { return value; } }
        public string DataType { get { return nameTable.GetStringByCode(curi); } }
        public override int CompareTo(object obj)
        {
            if (obj is ObjectVariants)
            {
                var cmpBase = base.CompareTo(obj);
                //if (obj is OV_langstring) //если совпали варианты, то и типы идентичны.
                if (cmpBase != 0)
                    return cmpBase;
                var otherTyped = (OV_typedint)obj;
                return DataType.CompareTo(otherTyped.DataType);
            }
            throw new ArgumentException();
        }
    }
}