using System;

namespace RDFCommon.OVns
{
    public class OV_iri : ObjectVariants, IIriNode, IBlankNode
    {
        private readonly string uriString;

        public OV_iri(string fullId)
        {
            uriString = fullId.ToLowerInvariant();
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.Iri; }
        }

        public override object WritableValue
        {
            get { return UriString; }
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

            return uriString == ((OV_iri)obj).uriString;

        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            var hashCode = uriString.GetHashCode();
            return unchecked((11 ^ hashCode) * (13 ^ Variant.GetHashCode()));
        }

        public string UriString
        {
            get { return uriString; }
        }

        public string Name { get { return uriString; } }
        public override string ToString()
        {
            return uriString;
        }

      
        public override dynamic Content
        {
            get { return uriString; }
        }

        public override ObjectVariants Change(Func<dynamic, dynamic> changing)
        {
            return new OV_iri(changing(uriString));
        }

        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_iri)obj;
            return System.String.Compare(uriString, otherTyped.uriString, System.StringComparison.InvariantCulture);
        }

      
    }
}