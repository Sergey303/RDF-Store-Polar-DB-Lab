using System;
using System.Collections.Generic;
using System.Linq;
using GoTripleStore;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using RDFTripleStore;
using RDFTurtleParser;

namespace TestingNs
{
    public class SecondStringGraph : GaGraphStringBased, IGraph 
    {
        private readonly NodeGenerator ng = new NodeGenerator();


        public SecondStringGraph(string path)
            : base(path)
        {
        
 
        }

        public string Name { get; private set; }
        public INodeGenerator NodeGenerator { get { return ng; } }
        public void Clear()
        {
            //throw new NotImplementedException();
        }

        public IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants o, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return base.GetTriplesWithObject(o)
                .Select(base.Dereference)
                //.ReadWritableTriples()
                .Select(row => createResult(new OV_iri(DecodeIRI(row[0])), new OV_iri((DecodeIRI(row[1])))));
        }

        public IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants p, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return base.GetTriplesWithPredicate(((IIriNode)p).UriString)
             //   .ReadWritableTriples()
                .Select(base.Dereference)
                .Select(row => createResult(new OV_iri(DecodeIRI(row[0])), DecodeOV(row[2])));
        }

        public IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants s, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return base.GetTriplesWithSubject(((IIriNode)s).UriString)
                //ReadWritableTriples()
                .Select(base.Dereference)
                .Select(row => createResult(new OV_iri(DecodeIRI(row[1])), DecodeOV(row[2])));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            return base.GetTriplesWithSubjectPredicate(((IIriNode)subj).UriString, ((IIriNode)pred).UriString)
              //  .ReadWritableTriples()
                .Select(base.Dereference)
                .Select(row =>
                {
                    return DecodeOV(row[2]);
                }).ToArray();
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();
            return base.GetTriplesWithSubjectPredicate(((IIriNode)subj).UriString, obj)
                  //.ReadWritableTriples()
                .Select(base.Dereference)
                  .Select(row => new  OV_iri(DecodeIRI(row[1]))).ToArray();
        }

        public IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
         
            return base.GetTriplesWithPredicateObject(((IIriNode)pred).UriString, obj)
               // .ReadWritableTriples()
                 .Select(base.Dereference)
                .Select(row => new OV_iri(DecodeIRI(row[0]))).ToArray();
        }

        public IEnumerable<T> GetTriples<T>(Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns)
        {
            throw new NotImplementedException();
        }

        public void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            throw new NotImplementedException();
        }

        public void Insert(IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> triples)
        {
            throw new NotImplementedException();
        }

        public void Add(Triple<ObjectVariants, ObjectVariants, ObjectVariants> t)
        {
            throw new NotImplementedException();
        }

        public bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            return base.Contains((object)subject.Content, (object)predicate.Content, obj);
        }

        public void Delete(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetAllSubjects()
        {
            throw new NotImplementedException();
        }

        public long GetTriplesCount()
        {
           return base.GetTriples().Count();
        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public void FromTurtle(string path)
        {
            //table.Clear();
            //Build(ReadTripleStringsFromTurtle.LoadGraph(gString).Select(t => Tuple.Create(t.Subject.ToLowerInvariant(), t.Predicate.ToLowerInvariant(), t.Object)));
        Build(new TripleGeneratorBufferedParallel(path, "g"));

        }
        
    }
}