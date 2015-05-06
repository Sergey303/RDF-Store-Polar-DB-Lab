using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;

namespace RDFCommon
{
    public interface IStoreNamedGraphs
    {
        IEnumerable<ObjectVariants> GetPredicate(ObjectVariants subjectNode, ObjectVariants objectNode, ObjectVariants graph);
        IEnumerable<ObjectVariants> GetSubject(ObjectVariants predicateNode, ObjectVariants objectNode, ObjectVariants graph);
        IEnumerable<ObjectVariants> GetObject(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants graph);
        IEnumerable<ObjectVariants> GetGraph(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants objectNode);

        IEnumerable<T> GetTriplesWithSubjectPredicate<T>(ObjectVariants subjectNode, ObjectVariants predicateNode, Func<ObjectVariants, ObjectVariants, T> returns);
        IEnumerable<T> GetTriplesWithPredicateObject<T>(ObjectVariants predicateNode, ObjectVariants objectNode, Func<ObjectVariants, ObjectVariants, T> returns);
        IEnumerable<T> GetTriplesWithSubjectObject<T>(ObjectVariants subjectNode, ObjectVariants objectNode, Func<ObjectVariants, ObjectVariants, T> returns);

        IEnumerable<T> GetTriplesWithSubjectFromGraph<T>(ObjectVariants subjectNode, ObjectVariants graph, Func<ObjectVariants, ObjectVariants, T> returns);
        IEnumerable<T> GetTriplesWithPredicateFromGraph<T>(ObjectVariants predicateNode, ObjectVariants graph, Func<ObjectVariants, ObjectVariants, T> returns);
        IEnumerable<T> GetTriplesWithObjectFromGraph<T>(ObjectVariants objectNode, ObjectVariants graph, Func<ObjectVariants, ObjectVariants, T> returns);

        IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants predicateNode, Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns);
        IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants objectNode, Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns);
        IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants subjectNode, Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns);
        IEnumerable<T> GetTriplesFromGraph<T>(ObjectVariants graph, Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns);

        IGraph CreateGraph(string sparqlUriNode);
        bool Contains(ObjectVariants sValue, ObjectVariants pValue, ObjectVariants oValue, ObjectVariants graph);
        void DropGraph(string sparqlGrpahRefTypeEnum);
        void Clear(string uri);

        void Delete(ObjectVariants g, IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> triples);
        void DeleteFromAll(IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> triples);
        void Insert(ObjectVariants name, IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> triples);

      //  IGraph TryGetGraph(IUriNode graphUriNode);

       //  Dictionary<IUriNode,IGraph> Named { get;  }
        void AddGraph(string to, IGraph fromGraph);
        void ReplaceGraph(ObjectVariants to, IGraph graph);
        IEnumerable<KeyValuePair<ObjectVariants, long>> GetAllGraphCounts();

       // bool ContainsGraph(IUriNode to);

        IGraph GetGraph(string graphUriNode);
        IEnumerable<ObjectVariants> GetAllSubjects(ObjectVariants graphUri);
        bool Any(ObjectVariants graphUri);

        void ClearAllNamedGraphs();
        IEnumerable<T> GetAll<T>(Func<ObjectVariants, ObjectVariants, ObjectVariants, ObjectVariants, T> func);
    }
}