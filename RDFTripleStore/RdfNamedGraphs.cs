using System.Collections.Generic;
using System.Linq;
using RDFCommon;

namespace RDFTripleStore
{
    public class RdfNamedGraphs :IStoreNamedGraphs
    {
        private readonly Dictionary<IGraphNode, IGraph> named = new Dictionary<IGraphNode, IGraph>();

        public IEnumerable<IGrouping<IGraphNode, Triple<ISubjectNode, IPredicateNode, IObjectNode>>> GetTriplesFromGraphs(DataSet graphs)
        {
            return graphs.Where(dataset => Named.ContainsKey(dataset))
                .Select(dataset => new Grouping<IGraphNode, Triple<ISubjectNode, IPredicateNode, IObjectNode>>(dataset, Named[dataset].GetTriples()));
        }


        public IEnumerable<IGrouping<IGraphNode, IObjectNode>> GetTriplesWithSubjectPredicateFromGraphs(ISubjectNode subjectNode, IPredicateNode predicateNode, DataSet graphs)
        {    
            return graphs.Where(dataset => Named.ContainsKey(dataset))
                .Select(dataset => new Grouping<IGraphNode, IObjectNode>(dataset, Named[dataset].GetTriplesWithSubjectPredicate(subjectNode, predicateNode)));
        }

        public IEnumerable<IGrouping<IGraphNode, IPredicateNode>> GetTriplesWithSubjectObjectFromGraphs(ISubjectNode subjectNode, IObjectNode objectNode, DataSet graphs)
        {
            return graphs.Where(dataset => Named.ContainsKey(dataset))
              .Select(dataset => new Grouping<IGraphNode, IPredicateNode>(dataset, Named[dataset].GetTriplesWithSubjectObject(subjectNode, objectNode)));
        }

        public IEnumerable<IGrouping<IGraphNode, Triple<ISubjectNode, IPredicateNode, IObjectNode>>> GetTriplesWithSubjectFromGraphs(ISubjectNode subjectNode, DataSet graphs)
        {
            return graphs.Where(dataset => Named.ContainsKey(dataset))
              .Select(dataset => new Grouping<IGraphNode, Triple<ISubjectNode, IPredicateNode, IObjectNode>>(dataset, Named[dataset].GetTriplesWithSubject(subjectNode)));
        }

        public IEnumerable<IGrouping<IGraphNode, ISubjectNode>> GetTriplesWithPredicateObjectFromGraphs(IPredicateNode predicateNode, IObjectNode objectNode, DataSet graphs)
        {
            return graphs.Where(dataset => Named.ContainsKey(dataset))
              .Select(dataset => new Grouping<IGraphNode, ISubjectNode>(dataset, Named[dataset].GetTriplesWithPredicateObject(predicateNode, objectNode)));
        }

        public IEnumerable<IGrouping<IGraphNode, Triple<ISubjectNode, IPredicateNode, IObjectNode>>> GetTriplesWithPredicateFromGraphs(IPredicateNode predicateNode, DataSet graphs)
        {
            return graphs.Where(dataset => Named.ContainsKey(dataset))
              .Select(dataset => new Grouping<IGraphNode, Triple<ISubjectNode, IPredicateNode, IObjectNode>>(dataset, Named[dataset].GetTriplesWithPredicate(predicateNode)));
        }

        public IEnumerable<IGrouping<IGraphNode, Triple<ISubjectNode, IPredicateNode, IObjectNode>>> GetTriplesWithObjectFromGraphs(IObjectNode objectNode, DataSet graphs)
        {
            return  graphs.Where(dataset => Named.ContainsKey(dataset))
              .Select(dataset => new Grouping<IGraphNode, Triple<ISubjectNode, IPredicateNode, IObjectNode>>(dataset, Named[dataset].GetTriplesWithObject(objectNode)));
        }

        public IGraph CreateGraph(IGraphNode sparqlUriNode)
        {
            if (sparqlUriNode==null || Named.ContainsKey(sparqlUriNode)) throw new ContainsGraphWhenCreatingException(sparqlUriNode);
            var rdfInMemoryGraph = new RamListOfTriplesGraph(sparqlUriNode);
            Named.Add(sparqlUriNode, rdfInMemoryGraph);
            return rdfInMemoryGraph;
        }

        public bool Contains(ISubjectNode subject, IPredicateNode predicate, IObjectNode @object, DataSet graphs)
        {
            IGraph g;
            return graphs.Any(uri => Named.TryGetValue(uri, out g) && g.Contains(subject, predicate, @object));
        }

        public void DropGraph(IGraphNode updateGraph)
        {
            if (!Named.ContainsKey(updateGraph))
                throw new NoGraphExeption(updateGraph);
            Named.Remove(updateGraph);
        }



        public void Clear(IGraphNode uri)
        {
            IGraph g;
            if (!Named.TryGetValue(uri, out g))
                throw new NoGraphExeption(uri);
            g.Clear();
        }



        public bool ContainsGraph(IGraphNode uri)
        {
            return Named.ContainsKey(uri);
        }
        public void Delete(IGraphNode uri, IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples)
        {
           Named[uri].Delete(triples);
        }

        public void DeleteFromAll(IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples)
        {
            foreach (var graph in named)
            {
                
            }
        }

        public void Insert(IGraphNode name, IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples)
        {
            if(name==null) ((IGraph)this).Insert(triples);
            else  Named[name].Insert(triples);
        }

        public IGraph TryGetGraph(IGraphNode graphUriNode)
        {
            throw new System.NotImplementedException();
        }


        public DataSet GetGraphs(ISubjectNode sValue, IPredicateNode pValue, IObjectNode oValue, DataSet graphsSeq)
        {
           var graphs = graphsSeq.ToArray();
            IGraph graph;
            return new DataSet(
                graphs.Length == 0
                    ? Named.Where(g => g.Value.Contains(sValue, pValue, oValue)).Select(pair => pair.Key)
                    : graphs.Where(g => Named.TryGetValue(g, out graph) && graph.Contains(sValue, pValue, oValue)));
        }

        public void AddGraph(IGraphNode to, IGraph fromGraph)
        {
            named.Add(to, fromGraph);
        }

        public void ReplaceGraph(IGraphNode to, IGraph graph)
        {
            named.Add(to, graph);
        }

        public IEnumerable<KeyValuePair<IGraphNode, long>> GetAllGraphCounts()
        {
            return Named.Select(pair => new KeyValuePair<IGraphNode, long>(pair.Key, pair.Value.GetTriplesCount()));
        }

        public void ClearAllNamedGraphs()
        {
            named.Clear();
        }

        public Dictionary<IGraphNode, IGraph> Named { get { return named; } }

        public IGraph GetGraph(IGraphNode uri)
        {
            IGraph g;
            if (!Named.TryGetValue(uri, out g))
                throw new NoGraphExeption(uri);
            return g;
        }

        public IEnumerable<ISubjectNode> GetAllSubjects(IGraphNode graphUri)
        {
            throw new System.NotImplementedException();
        }

        public bool Any(IGraphNode graphUri)
        {
            throw new System.NotImplementedException();
        }
    }
}