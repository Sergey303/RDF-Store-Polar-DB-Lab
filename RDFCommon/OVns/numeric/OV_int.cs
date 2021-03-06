using System;

namespace RDFCommon.OVns
{
    public class OV_int : ObjectVariants, ILiteralNode, INumLiteral
    {
        public readonly int value;

        public OV_int(int value)
        {
            this.value = value;
        }

        public OV_int(string s)
            : this(int.Parse(s))
        {
            

        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.Int; }
        }

        public override object WritableValue
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

            return value == ((OV_int)obj).value;

        }

        public override int GetHashCode()
        {
            //return value+ Variant.GetHashCode();
           // return (value + Variant.ToString()).ToCharArray().GetHash();
            
            return Tuple.Create(value, Variant).GetHashCode();
            // var hashCode = value.GetHashCode();
            //   return unchecked(( 79 ^ hashCode) * ( 127 ^ Variant.GetHashCode()));
            //int c = Variant.GetHashCode() << 27 | (hashCode & ((1 << 27) - 1));
            //return c;
        }


        public override object Content { get { return value; } }
        public override ObjectVariants Change(Func<dynamic, dynamic> changing)
        {
            return new OV_int(changing(value));
        }

        public string DataType { get { return SpecialTypesClass.Integer; } }
        public override string ToString()
        {
            return value.ToString();
        }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_int)obj;
            return value.CompareTo(otherTyped.value);
        }
    }
}