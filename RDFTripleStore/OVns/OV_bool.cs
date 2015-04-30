using RDFCommon;

namespace RDFTripleStore.OVns
{
    public class OV_bool : ObjectVariants, ILiteralNode
    {
        public readonly bool value;

        public OV_bool(bool value)
        {
            this.value = value;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.Bool; }
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

            return value == ((OV_bool)obj).value;

        }

        public override int GetHashCode()
        {
            return unchecked((27644437 ^ value.GetHashCode()) * (127 ^ Variant.GetHashCode()));
        }


        public override dynamic Content { get { return value; } }
        public string DataType { get { return SpecialTypesClass.Bool.FullName; } }
        public override string ToString()
        {
            return value.ToString();
        }
    }
}