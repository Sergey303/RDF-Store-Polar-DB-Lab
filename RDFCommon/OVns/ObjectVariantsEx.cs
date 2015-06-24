using System;

namespace RDFCommon.OVns
{
    public   static class ObjectVariantsEx
    {
       public delegate ObjectVariants Create(object writable, Func<int, string> decode = null);

        public static readonly Create[] w2ov =
        {
            (s, nt) => new OV_iri((string) s),
            (s, decode) => new OV_iriint((int) s, decode),
            (s, nt) => new OV_bool((bool) s),
            (s, nt) => new OV_string((string) s),
            (strLang, nt) => new OV_langstring((string) ((object[]) strLang)[0], (string) ((object[]) strLang)[1]),
            (s, nt) => new OV_double((double) s),
            (s, nt) => new OV_decimal(Convert.ToDecimal(s)),
            (s, nt) => new OV_float((float) s),
            (s, nt) => new OV_int((int) s),
            (s, nt) => new OV_dateTimeStamp(DateTimeOffset.FromFileTime((long) s)), 
            (s, nt) => new OV_dateTime(DateTime.FromBinary((long) s)),
            (s, nt) => new OV_date(DateTime.FromBinary((long) s)),
            (s, nt) => new OV_time(DateTimeOffset.FromFileTime((long)s)),
            (typed, nt) => new OV_typed((string) ((object[]) typed)[0], (string) ((object[]) typed)[1]),
            (typed, decode) => new OV_typedint((string) ((object[]) typed)[0], (int) ((object[]) typed)[1], decode),
        };


        public static ObjectVariants Writeble2OVariant(this object[] @object, Func<int, string> nt = null)
        {
            return w2ov[(int)@object[0]](@object[1], nt);
        }
        public static ObjectVariants ToOVariant(this object @object, Func<int, string> nt = null)
        {
            return Writeble2OVariant((object[]) @object,nt);
        }

        //public IComparable ToComparable(this object @object)
        //{
        //    var o = (object[])@object;
        //    if(o[0]==4 ||)
        //}
    }
}