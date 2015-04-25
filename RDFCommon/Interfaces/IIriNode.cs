using System;

namespace RDFCommon
{
    public interface IIriNode :ISubjectNode,IObjectNode,IPredicateNode,IGraphNode
    {
         string UriString { get; }           
    }
}
