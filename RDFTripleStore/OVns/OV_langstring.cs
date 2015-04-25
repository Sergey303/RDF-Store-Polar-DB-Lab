using System;
using RDFCommon;

namespace RDFTripleStore.OVns
{
    public class OV_langstring : ObjectVariants, ILanguageLiteral 
    {
        public readonly string value; public readonly string lang;

        public OV_langstring(string value, string lang)
        {
            this.value = value;
            this.lang = lang;
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.Lang; }
        }

        public override object WritableValue
        {
            get { return new object[] { value, lang }; }
        }
        public override IComparable ToComparable()
        {
            return new Comparer3(Variant, value, lang);
        }

        // override object.Equals
        public bool Equals(OV_langstring other)
        {
            throw new NotImplementedException();
        }

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
            var other = ((OV_langstring)obj);
            return lang == other.lang && value == other.value;  
        }

        public override int GetHashCode()
        {
            return value.GetHashCode() + 373*lang.GetHashCode();
        }


        public string Lang { get { return lang; } }
        public dynamic Content { get { return value; } }
        public string DataType { get { return SpecialTypesClass.LangString.FullName; } }
    }

}