using System;

namespace RDFCommon.OVns
{
    public class OV_iriint : ObjectVariants, IIriNode
    {
        public readonly int code;
        private readonly Func<int, string> decode;
        

        public OV_iriint(int code, Func<int, string> decode)
        {
            this.code = code;
            this.decode = decode;
        }

        
        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.IriInt; }
        }

        public override object WritableValue
        {
            get { return code; }
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

            return code == ((OV_iriint)obj).code;

        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return unchecked((11 ^ code) * (13 ^ Variant.GetHashCode()));
            //int v = Variant.GetHashCode();
            //int c = v << 27 | (code & ((1 << 27) - 1));
            //return c; 
        }
        public override string  ToString()
        {
            return UriString;
        }
    

        public override object Content
        {
            get { return code; }
        }

        public override ObjectVariants Change(Func<dynamic, dynamic> changing)
        {
            dynamic newIri = changing(UriString);
            //return new OV_iriint(getCode(newIri), getCode);
            return new OV_iri(newIri);
        }

        public string UriString { get { return decode(code); } }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_iriint)obj;
            return code.CompareTo(otherTyped.code);
        }
    }
}