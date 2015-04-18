namespace RDFTripleStore
{
    public class SpecialTypes
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
        public static UriPrefixed SimpleLiteral = new UriPrefixed("smpl:", "", XmlSchema.Default.simple_literal);

        public static UriPrefixed RdfFirst = new UriPrefixed("rdf:", "first", XmlSchema.Default.rdf_syntax_ns);
        public static UriPrefixed RdfRest = new UriPrefixed("rdf:", "rest", XmlSchema.Default.rdf_syntax_ns);
           public static UriPrefixed RdfType = new UriPrefixed("rdf:", XmlSchema.Default.type, XmlSchema.Default.rdf_syntax_ns);
        public static UriPrefixed Nil = new UriPrefixed("rdf:", XmlSchema.Default.nil, XmlSchema.Default.rdf_syntax_ns);

    }

}