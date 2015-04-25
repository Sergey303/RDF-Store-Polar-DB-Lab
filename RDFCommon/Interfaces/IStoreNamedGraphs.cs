using System.Collections.Generic;
using System.Linq;

namespace RDFCommon
{
    public interface IStoreNamedGraphs
    {
        IEnumerable<IGrouping<IGraphNode, IObjectNode>> GetTriplesWithSubjectPredicateFromGraphs(ISubjectNode subjectNode, IPredicateNode predicateNode, DataSet graphs);
        IEnumerable<IGrouping<IGraphNode, IPredicateNode>> GetTriplesWithSubjectObjectFromGraphs(ISubjectNode subjectNode, IObjectNode objectNode, DataSet graphs);
        IEnumerable<IGrouping<IGraphNode, Triple<ISubjectNode, IPredicateNode, IObjectNode>>> GetTriplesWithSubjectFromGraphs(ISubjectNode subjectNode, DataSet graphs);
        IEnumerable<IGrouping<IGraphNode, ISubjectNode>> GetTriplesWithPredicateObjectFromGraphs(IPredicateNode predicateNode, IObjectNode objectNode, DataSet graphs);
        IEnumerable<IGrouping<IGraphNode, Triple<ISubjectNode, IPredicateNode, IObjectNode>>> GetTriplesWithPredicateFromGraphs(IPredicateNode predicateNode, DataSet graphs);
        IEnumerable<IGrouping<IGraphNode, Triple<ISubjectNode, IPredicateNode, IObjectNode>>> GetTriplesWithObjectFromGraphs(IObjectNode objectNode, DataSet graphs);
        IEnumerable<IGrouping<IGraphNode, Triple<ISubjectNode, IPredicateNode, IObjectNode>>> GetTriplesFromGraphs(DataSet graphs);
        IGraph CreateGraph(IGraphNode sparqlUriNode);
        bool Contains(ISubjectNode sValue, IPredicateNode pValue, IObjectNode oValue, DataSet graphs);
        void DropGraph(IGraphNode sparqlGrpahRefTypeEnum);
        void Clear(IGraphNode uri);

        void Delete(IGraphNode g, IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples);
        void DeleteFromAll(IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples);
        void Insert(IGraphNode name, IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples);

      //  IGraph TryGetGraph(IUriNode graphUriNode);

        DataSet GetGraphs(ISubjectNode sValue, IPredicateNode pValue, IObjectNode oValue, DataSet graphs);
       //  Dictionary<IUriNode,IGraph> Named { get;  }
        void AddGraph(IGraphNode to, IGraph fromGraph);
        void ReplaceGraph(IGraphNode to, IGraph graph);
        IEnumerable<KeyValuePair<IGraphNode, long>> GetAllGraphCounts();
        void ClearAllNamedGraphs();
       // bool ContainsGraph(IUriNode to);

        IGraph GetGraph(IGraphNode graphUriNode);
        IEnumerable<ISubjectNode> GetAllSubjects(IGraphNode graphUri);
        bool Any(IGraphNode graphUri);
    }
}