using System;

namespace RDFCommon.OVns
{
    public class OV_normalizedString : ObjectVariants, ILiteralNode
    {
        public readonly string value;

        public OV_normalizedString(string value)
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

            return value == ((OV_normalizedString)obj).value;

        }

        public override int GetHashCode()
        {
            return unchecked((27644437 ^ value.GetHashCode()) * (127 ^ Variant.GetHashCode()));
        }


        public override dynamic Content { get { return value; } }
        public override ObjectVariants Change(Func<dynamic, dynamic> changing)
        {
            return new OV_normalizedString(changing(value));
        }

        public string DataType { get { return SpecialTypesClass.Bool; } }
        public override string ToString()
        {
            return value.ToString();
        }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_normalizedString)obj;           
            return value.CompareTo(otherTyped.value);
        }
    }
}