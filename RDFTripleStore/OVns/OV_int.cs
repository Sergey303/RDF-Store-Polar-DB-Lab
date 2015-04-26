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

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public dynamic Content { get { return value; } }
        public string DataType { get { return SpecialTypesClass.Integer.FullName; } }
        public override string ToString()
        {
            return value.ToString();
        }
    }
}