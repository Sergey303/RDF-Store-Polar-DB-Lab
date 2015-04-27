using System;
using RDFCommon;
using RDFTripleStore.Comparer;

namespace RDFTripleStore.OVns
{
    public class OV_iri : ObjectVariants, IIriNode, IBlankNode
    {
        private readonly string uriString;

        public OV_iri(string fullId)
        {
            uriString = fullId.ToLower();
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
            return unchecked((Variant.GetHashCode() << 4) + uriString.GetHashCode());
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

        public override Comparer.Comparer ToComparable()
        {
            return new Comparer2(Variant, uriString);
        }
    }
}