using RDFCommon;

namespace RDFTripleStore.OVns
{
    public class OV_int : ObjectVariants, ILiteralNode
    {
        public readonly int value;

        public OV_int(int value)
        {
            this.value = value;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.Int; }
        }

        public override object WritableValue
        {
            get { return value; }
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

            return value == ((OV_int)obj).value;

        }

        public override int GetHashCode()
        {
            var hashCode = value.GetHashCode();
            return unchecked(( 79 ^ hashCode) * ( 127 ^ Variant.GetHashCode()));
            //int c = Variant.GetHashCode() << 27 | (hashCode & ((1 << 27) - 1));
            //return c;
        }


        public override dynamic Content { get { return value; } }
        public string DataType { get { return SpecialTypesClass.Integer.FullName; } }
        public override string ToString()
        {
            return value.ToString();
        }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_int)obj;
            return value.CompareTo(otherTyped.value);
        }
    }
}