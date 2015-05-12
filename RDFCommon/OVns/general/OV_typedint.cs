using System;

namespace RDFCommon.OVns
{
    public class OV_typedint : ObjectVariants, ILiteralNode
    {
        internal readonly string value; public readonly int curi;
        private readonly Func<int, string> nameTable;

        public OV_typedint(string value, int curi, Func<int, string> nameTable)
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

        public override object Content { get { return value; } }
        public override ObjectVariants Change(Func<dynamic, dynamic> changing)
        {
            return new OV_typedint(changing(value), curi, nameTable);
        }

        public string DataType { get { return nameTable(curi); } }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_typedint)obj;
            var cmpBase = String.Compare(DataType, otherTyped.DataType, StringComparison.InvariantCulture);
            //if (obj is OV_langstring) //если совпали варианты, то и типы идентичны.
            if (cmpBase != 0)  return cmpBase;
            return System.String.Compare(value, otherTyped.value, System.StringComparison.InvariantCulture);
            
        }
    }
}