using System;

namespace RDFCommon.OVns
{
    public class OV_date : ObjectVariants, ILiteralNode
    {
        public readonly DateTime value;

        public OV_date(DateTime value)
        {
            this.value = value;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.Date; }
        }

        public override object WritableValue
        {
            get { return value.ToBinary(); }
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

            return value == ((OV_date)obj).value;

        }

        public override int GetHashCode()
        {
            var hashCode = value.GetHashCode();
            return unchecked((59 ^ hashCode) *(61 ^ Variant.GetHashCode()));
        }


        public override dynamic Content { get { return value; } }
        public override ObjectVariants Change(Func<dynamic, dynamic> changing)
        {
            return new OV_date(changing(value));
        }

        public string DataType { get { return SpecialTypesClass.Date.FullName; } }
        public override string ToString()
        {
            return value.ToString();
        }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_date)obj;
            return value.CompareTo(otherTyped.value);
        }
    }
}