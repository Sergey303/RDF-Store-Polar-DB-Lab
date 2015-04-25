using System;

namespace RDFTripleStore.ObjectVariants
{
    public class OV_typedint : ObjectVariants
    {
        private readonly string value; public readonly int curi;

        public OV_typedint(string value, int curi)
        {
            this.value = value;
            this.curi = curi;
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
    }
}