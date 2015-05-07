using System;
using System.Collections.Generic;
using System.IO;
using RDFCommon.OVns;

namespace RDFCommon
{
    public interface IGraph
    {
        string Name { get; }

        INodeGenerator NodeGenerator { get; }   
      
        void Clear();

        /// <summary>
        /// Selects all Triples where the Object is a given Node
        /// </summary>
        /// <param name="o">Node</param>
        /// <returns></returns>
        IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants o, Func<ObjectVariants, ObjectVariants, T> createResult);

        /// <summary>
        /// Selects all Triples where the Predicate is a given Node
        /// </summary>
        /// <param name="n">Node</param>
        /// <returns></returns>
        IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants p, Func<ObjectVariants, ObjectVariants,T> createResult);

        
        /// <summary>
        /// Selects all Triples where the Subject is a given Node
        /// </summary>
        /// <param name="n">Node</param>
        /// <returns></returns>
        IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants s, Func<ObjectVariants, ObjectVariants,T> createResult);

        /// <summary>
        /// Selects all Triples with the given Subject and Predicate
        /// </summary>
        /// <param name="subj">Subject</param>
        /// <param name="pred">Predicate</param>
        /// <returns></returns>
        IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred);

        /// <summary>
        /// Selects all Triples with the given Subject and Object
        /// </summary>
        /// <param name="subj">Subject</param>
        /// <param name="obj">Object</param>
        /// <returns></returns>
        IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj);

        /// <summary>
        /// Selects all Triples with the given Predicate and Object
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <param name="obj">Object</param>
        /// <returns></returns>
        IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj);
        IEnumerable<T> GetTriples<T>(Func<ObjectVariants,ObjectVariants,ObjectVariants, T> returns );


        void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o);
     
       
        bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj);
        void Delete(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj);
      
        IEnumerable<ObjectVariants> GetAllSubjects();
        long GetTriplesCount();

        bool Any();
        void FromTurtle(string gString);
    }
}