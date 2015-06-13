using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;

namespace RDFTripleStore
{
    public class RdfNamedGraphs :IStoreNamedGraphs
    {
       private readonly Dictionary<string,IGraph> named=new Dictionary<string, IGraph>();
        private NodeGenerator ng;

        public RdfNamedGraphs(NodeGenerator ng, Func<string, IGraph> graphCtor)
        {
            getGraphUriByName = g => this.ng.GetUri(g.Name);
            this.ng = ng;
            this.graphCtor = graphCtor;
        }

        public IEnumerable<ObjectVariants> GetPredicate(ObjectVariants subjectNode, ObjectVariants objectNode, ObjectVariants graph)
        {
            IGraph g;
            if (named.TryGetValue(graph.ToString(), out g)) return Enumerable.Empty<ObjectVariants>();
            return g.GetTriplesWithSubjectObject(subjectNode, objectNode);
        }

        public IEnumerable<ObjectVariants> GetSubject(ObjectVariants predicateNode, ObjectVariants objectNode, ObjectVariants graph)
        {
            IGraph g;
            if (named.TryGetValue(graph.ToString(), out g)) return Enumerable.Empty<ObjectVariants>();
            return g.GetTriplesWithPredicateObject(predicateNode, objectNode);
        }

        public IEnumerable<ObjectVariants> GetObject(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants graph)
        {
            IGraph g;
            if (named.TryGetValue(graph.ToString(), out g)) return Enumerable.Empty<ObjectVariants>();
            return g.GetTriplesWithSubjectPredicate(subjectNode, predicateNode);
        }

        public IEnumerable<ObjectVariants> GetGraph(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants objectNode)
        {

            return named.Values.Where(g => g.Contains(subjectNode, predicateNode, objectNode)).Select(getGraphUriByName);
        }

        private readonly Func<IGraph, ObjectVariants> getGraphUriByName;
        private readonly Func<string, IGraph> graphCtor;

        public IEnumerable<T> GetTriplesWithSubjectPredicate<T>(ObjectVariants subjectNode, ObjectVariants predicateNode, Func<ObjectVariants, ObjectVariants, T> returns)
        {
            return named.Values.SelectMany(g => g.GetTriplesWithSubjectPredicate(subjectNode, predicateNode).Select(o => returns(o,getGraphUriByName(g))));
        }

        public IEnumerable<T> GetTriplesWithPredicateObject<T>(ObjectVariants predicateNode, ObjectVariants objectNode, Func<ObjectVariants, ObjectVariants, T> returns)
        {
            return named.Values.SelectMany(g => g.GetTriplesWithPredicateObject(predicateNode, objectNode).Select(o => returns(o, getGraphUriByName(g))));
        }

        public IEnumerable<T> GetTriplesWithSubjectObject<T>(ObjectVariants subjectNode, ObjectVariants objectNode, Func<ObjectVariants, ObjectVariants, T> returns)
        {
            return named.Values.SelectMany(g => g.GetTriplesWithSubjectObject(subjectNode, objectNode).Select(o => returns(o, getGraphUriByName(g))));
        }

        public IEnumerable<T> GetTriplesWithSubjectFromGraph<T>(ObjectVariants subjectNode, ObjectVariants graph, Func<ObjectVariants, ObjectVariants, T> returns)
        {
            IGraph g;
            if (named.TryGetValue(graph.ToString(), out g)) return Enumerable.Empty<T>();
            return g.GetTriplesWithSubject(subjectNode, returns);
        }

        public IEnumerable<T> GetTriplesWithPredicateFromGraph<T>(ObjectVariants predicateNode, ObjectVariants graph, Func<ObjectVariants, ObjectVariants, T> returns)
        {
            IGraph g;                                     
            if (named.TryGetValue(graph.ToString(), out g)) return Enumerable.Empty<T>();
            return g.GetTriplesWithPredicate(predicateNode, returns);
        }

        public IEnumerable<T> GetTriplesWithObjectFromGraph<T>(ObjectVariants objectNode, ObjectVariants graph, Func<ObjectVariants, ObjectVariants, T> returns)
        {                     
            IGraph g;
            if (named.TryGetValue(graph.ToString(), out g)) return Enumerable.Empty<T>();
            return g.GetTriplesWithObject(objectNode, returns);
        }

        public IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants predicateNode, Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns)
        {
            return named.Values.SelectMany(g => g.GetTriplesWithPredicate(predicateNode, (s, o) => returns(s, o, getGraphUriByName(g))));
        }

        public IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants objectNode, Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns)
        {
            return named.Values.SelectMany(g => g.GetTriplesWithObject(objectNode, (s, p) => returns(s, p, getGraphUriByName(g))));
        }

        public IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants subjectNode, Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns)
        {
            return named.Values.SelectMany(g => g.GetTriplesWithSubject(subjectNode, (p, o) => returns(p, o, getGraphUriByName(g))));
        }

        public IEnumerable<T> GetTriplesFromGraph<T>(ObjectVariants graph, Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns)
        {
            IGraph g;
            if (named.TryGetValue(graph.ToString(), out g)) return Enumerable.Empty<T>();
            return g.GetTriples(returns);
        }

        public IGraph CreateGraph(string g)
        {
            var graph = graphCtor(g);
            named.Add(g, graph);
            return graph;
        }

        public bool Contains(ObjectVariants sValue, ObjectVariants pValue, ObjectVariants oValue, ObjectVariants graph)
        {
            return named.Values.Any(g => g.Contains(sValue, pValue, oValue));
        }

        public void DropGraph(string g)
        {
            named.Remove(g);
        }

        public void Clear(string uri)
        {
            IGraph g;
            if(named.TryGetValue(uri, out g))
                g.Clear();
        }

        public void Delete(ObjectVariants g, ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            IGraph graph;
            if (named.TryGetValue(g.ToString(), out graph))
                graph.Delete(s, p, o);
        }

        //public void DeleteFromAll(IEnumerable<TripleOV> triples)
        //{
        //    throw new NotImplementedException();
        //}

        public void Add(ObjectVariants g, ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            IGraph graph;
            if (named.TryGetValue(g.ToString(), out graph))
                graph.Add(s, p, o);
        }

        public void AddGraph(string to, IGraph fromGraph)
        {
          named.Add(to, fromGraph);
        }

      
        public IEnumerable<KeyValuePair<string, long>> GetAllGraphCounts()
        {
            return named.Select(pair => new KeyValuePair<string, long>(pair.Key, pair.Value.GetTriplesCount()));
        }

        public IGraph GetGraph(string graphUriNode)
        {
            throw new NotImplementedException();
        }

      

        public bool Any(string graphUri)
        {
            throw new NotImplementedException();
        }

        public void ClearAllNamedGraphs()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll<T>(Func<ObjectVariants, ObjectVariants, ObjectVariants, ObjectVariants, T> func)
        {
            throw new NotImplementedException();
        }
    }
}