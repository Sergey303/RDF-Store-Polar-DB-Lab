using RDFCommon.OVns;

namespace RDFCommon
{
    public interface INodeGenerator
    {
        ObjectVariants CreateUriNode(string p);
        ObjectVariants CreateUriNode(UriPrefixed uri);
        ObjectVariants CreateLiteralNode(string p);
        ObjectVariants CreateLiteralWithLang(string s, string lang);
        ObjectVariants CreateLiteralNode(int parse);
        ObjectVariants CreateLiteralNode(decimal p);
        ObjectVariants CreateLiteralNode(double p);
        ObjectVariants CreateLiteralNode(bool p);
        ObjectVariants CreateLiteralNode(string p, ObjectVariants sparqlUriNode);
        ObjectVariants CreateBlankNode(ObjectVariants graphName, string blankNodeString = null);
        ObjectVariants GetUri(string uri);      
        SpecialTypesClass SpecialTypes { get; }

        ObjectVariants GetUriNode(UriPrefixed uriPrefixed);
        ObjectVariants CreateBlankNode();
    }
}