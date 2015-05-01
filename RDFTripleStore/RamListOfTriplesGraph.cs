using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RDFCommon;
using RDFTripleStore.parsers.RDFTurtle;

namespace RDFTripleStore
{
    public class RamListOfTriplesGraph : NodeGenerator, IGraph 
    {
        public RamListOfTriplesGraph(IGraphNode name)
        {
          //  Name = Guid.NewGuid().ToString();
            Name = name;
            
        }
        private readonly List<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples=new List<Triple<ISubjectNode, IPredicateNode, IObjectNode>>();

        public RamListOfTriplesGraph()
        {
           
        }

        public IEnumerable<IObjectNode> GetTriplesWithSubjectPredicate(ISubjectNode subjectNode, IPredicateNode predicateNode)
        {
            return triples.Where(triple => triple.Subject .Equals( subjectNode) && triple.Predicate .Equals( predicateNode)).Select(triple => triple.Object);
        }

        public IEnumerable<IPredicateNode> GetTriplesWithSubjectObject(ISubjectNode subjectNode, IObjectNode objectNode)
        {
            return triples.Where(triple => triple.Subject .Equals( subjectNode) && triple.Object .Equals( objectNode)).Select(triple => triple.Predicate);

        }

        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithSubject(ISubjectNode subjectNode)
        {
            return triples.Where(triple => triple.Subject .Equals( subjectNode)).ToArray();
        }

        public IEnumerable<ISubjectNode> GetTriplesWithPredicateObject(IPredicateNode predicateNode, IObjectNode objectNode)
        {
            return triples.Where(triple => triple.Predicate .Equals( predicateNode) && triple.Object .Equals( objectNode)).Select(triple => triple.Subject);
        }

        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithPredicate(IPredicateNode predicateNode)
        {
            return triples.Where(triple => triple.Predicate .Equals( predicateNode));
        }

     

        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithObject(IObjectNode o)
        {
            return triples.Where(triple => triple.Object .Equals( o));
        }

        public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriples()
        {
            return triples;
        }

        public bool Contains(ISubjectNode subject, IPredicateNode predicate, IObjectNode @object)
        {
           return triples.Any(triple => triple.Subject.Equals(subject) && triple.Predicate.Equals(predicate) && triple.Object.Equals(@object));
        }

        public void Delete(IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> ts)
        {
            foreach (var triple in ts)
                triples.Remove(triple);
        }

   

        public IEnumerable<ISubjectNode> GetAllSubjects()
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
                        new Triple<ISubjectNode, IPredicateNode, IObjectNode>(
                            NodeGenerator.CreateUriNode(t.Subject),
                            NodeGenerator.CreateUriNode(t.Predicate), 
                            (IObjectNode) t.Object))));
        }


        public IGraphNode Name { get; private set; }
        public INodeGenerator NodeGenerator { get { return this; }}

        public void Clear()
        {
           triples.Clear();
        }

    
        public void Add(ISubjectNode s, IPredicateNode p, IObjectNode o)
        {
           triples.Add(new Triple<ISubjectNode, IPredicateNode, IObjectNode>(s,p,o));
        }

    
        //public void LoadFrom(IUriNode @from)
        //{
        //    throw new NotImplementedException();
        //}


        

        public void Insert(IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples)
        {
          this.triples.AddRange(triples);
        }

        public void Add(Triple<ISubjectNode, IPredicateNode, IObjectNode> t)
        {
         triples.Add(t);
        }

        public void AddRange(IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples)
        {
            this.triples.AddRange(triples);
        }
    }
}
