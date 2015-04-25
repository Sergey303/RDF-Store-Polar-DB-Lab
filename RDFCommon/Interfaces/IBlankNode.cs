namespace RDFCommon
{
    public interface IBlankNode :ISubjectNode,IObjectNode
    {
        string Name { get; }
        
    }
}