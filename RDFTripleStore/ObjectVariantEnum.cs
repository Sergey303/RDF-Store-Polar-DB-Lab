using System;
using PolarDB;
using RDFTripleStore.Comparer;
using RDFTripleStore.OVns;
using Task15UniversalIndex;

namespace RDFTripleStore
{
  
    public enum ObjectVariantEnum
    {
        Iri, IriInt, Bool, Str, Lang, Double, Decimal, Float,Int, DateTimeZone,DateTime, Date, Time, Other, OtherIntType
    }

    public static class ObjectVariantsPolarType
    {
        public static readonly PTypeUnion ObjectVariantPolarType = new PTypeUnion(
            new NamedType("iri", new PType(PTypeEnumeration.sstring)),
            new NamedType("iri coded", new PType(PTypeEnumeration.integer)),
            new NamedType("bool", new PType(PTypeEnumeration.boolean)),
            new NamedType("str", new PType(PTypeEnumeration.sstring)),
            new NamedType("lang str",
                new PTypeRecord(new NamedType("str", new PType(PTypeEnumeration.sstring)),
                    new NamedType("lang", new PType(PTypeEnumeration.sstring)))),
            new NamedType("double", new PType(PTypeEnumeration.real)),
            new NamedType("decimal", new PType(PTypeEnumeration.real)),
            new NamedType("float", new PType(PTypeEnumeration.real)),
            new NamedType("int", new PType(PTypeEnumeration.integer)),
            new NamedType("date time zone", new PType(PTypeEnumeration.longinteger)),
            new NamedType("date time", new PType(PTypeEnumeration.longinteger)),
            new NamedType("date", new PType(PTypeEnumeration.longinteger)),
            new NamedType("time", new PType(PTypeEnumeration.longinteger)),
            new NamedType("other",
                new PTypeRecord(new NamedType("str", new PType(PTypeEnumeration.sstring)),
                    new NamedType("type", new PType(PTypeEnumeration.sstring)))),
            new NamedType("other coded type",
                new PTypeRecord(new NamedType("str", new PType(PTypeEnumeration.sstring)),
                    new NamedType("type", new PType(PTypeEnumeration.integer)))));
    }
 public   static class ObjectVariantsEx
    {
        public static readonly Func<object, IComparable>[] w2c=
            {
                  s=>new Comparer2(0, (IComparable) s),
                  s=>new Comparer2(1, (IComparable) s),   
                  s=>new Comparer2(2, (IComparable) s),   
                  s=>new Comparer2(3, (IComparable) s),  
                  strLang=> new Comparer3(5, (IComparable) ((object[])strLang)[1],(IComparable) ((object[])strLang)[0]),
                  s=>new Comparer2(6, (IComparable) s),   
                  s=>new Comparer2(7, (IComparable) s),   
                  s=>new Comparer2(8, (IComparable) s),   
                  s=>new Comparer2(9, (IComparable) s),   
                  s=>new Comparer2(10, (IComparable) s),   
                  s=>new Comparer2(11, (IComparable) s),   
                  s=>new Comparer2(12, (IComparable) s),   
                  s=>new Comparer2(13, (IComparable) s),   
                  typed=>new Comparer3(13,(IComparable) ((object[])typed)[1],(IComparable) ((object[])typed)[0]),   
                  typed=>new Comparer3(14, (IComparable) ((object[])typed)[1],(IComparable) ((object[])typed)[0]),   
            };


        public static IComparable Writeble2Comparable(object[] @object)
        {
            return w2c[(int)@object[0]](@object[1]);
        }

     public delegate ObjectVariants Create(object writable, NameTableUniversal nt = null);

     public static readonly Create[] w2ov =
     {
         (s, nt) => new OV_iri((string) s),
         (s, nt) => new OV_iriint((int) s, nt),
         (s, nt) => new OV_bool((bool) s),
         (s, nt) => new OV_string((string) s),
         (strLang, nt) => new OV_langstring((string) ((object[]) strLang)[0], (string) ((object[]) strLang)[1]),
         (s, nt) => new OV_double((double) s),
         (s, nt) => new OV_decimal((decimal) s),
         (s, nt) => new OV_float((float) s),
         (s, nt) => new OV_int((int) s),
         (s, nt) => new OV_dateTimeZone(DateTimeOffset.FromFileTime((long) s)),
         (s, nt) => new OV_dateTime(DateTime.FromBinary((long) s)),
         (s, nt) => new OV_date(DateTime.FromBinary((long) s)),
         (s, nt) => new OV_time(TimeSpan.FromTicks((long) s)),
         (typed, nt) => new OV_typed((string) ((object[]) typed)[0], (string) ((object[]) typed)[1]),
         (typed, nt) => new OV_typedint((string) ((object[]) typed)[0], (int) ((object[]) typed)[1], nt),
     };


     public static ObjectVariants Writeble2OVariant(this object[] @object, NameTableUniversal nt = null)
        {
            return w2ov[(int)@object[0]](@object[1], nt);
        }
     public static ObjectVariants ToOVariant(this object @object, NameTableUniversal nt = null)
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