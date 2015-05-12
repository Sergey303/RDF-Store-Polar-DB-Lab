using System;

namespace RDFCommon.OVns
{
    public class OV_dateTime : ObjectVariants
    {
        public readonly DateTimeOffset value;

        public OV_dateTime(DateTime value)
        {
            this.value = value;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.DateTime; }
        }

        public override object WritableValue
        {
            get { return value.ToFileTime(); }
        }

        public override object Content
        {
            get { return value; }
        }

        public override ObjectVariants Change(Func<dynamic, dynamic> changing)
        {
            return new OV_dateTime(changing(value));
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

            return value == ((OV_dateTime)obj).value;

        }

        public override int GetHashCode()
        {
            var hashCode = value.GetHashCode();
            return unchecked((47^ hashCode)  * (53^Variant.GetHashCode()));
        }


        public override string ToString()
        {
            return value.ToString();
        }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_dateTime)obj;
            return value.CompareTo(otherTyped.value);
        }
    }
}