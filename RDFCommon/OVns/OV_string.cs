namespace RDFCommon.OVns
{
    public class OV_string : ObjectVariants, ILiteralNode, IStringLiteralNode
    {
        public readonly string value;

        public OV_string(string value)
        {
            this.value = value;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.Str; }
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

            return value == ((OV_string)obj).value;

        }

        public override int GetHashCode()
        {
            var hashCode = value.GetHashCode();
            return unchecked((101 ^ hashCode) * (727 ^ Variant.GetHashCode()));
            //return Variant.GetHashCode() << 17 | (hashCode & (1 << 17 - 1));
        }


        public override dynamic Content { get { return value; } }
        public string DataType { get { return SpecialTypesClass.Integer.FullName; } }
        public override string ToString()
        {
            return value;
        }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_string)obj;
            return System.String.Compare(value, otherTyped.value, System.StringComparison.InvariantCulture);
        }
    }
}