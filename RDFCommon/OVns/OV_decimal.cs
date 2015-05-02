namespace RDFCommon.OVns
{
    public class OV_decimal : ObjectVariants, ILiteralNode
    {
        public readonly decimal value;

        public OV_decimal(decimal value)
        {
            this.value = value;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.Decimal; }
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

            return value == ((OV_decimal)obj).value;

        }

        public override int GetHashCode()
        {
            var hashCode = value.GetHashCode();
            return unchecked((31 ^ hashCode )* (37 ^ Variant.GetHashCode()));

        }

        public override dynamic Content { get { return value; } }
        public string DataType { get { return SpecialTypesClass.Decimal.FullName; } }
        public override string ToString()
        {
            return value.ToString();
        }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_decimal)obj;
            return value.CompareTo(otherTyped.value);
        }
    }
}