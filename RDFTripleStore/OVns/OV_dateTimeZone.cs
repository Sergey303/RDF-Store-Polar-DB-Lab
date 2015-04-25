using System;
using RDFCommon;

namespace RDFTripleStore.OVns
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

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public dynamic Content { get { return value; } }
        public string DataType { get { return SpecialTypesClass.DayTimeDuration.FullName; } }

    }
}