using System;
using RDFCommon;

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

        public override IComparable ToComparable()
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
            return value.GetHashCode()+37*turi.GetHashCode();
        }

        public dynamic Content { get { return value; } }
        public string DataType { get { return turi; } }
        public override string ToString()
        {
            return "\"" + value + "\"^^<"+DataType+">";
        }

    }
}