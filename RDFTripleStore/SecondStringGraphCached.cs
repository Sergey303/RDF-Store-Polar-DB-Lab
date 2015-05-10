using System;
using System.Collections.Generic;
using System.Linq;
using GoTripleStore;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using RDFTripleStore;
using RDFTurtleParser;
using IGraph = RDFCommon.IGraph;

namespace TestingNs
{
    public class SecondStringGraphCached : GaGraphStringBased, IGraph
    {
        private readonly NodeGenerator ng = new NodeGenerator();
        private readonly Cache<ObjectVariants, ObjectVariants, ObjectVariants, bool> spo;
        private readonly Cache<ObjectVariants, ObjectVariants, ObjectVariants[]> spO;
        private readonly Cache<ObjectVariants, ObjectVariants, ObjectVariants[]> sPo;
        private readonly Cache<ObjectVariants, ObjectVariants, ObjectVariants[]> Spo;
        private readonly Cache<ObjectVariants, KeyValuePair<ObjectVariants, ObjectVariants>[]> SPo;
        private readonly Cache<ObjectVariants, KeyValuePair<ObjectVariants, ObjectVariants>[]> SpO;
        private readonly Cache<ObjectVariants, KeyValuePair<ObjectVariants, ObjectVariants>[]> sPO;


        public SecondStringGraphCached(string path)
            : base(path)
        {

            spo = new Cache<ObjectVariants, ObjectVariants, ObjectVariants, bool>(
                (subject, predicate, obj) => Contains((object) subject.Content, (object) predicate.Content, obj));
            spO = new Cache<ObjectVariants, ObjectVariants, ObjectVariants[]>((s, p) =>
                base.GetTriplesWithSubjectPredicate(((IIriNode) s).UriString, ((IIriNode) p).UriString)
                    .Select(base.Dereference)
                    .Select(row => DecodeOV(row[2]))
                    .ToArray());
            sPo = new Cache<ObjectVariants, ObjectVariants, ObjectVariants[]>((s, o) =>
                null);
            Spo = new Cache<ObjectVariants, ObjectVariants, ObjectVariants[]>((p, o) =>
                base.GetTriplesWithPredicateObject(((IIriNode) p).UriString, o)
                    // .ReadWritableTriples()
                    .Select(base.Dereference)
                    .Select(row => new OV_iri(DecodeIRI(row[0]))).Cast<ObjectVariants>().ToArray());

            SPo = new Cache<ObjectVariants, KeyValuePair<ObjectVariants, ObjectVariants>[]>(o =>
                base.GetTriplesWithObject(o)
                    .Select(Dereference)
                    .Select(row =>
                        new KeyValuePair<ObjectVariants, ObjectVariants>(
                            new OV_iri(DecodeIRI(row[0])), new OV_iri((DecodeIRI(row[1])))))
                    .ToArray());
            SpO = new Cache<ObjectVariants, KeyValuePair<ObjectVariants, ObjectVariants>[]>(p =>
                base.GetTriplesWithPredicate(p)
                    .Select(Dereference)
                    .Select(row =>
                        new KeyValuePair<ObjectVariants, ObjectVariants>(
                            new OV_iri(DecodeIRI(row[0])), DecodeOV(row[2])))
                    .ToArray());
            sPO = new Cache<ObjectVariants, KeyValuePair<ObjectVariants, ObjectVariants>[]>(s =>
                base.GetTriplesWithObject(s)
                    .Select(Dereference)
                    .Select(row =>
                        new KeyValuePair<ObjectVariants, ObjectVariants>(
                            new OV_iri(DecodeIRI(row[1])), DecodeOV(row[2])))
                    .ToArray());
        }

        public string Name { get; private set; }
        public INodeGenerator NodeGenerator { get { return ng; } }
        public void Clear()
        {
            //throw new NotImplementedException();
        }

        public IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants o, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return SPo.Get(o).Select(pair => createResult(pair.Key, pair.Value));
        }

        public IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants p, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return SpO.Get(p).Select(pair => createResult(pair.Key, pair.Value));

        }

        public IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants s, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return sPO.Get(s).Select(pair => createResult(pair.Key, pair.Value));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            return spO.Get(subj, pred);
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();
            return base.GetTriplesWithSubjectPredicate(((IIriNode)subj).UriString, obj)
                //.ReadWritableTriples()
                .Select(base.Dereference)
                  .Select(row => new OV_iri(DecodeIRI(row[1]))).ToArray();
        }

        public IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            return Spo.Get(pred, obj);
        }

        public IEnumerable<T> GetTriples<T>(Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns)
        {
            throw new NotImplementedException();
        }

        public void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            throw new NotImplementedException();
        }

        public void Add(Triple<ObjectVariants, ObjectVariants, ObjectVariants> t)
        {
            throw new NotImplementedException();
        }

        public bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            return spo.Get(subject,predicate, obj);
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
            Build(new TripleGeneratorBufferedParallel(path, "g"));

        }

    }
}