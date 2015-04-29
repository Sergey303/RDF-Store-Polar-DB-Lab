using System;

namespace RDFTripleStore.OVns
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
              return unchecked((109 ^ hashCode) * (127 ^ Variant.GetHashCode()));

        }


        public override string ToString()
        {
            return value.ToString();
        }
    }
}