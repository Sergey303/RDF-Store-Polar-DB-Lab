using RDFCommon;

namespace RDFTripleStore.OVns
{
    public class OV_float : ObjectVariants, ILiteralNode
    {
        public readonly float value;

        public OV_float(float value)
        {
            this.value = value;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.Float; }
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

            return value == ((OV_float)obj).value;

        }

        public override int GetHashCode()
        {
            var hashCode = value.GetHashCode();
            return unchecked((17 ^ hashCode) * (19 ^ Variant.GetHashCode()));
        }


        public override dynamic Content { get { return value; } }
        public string DataType { get { return SpecialTypesClass.Float.FullName; } }
        public override string ToString()
        {
            return value.ToString();
        }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_float)obj;
            return value.CompareTo(otherTyped.value);
        }
    }
}