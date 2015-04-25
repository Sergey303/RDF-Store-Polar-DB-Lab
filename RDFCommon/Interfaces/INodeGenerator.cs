namespace RDFCommon
{
    public interface INodeGenerator
    {
        IIriNode CreateUriNode(string p);
        IIriNode CreateUriNode(UriPrefixed uri);
        ILiteralNode CreateLiteralNode(string p);
        ILiteralNode CreateLiteralWithLang(string s, string lang);
        ILiteralNode CreateLiteralNode(int parse);
        ILiteralNode CreateLiteralNode(decimal p);
        ILiteralNode CreateLiteralNode(double p);
        ILiteralNode CreateLiteralNode(bool p);
        ILiteralNode CreateLiteralNode(string p, IIriNode sparqlUriNode);
        IBlankNode CreateBlankNode(IGraphNode graphName, string blankNodeString = null);
        IIriNode GetUri(string uri);      
        SpecialTypesClass SpecialTypes { get; }
    }
}