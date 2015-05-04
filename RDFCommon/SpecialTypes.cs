using RDFCommon.OVns;
using RDFTripleStore;

namespace RDFCommon
{
    public class SpecialTypesClass
    {

        public static UriPrefixed Integer = new UriPrefixed(XmlSchema.Default.xmlSchemaPrefix, XmlSchema.Default.Integer, XmlSchema.Default.xmlSchemaNs);
        public static UriPrefixed Float = new UriPrefixed(XmlSchema.Default.xmlSchemaPrefix, XmlSchema.Default.Float, XmlSchema.Default.xmlSchemaNs);
        public static UriPrefixed Double = new UriPrefixed(XmlSchema.Default.xmlSchemaPrefix, XmlSchema.Default.Double, XmlSchema.Default.xmlSchemaNs);
        public static UriPrefixed Decimal = new UriPrefixed(XmlSchema.Default.xmlSchemaPrefix, XmlSchema.Default.Decimal, XmlSchema.Default.xmlSchemaNs);
        public static UriPrefixed Bool = new UriPrefixed(XmlSchema.Default.xmlSchemaPrefix, XmlSchema.Default.Bool, XmlSchema.Default.xmlSchemaNs);
        public static UriPrefixed Date = new UriPrefixed(XmlSchema.Default.xmlSchemaPrefix, XmlSchema.Default.Date, XmlSchema.Default.xmlSchemaNs);
        public static UriPrefixed DateTime = new UriPrefixed(XmlSchema.Default.xmlSchemaPrefix, XmlSchema.Default.DateTime, XmlSchema.Default.xmlSchemaNs);
        public static UriPrefixed LangString = new UriPrefixed(XmlSchema.Default.xmlSchemaPrefix, XmlSchema.Default.LangString, XmlSchema.Default.xmlSchemaNs);
        public static UriPrefixed String = new UriPrefixed(XmlSchema.Default.xmlSchemaPrefix, XmlSchema.Default.String, XmlSchema.Default.xmlSchemaNs);
        public static UriPrefixed DayTimeDuration = new UriPrefixed(XmlSchema.Default.xmlSchemaPrefix, XmlSchema.Default.DayTimeDuration, XmlSchema.Default.xmlSchemaNs);
        //public static UriPrefixed SimpleLiteral = new UriPrefixed("smpl:", "", XmlSchema.Default.simple_literal);

        public static UriPrefixed RdfFirst = new UriPrefixed("rdf:", "first", XmlSchema.Default.rdf_syntax_ns);
        public static UriPrefixed RdfRest = new UriPrefixed("rdf:", "rest", XmlSchema.Default.rdf_syntax_ns);
           public static UriPrefixed RdfType = new UriPrefixed("rdf:", XmlSchema.Default.type, XmlSchema.Default.rdf_syntax_ns);
        public static UriPrefixed Nil = new UriPrefixed("rdf:", XmlSchema.Default.nil, XmlSchema.Default.rdf_syntax_ns);
        public ObjectVariants date;
        public ObjectVariants @string;
        //public ObjectVariants simpleLiteral;
        public ObjectVariants langString;
        public ObjectVariants integer;
        public ObjectVariants @decimal;
        public ObjectVariants @double;
        public ObjectVariants @bool;
        public ObjectVariants @float;
        public ObjectVariants timeDuration;
        public ObjectVariants dateTime;
        public ObjectVariants nil;
        public ObjectVariants first;
        public ObjectVariants rest;
        public ObjectVariants type;

        public static string[] GetAll()
        {
            return new[]
            {
                Integer.FullName, Float.FullName, Double.FullName, Decimal.FullName, Bool.FullName, Date.FullName,
                DateTime.FullName, LangString.FullName, String.FullName, DayTimeDuration.FullName, RdfFirst.FullName,
                RdfRest.FullName,    RdfType.FullName, Nil.FullName
            };
        }
        public SpecialTypesClass(INodeGenerator nodeGenerator)
        {
            date =nodeGenerator. CreateUriNode(Date);
            @string = nodeGenerator.CreateUriNode(String);
            //simpleLiteral = simple_literal_equals_string_literal
            //    ? String
            //    :  nodeGenerator.CreateUriNode(SpecialTypes.SimpleLiteral);
            langString = nodeGenerator.CreateUriNode(String);
            integer = nodeGenerator.CreateUriNode(Integer);
            @decimal = nodeGenerator.CreateUriNode(Decimal);
            @double = nodeGenerator.CreateUriNode(Double);
            @bool = nodeGenerator.CreateUriNode(Bool);
            @float = nodeGenerator.CreateUriNode(Float);
            timeDuration = nodeGenerator.CreateUriNode(DayTimeDuration);
            dateTime = nodeGenerator.CreateUriNode(DateTime);
            nil = nodeGenerator.CreateUriNode(Nil);
            first = nodeGenerator.CreateUriNode(RdfFirst);
            rest = nodeGenerator.CreateUriNode(RdfRest);
            type = nodeGenerator.CreateUriNode(RdfType);        
        }
    }

}