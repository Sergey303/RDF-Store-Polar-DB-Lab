using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using RDFTripleStore.parsers.RDFTurtle;

namespace RDFTripleStore
{
    public class RamListOfTriplesGraph : NodeGenerator, IGraph 
    {
        public RamListOfTriplesGraph(ObjectVariants name)
        {
          //  Name = Guid.NewGuid().ToString();
            Name = name;
            
        }
        private readonly List<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> triples=new List<Triple<ObjectVariants, ObjectVariants, ObjectVariants>>();

        public RamListOfTriplesGraph()
        {
           
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subjectNode, ObjectVariants predicateNode)
        {
            return triples.Where(triple => triple.Subject .Equals( subjectNode) && triple.Predicate .Equals( predicateNode)).Select(triple => triple.Object);
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subjectNode, ObjectVariants objectNode)
        {
            return triples.Where(triple => triple.Subject .Equals( subjectNode) && triple.Object .Equals( objectNode)).Select(triple => triple.Predicate);

        }

        public IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants subjectNode, Func<ObjectVariants,ObjectVariants, T> returns)
        {
            return triples.Where(triple => triple.Subject.Equals(subjectNode)).Select(triple =>  returns(triple.Predicate, triple.Object));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants predicateNode, ObjectVariants objectNode)
        {
            return triples.Where(triple => triple.Predicate .Equals( predicateNode) && triple.Object .Equals( objectNode)).Select(triple => triple.Subject);
        }

        public IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants predicateNode, Func<ObjectVariants, ObjectVariants, T> returns)
        {
            return triples.Where(triple => triple.Predicate .Equals( predicateNode)).Select(triple => returns(triple.Subject, triple.Object));
        }



        public IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants o, Func<ObjectVariants, ObjectVariants, T> returns)
        {
            return triples.Where(triple => triple.Object.Equals(o)).Select(triple => returns(triple.Subject, triple.Predicate)); 
        }

        public IEnumerable<T> GetTriples<T>(Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns)
        {
            return triples.Select(triple => returns(triple.Subject, triple.Predicate, triple.Object));
        }

        public bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants @object)
        {
           return triples.Any(triple => triple.Subject.Equals(subject) && triple.Predicate.Equals(predicate) && triple.Object.Equals(@object));
        }

        public void Delete(IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> ts)
        {
            foreach (var triple in ts)
                triples.Remove(triple);
        }

   

        public IEnumerable<ObjectVariants> GetAllSubjects()
        {
            return triples.Select(t => t.Subject).Distinct();
        }

        public long GetTriplesCount()
        {
            return triples.Count;
        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public void FromTurtle(string fileName)
        {
            var generator = new TripleGeneratorBufferedParallel(fileName, Name.ToString());
            generator.Start(list => triples.AddRange(
                list.Select(
                    t =>    
                        new Triple<ObjectVariants, ObjectVariants, ObjectVariants>(
                            NodeGenerator.CreateUriNode(t.Subject),
                            NodeGenerator.CreateUriNode(t.Predicate), 
                            (ObjectVariants) t.Object))));
        }


        public ObjectVariants Name { get; private set; }
        public INodeGenerator NodeGenerator { get { return this; }}

        public void Clear()
        {
           triples.Clear();
        }

    
        public void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
           triples.Add(new Triple<ObjectVariants, ObjectVariants, ObjectVariants>(s,p,o));
        }

    
        //public void LoadFrom(IUriNode @from)
        //{
        //    throw new NotImplementedException();
        //}


        

        public void Insert(IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> triples)
        {
          this.triples.AddRange(triples);
        }

        public void Add(Triple<ObjectVariants, ObjectVariants, ObjectVariants> t)
        {
         triples.Add(t);
        }

        public void AddRange(IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> triples)
        {
            this.triples.AddRange(triples);
        }
    }
}
