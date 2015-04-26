using System;
using RDFCommon;

namespace RDFTripleStore.OVns
{
    public abstract class ObjectVariants   :IObjectNode
    {
        public  abstract ObjectVariantEnum Variant { get; }
        public abstract object WritableValue { get; }

        public virtual object[] ToWritable()
        {
            return new object[]{(int)Variant, WritableValue};
        }
        
        public static ObjectVariants CreateLiteralNode(string p)
        {
            p = p.Trim('"', '\'');
            return new OV_string(p);//SimpleLiteralNode
        }

        public static ObjectVariants CreateLang(string s, string lang)
        {
            s = s.Trim('"', '\'');
            return new OV_langstring( s, lang );

        }

        public static ObjectVariants CreateLiteralNode(int parse)
        {
            return new OV_int(parse);//int

        }

        public static ObjectVariants CreateLiteralNode(decimal p)
        {
            return new OV_decimal(p);//decimal
        }

        public static ObjectVariants CreateLiteralNode(double p)
        {
            return new OV_double(p);//double
        }

        public static ObjectVariants CreateLiteralNode(bool p)
        {
            return new OV_bool(p);// ? BoolLiteralNode.TrueNode((SpecialTypes.Bool)) : BoolLiteralNode.FalseNode((SpecialTypes.Bool));
        }

        public static ObjectVariants CreateLiteralNode(string p, string typeUriNode)
        {
            p = p.Trim('"', '\'');
            typeUriNode = typeUriNode.ToLower();
            if (typeUriNode == SpecialTypesClass.String.FullName)
                return new OV_string(p);
            else if (typeUriNode == SpecialTypesClass.Date.FullName)
            {
                DateTime date;
                if (!DateTime.TryParse(p, out date)) throw new ArgumentException(p);
                return new OV_date(date);
            }
            else if (typeUriNode == SpecialTypesClass.DateTime.FullName)
            {
                DateTimeOffset date;
                if (!DateTimeOffset.TryParse(p, out date)) throw new ArgumentException(p);
                return new OV_dateTimeZone(date);
            }
            else if (typeUriNode == (SpecialTypesClass.Bool.FullName))
            {
                bool b;
                if (!bool.TryParse(p, out b)) throw new ArgumentException(p);
                return new OV_bool(b);
            }
            else if (typeUriNode == SpecialTypesClass.Decimal.FullName)
            {
                decimal d;
                if (!decimal.TryParse(p.Replace(".", ","), out d)) throw new ArgumentException(p);
                return new OV_decimal( d);
            }
            else if (typeUriNode == SpecialTypesClass.Double.FullName)
            {
                double d;
                if (!double.TryParse(p.Replace(".", ","), out d)) throw new ArgumentException(p);
                return new OV_double(d);
            }
            else if (typeUriNode == SpecialTypesClass.Float.FullName)
            {
                float f;
                if (!float.TryParse(p.Replace(".", ","), out f)) throw new ArgumentException(p);
                return new OV_float(f);
            }
            else if (typeUriNode == SpecialTypesClass.Integer.FullName)
            {
                int i;
                if (!int.TryParse(p, out i)) throw new ArgumentException(p);
                return new OV_int(i);
            }
            else if (typeUriNode == SpecialTypesClass.DayTimeDuration.FullName)
            {
                TimeSpan i;
                if (!TimeSpan.TryParse(p, out i)) throw new ArgumentException(p);
                return new OV_time(i);
            }
            else
                return new OV_typed(p, typeUriNode);
        }

        public static string CreateBlankNode(string graph, string blankNodeString = null)
        {
            if (blankNodeString == null)
                blankNodeString = "blank" + (long)(random.NextDouble() * 1000 * 1000 * 1000 * 1000);

            if (graph != null) blankNodeString = graph + "/" + blankNodeString;

            return blankNodeString;
        }

        private static readonly Random random = new Random();

        public virtual IComparable ToComparable()
        {
            if(this is IIriNode)
                return new Comparer2(Variant, ((IIriNode)this).UriString);
            return new Comparer2(Variant, ((ILiteralNode)this).Content);
        }


        
    }
}