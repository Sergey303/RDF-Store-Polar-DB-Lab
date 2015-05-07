﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDFCommon;
using RDFCommon.OVns;

namespace RDFTripleStore
{
    class RamDictGraph :IGraph
    {
        private INodeGenerator ng=new NodeGenerator();
        private Dictionary<ObjectVariants,
            KeyValuePair<Dictionary<string, ObjectVariants>, Dictionary<string, HashSet<string>>>> triples = new Dictionary<ObjectVariants, KeyValuePair<Dictionary<string, ObjectVariants>, Dictionary<string, HashSet<string>>>>(); 
        public string Name { get { return "g"; } }

        public INodeGenerator NodeGenerator
        {
            get { return ng; }          
        }

        public void Clear()
        {
           triples.Clear();
        }

        public IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants o, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            KeyValuePair<Dictionary<string, ObjectVariants>, Dictionary<string, HashSet<string>>> finded;
            if (triples.TryGetValue(o, out finded)) return Enumerable.Empty<T>();
            return null;
        }

        public IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants p, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants s, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetTriples<T>(Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns)
        {
            throw new NotImplementedException();
        }

        public void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            throw new NotImplementedException();
        }

        public bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetAllSubjects()
        {
            throw new NotImplementedException();
        }

        public long GetTriplesCount()
        {
            throw new NotImplementedException();
        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public void FromTurtle(string gString)
        {
            throw new NotImplementedException();
        }
    }
}
