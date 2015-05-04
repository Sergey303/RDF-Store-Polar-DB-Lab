using System;

namespace RDFCommon.OVns
{
    public class OV_dateTimeZone : ObjectVariants, ILiteralNode
    {
        public readonly DateTimeOffset value;

        public OV_dateTimeZone(DateTimeOffset value)
        {
            this.value = value;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.DateTimeZone; }
        }

        public override object WritableValue
        {
            get { return value.ToFileTime(); }
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

            return value == ((OV_dateTimeZone)obj).value;

        }

        public override int GetHashCode()
        {
            var hashCode = value.GetHashCode();
            return unchecked((41 ^ hashCode) * (43 ^ Variant.GetHashCode()));
        }


        public override dynamic Content { get { return value; } }
        public override ObjectVariants Change(Func<dynamic, dynamic> changing)
        {
            return new OV_dateTimeZone(changing(value));
        }

        public string DataType { get { return SpecialTypesClass.DayTimeDuration.FullName; } }
        public override string ToString()
        {
            return value.ToString();
        }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_dateTimeZone)obj;
            return value.CompareTo(otherTyped.value);
        }
    }
}