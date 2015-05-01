using System;

namespace RDFCommon.OVns
{
    public class OV_time : ObjectVariants
    {
        public readonly TimeSpan value;

        public OV_time(TimeSpan value)
        {
            this.value = value;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.Time; }
        }

        public override object WritableValue
        {
            get { return value.Ticks; }
        }

        public override dynamic Content
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

            return value == ((OV_time)obj).value;

        }

        public override int GetHashCode()
        {
              var hashCode =value.GetHashCode();
              return unchecked((71 ^ hashCode) * (73 ^ Variant.GetHashCode()));

        }


        public override string ToString()
        {
            return value.ToString();
        }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_time)obj;
            return value.CompareTo(otherTyped.value);
        }
    }
}