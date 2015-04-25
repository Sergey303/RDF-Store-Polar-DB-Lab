using System.Collections.Generic;

namespace RDFCommon
{
    public interface IGraph
    {
        IGraphNode Name { get; }

        INodeGenerator NodeGenerator { get; }   
      
        void Clear();
      //  void Build(); // Это действие отсутствует в стандарте dotnetrdf!


    /// <summary>
        /// Selects all Triples where the Object is a given Node
        /// </summary>
        /// <param name="n">Node</param>
        /// <returns></returns>
        IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithObject(IObjectNode n);

        /// <summary>
        /// Selects all Triples where the Predicate is a given Node
        /// </summary>
        /// <param name="n">Node</param>
        /// <returns></returns>
        IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithPredicate(IPredicateNode n);

        
        /// <summary>
        /// Selects all Triples where the Subject is a given Node
        /// </summary>
        /// <param name="n">Node</param>
        /// <returns></returns>
        IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithSubject(ISubjectNode n);

        /// <summary>
        /// Selects all Triples with the given Subject and Predicate
        /// </summary>
        /// <param name="subj">Subject</param>
        /// <param name="pred">Predicate</param>
        /// <returns></returns>
        IEnumerable<IObjectNode> GetTriplesWithSubjectPredicate(ISubjectNode subj, IPredicateNode pred);

        /// <summary>
        /// Selects all Triples with the given Subject and Object
        /// </summary>
        /// <param name="subj">Subject</param>
        /// <param name="obj">Object</param>
        /// <returns></returns>
        IEnumerable<IPredicateNode> GetTriplesWithSubjectObject(ISubjectNode subj, IObjectNode obj);

        /// <summary>
        /// Selects all Triples with the given Predicate and Object
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <param name="obj">Object</param>
        /// <returns></returns>
        IEnumerable<ISubjectNode> GetTriplesWithPredicateObject(IPredicateNode pred, IObjectNode obj);
        IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriples();


        void Add(ISubjectNode s, IPredicateNode p, IObjectNode o);
     
       // void LoadFrom(IUriNode @from);

        void Insert(IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples);

        void Add(Triple<ISubjectNode, IPredicateNode, IObjectNode> t);
        bool Contains(ISubjectNode subject, IPredicateNode predicate, IObjectNode node);
        void Delete(IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples);
      
        IEnumerable<ISubjectNode> GetAllSubjects();
        long GetTriplesCount();

        bool Any();
        void FromTurtle(string gString);
    }
}