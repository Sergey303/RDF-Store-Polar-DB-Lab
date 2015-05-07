using RDFCommon.OVns;

namespace RDFCommon
{
    public interface INodeGenerator
    {
        ObjectVariants CreateLiteralNode(string p, string sparqlUriNode);
        ObjectVariants GetUri(string uri);      
        SpecialTypesClass SpecialTypes { get; }


        ObjectVariants CreateBlankNode();
        ObjectVariants CreateBlankNode(string blankNodeString, string fullgraphName=null);
        string CreateBlank();
        string CreateBlank(string fullgraphName, string blankNodeString);
        ObjectVariants AddIri(string iri);
    }
}