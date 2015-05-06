using System.Xml.Schema;
using RDFCommon.OVns;


namespace RDFCommon
{
    public class SpecialTypesClass
    {
        public static UriPrefixed String = new UriPrefixed(xmlSchemaPrefix, "string", xmlSchemaNs);
        public static UriPrefixed Bool = new UriPrefixed(xmlSchemaPrefix, "boolean", xmlSchemaNs);
        public static UriPrefixed Decimal = new UriPrefixed(xmlSchemaPrefix, "decimal", xmlSchemaNs);
        public static UriPrefixed Integer = new UriPrefixed(xmlSchemaPrefix, "integr", xmlSchemaNs);

        public static UriPrefixed Float = new UriPrefixed(xmlSchemaPrefix, "float", xmlSchemaNs);
        public static UriPrefixed Double = new UriPrefixed(xmlSchemaPrefix, "double", xmlSchemaNs);

        public static UriPrefixed Date = new UriPrefixed(xmlSchemaPrefix, "date", xmlSchemaNs);
        public static UriPrefixed Time = new UriPrefixed(xmlSchemaPrefix, "time", xmlSchemaNs);
        public static UriPrefixed DateTime = new UriPrefixed(xmlSchemaPrefix, "dateTime", xmlSchemaNs);
        public static UriPrefixed DateTimeStamp = new UriPrefixed(xmlSchemaPrefix, "dateTimeStamp", xmlSchemaNs);

        public static UriPrefixed GYear = new UriPrefixed(xmlSchemaPrefix, "gYear", xmlSchemaNs);
        public static UriPrefixed GMonth = new UriPrefixed(xmlSchemaPrefix, "gMonth", xmlSchemaNs);
        public static UriPrefixed GDay = new UriPrefixed(xmlSchemaPrefix, "gDay", xmlSchemaNs);
        public static UriPrefixed GYearMonth = new UriPrefixed(xmlSchemaPrefix, "gYearMonth", xmlSchemaNs);
        public static UriPrefixed GMonthDay = new UriPrefixed(xmlSchemaPrefix, "gMonthDay", xmlSchemaNs);
        public static UriPrefixed Duration = new UriPrefixed(xmlSchemaPrefix, "duration", xmlSchemaNs);
        public static UriPrefixed YearMonthDuration = new UriPrefixed(xmlSchemaPrefix, "yearMonthDuration", xmlSchemaNs);
        public static UriPrefixed DayTimeDuration = new UriPrefixed(xmlSchemaPrefix, "dayTimeDuration", xmlSchemaNs);


        public static UriPrefixed Byte = new UriPrefixed(xmlSchemaPrefix, "byte", xmlSchemaNs);
        public static UriPrefixed Short = new UriPrefixed(xmlSchemaPrefix, "short", xmlSchemaNs);
        public static UriPrefixed Int = new UriPrefixed(xmlSchemaPrefix, "int", xmlSchemaNs);
        public static UriPrefixed Long = new UriPrefixed(xmlSchemaPrefix, "long", xmlSchemaNs);
        public static UriPrefixed UnsignedByte = new UriPrefixed(xmlSchemaPrefix, "unsignedByte", xmlSchemaNs);
        public static UriPrefixed unsignedShort = new UriPrefixed(xmlSchemaPrefix, "unsignedShort", xmlSchemaNs);
        public static UriPrefixed unsignedInt = new UriPrefixed(xmlSchemaPrefix, "unsignedInt", xmlSchemaNs);
        public static UriPrefixed unsignedLong = new UriPrefixed(xmlSchemaPrefix, "unsignedLong", xmlSchemaNs);
        public static UriPrefixed positiveInteger = new UriPrefixed(xmlSchemaPrefix, "positiveInteger", xmlSchemaNs);
        public static UriPrefixed nonNegativeInteger = new UriPrefixed(xmlSchemaPrefix, "nonNegativeInteger", xmlSchemaNs);
        public static UriPrefixed negativeInteger = new UriPrefixed(xmlSchemaPrefix, "negativeInteger", xmlSchemaNs);
        public static UriPrefixed nonPositiveInteger = new UriPrefixed(xmlSchemaPrefix, "nonPositiveInteger", xmlSchemaNs);


        public static UriPrefixed hexBinary = new UriPrefixed(xmlSchemaPrefix, "hexBinary", xmlSchemaNs);
        public static UriPrefixed base64Binary = new UriPrefixed(xmlSchemaPrefix, "base64Binary", xmlSchemaNs);
        public static UriPrefixed anyURI = new UriPrefixed(xmlSchemaPrefix, "anyURI", xmlSchemaNs);
        public static UriPrefixed language = new UriPrefixed(xmlSchemaPrefix, "language", xmlSchemaNs);
        public static UriPrefixed normalizedString = new UriPrefixed(xmlSchemaPrefix, "normalizedString", xmlSchemaNs);
        public static UriPrefixed token = new UriPrefixed(xmlSchemaPrefix, "token", xmlSchemaNs);
        public static UriPrefixed NMTOKEN = new UriPrefixed(xmlSchemaPrefix, "NMTOKEN", xmlSchemaNs);
        public static UriPrefixed Name = new UriPrefixed(xmlSchemaPrefix, "Name", xmlSchemaNs);
        public static UriPrefixed NCName = new UriPrefixed(xmlSchemaPrefix, "NCName", xmlSchemaNs);

        public static UriPrefixed LangString = new UriPrefixed("rdf:", "langString", rdf_syntax_ns);
        //public static UriPrefixed SimpleLiteral = new UriPrefixed("smpl:", "", simple_literal);

        public static UriPrefixed RdfFirst = new UriPrefixed("rdf:", "first", rdf_syntax_ns);
        public static UriPrefixed RdfRest = new UriPrefixed("rdf:", "rest", rdf_syntax_ns);
           public static UriPrefixed RdfType = new UriPrefixed("rdf:", "type", rdf_syntax_ns);
        public static UriPrefixed Nil = new UriPrefixed("rdf:", "nil", rdf_syntax_ns);
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
        private const string rdf_syntax_ns = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        private const string xmlSchemaNs = "http://www.w3.org/2001/XMLSchema#";
        private const string xmlSchemaPrefix="xsd:";

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