﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoTripleStore;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using RDFTripleStore.parsers.RDFTurtle;
using RDFTripleStore;

namespace TestingNs
{
    public class SecondGraphString : GaGraphStringBased, IGraph 
    {
        private readonly NodeGenerator ng = new NodeGenerator();


        public SecondGraphString(string path) :base(path)
        {
        
 
        }

        public ObjectVariants Name { get; private set; }
        public INodeGenerator NodeGenerator { get { return ng; } }
        public void Clear()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants o, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return base.GetTriplesWithObject(o)
                .ReadWritableTriples()
                .Select(row => createResult(ng.CreateUriNode(DecodeIRI(row[0])), ng.CreateUriNode(DecodeIRI(row[1]))));
        }

        public IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants p, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return base.GetTriplesWithPredicate(((IIriNode)p).UriString)
                .ReadWritableTriples()
                .Select(row => createResult(ng.CreateUriNode(DecodeIRI(row[0])), DecodeOV(row[2])));
        }

        public IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants s, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return base.GetTriplesWithSubject(((IIriNode)s).UriString)
                .ReadWritableTriples()
                .Select(row => createResult(ng.CreateUriNode(DecodeIRI(row[1])), DecodeOV(row[2])));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            return base.GetTriplesWithSubjectPredicate(((IIriNode)subj).UriString, ((IIriNode)pred).UriString)
                .ReadWritableTriples()
                .Select(row =>
                {
                    return DecodeOV(row[2]);
                }).ToArray();
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();
            return base.GetTriplesWithSubjectPredicate(((IIriNode)subj).UriString, obj)
                  .ReadWritableTriples()
                  .Select(row => ng.CreateUriNode(DecodeIRI(row[1]))).ToArray();
        }

        public IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            return base.GetTriplesWithPredicateObject(((IIriNode)pred).UriString, obj)
                .ReadWritableTriples()
                .Select(row =>
                {
                    return ng.CreateUriNode(DecodeIRI(row[0]));
                }).ToArray();
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

        public void Delete(IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> triples)
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
            //table.Clear();
            Build(ReadTripleStringsFromTurtle.LoadGraph(gString).Select(t => Tuple.Create(t.Subject.ToLowerInvariant(), t.Predicate.ToLowerInvariant(), t.Object)));
        }
    }
}