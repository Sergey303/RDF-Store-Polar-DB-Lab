using RDFCommon;

namespace RDFTripleStore.OVns
{
    public class OV_double : ObjectVariants, ILiteralNode
    {
        public readonly double value;

        public OV_double(double value)
        {
            this.value = value;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.Double; }
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

            return value == ((OV_double)obj).value;

        }

        public override int GetHashCode()
        {
            int hashCode=value.GetHashCode();
            return unchecked((67 ^ hashCode) * (127 ^ Variant.GetHashCode()));
        }

        public dynamic Content { get { return value; } }
        public string DataType { get { return SpecialTypesClass.Double.FullName; } }
        public override string ToString()
        {
            return value.ToString();
        }
    }
}