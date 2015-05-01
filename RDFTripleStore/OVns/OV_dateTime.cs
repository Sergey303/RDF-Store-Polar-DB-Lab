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