using RDFCommon.OVns;

namespace RDFCommon
{
    public interface INodeGenerator
    {
        ObjectVariants CreateLiteralNode(string p, string sparqlUriNode);
        ObjectVariants CreateBlankNode(string fullgraphName, string blankNodeString = null);
        ObjectVariants GetUri(string uri);      
        SpecialTypesClass SpecialTypes { get; }

        
        ObjectVariants CreateBlankNode();
        ObjectVariants AddIri(string iri);
    }
}