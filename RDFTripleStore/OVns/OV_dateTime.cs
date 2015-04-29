using System;

namespace RDFTripleStore.OVns
{
    public class OV_dateTime : ObjectVariants
    {
        public readonly DateTime value;

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

            return value == ((OV_dateTime)obj).value;

        }

        public override int GetHashCode()
        {
            var hashCode = value.GetHashCode();
            return unchecked((41^ hashCode)  * (127^Variant.GetHashCode()));
        }


        public override string ToString()
        {
            return value.ToString();
        }

    }
}