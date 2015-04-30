using System;
using RDFCommon;
using RDFTripleStore.Comparer;

namespace RDFTripleStore.OVns
{
    public class OV_typed : ObjectVariants, ILiteralNode
    {
        public readonly string value; public readonly string turi;

        public OV_typed(string value, string turi)
        {
            this.value = value;
            this.turi = turi;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.Other; }
        }

        public override object WritableValue
        {
            get
            {
                return new object[] { value, turi };
            }
        }

        public Comparer.Comparer ToComparable()
        {
            return new Comparer3(Variant,turi, value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = ((OV_typed)obj);
            return value == other.value && turi.Equals(other.turi);
        }

        public override int GetHashCode()
        {
            return unchecked((1277 ^ value.GetHashCode()) * (31 ^ turi.GetHashCode()) *(127*Variant.GetHashCode()));
        }

        public override dynamic Content { get { return value; } }
        public string DataType { get { return turi; } }
        public override string ToString()
        {
            return "\"" + value + "\"^^<"+DataType+">";
        }
        public override int CompareTo(object obj)
        {
            if (obj is ObjectVariants)
            {
                var cmpBase = base.CompareTo(obj);
                //if (obj is OV_langstring) //если совпали варианты, то и типы идентичны.
                if (cmpBase != 0)
                    return cmpBase;
                var otherTyped = (OV_typed)obj;
                return DataType.CompareTo(otherTyped.DataType);
            }
            throw new ArgumentException();
        }
    }
}