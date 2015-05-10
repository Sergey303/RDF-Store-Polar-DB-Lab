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
    public class InterpretMeasure : IGraph
    {
        private SecondStringGraph g;
        private readonly Queue<bool> spo;
        private readonly Queue<List<ObjectVariants>> spO;
        private readonly Queue<List<ObjectVariants>> sPo;
        private readonly Queue<List<ObjectVariants>> Spo;
        private readonly Queue<List<KeyValuePair<ObjectVariants, ObjectVariants>>> SPo;
        private readonly Queue<List<KeyValuePair<ObjectVariants, ObjectVariants>>> SpO;
        private readonly Queue<List<KeyValuePair<ObjectVariants, ObjectVariants>>> sPO;

      public bool TrainingMode { get; set; }

        public InterpretMeasure(SecondStringGraph g)
        {
            this.g = g;

            spo = new Queue<bool>();
            Spo = new Queue<List<ObjectVariants>>();
            sPo = new Queue<List<ObjectVariants>>();
            spO = new Queue<List<ObjectVariants>>();
            SPo = new Queue<List<KeyValuePair<ObjectVariants, ObjectVariants>>>();
            SpO = new Queue<List<KeyValuePair<ObjectVariants, ObjectVariants>>>();
            sPO = new Queue<List<KeyValuePair<ObjectVariants, ObjectVariants>>>();
        }

        public string Name { get; private set; }
        public INodeGenerator NodeGenerator { get { return g.NodeGenerator; } }
        public void Clear()
        {
            //throw new NotImplementedException();
        }

        public IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants o,
            Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            if (TrainingMode)
            {
                var cacheList = new List<KeyValuePair<ObjectVariants, ObjectVariants>>();
                foreach (var t in g.GetTriplesWithObject(o, (s, p) =>
                {
                    cacheList.Add(new KeyValuePair<ObjectVariants, ObjectVariants>(s, p));
                    return createResult(s, p);
                }))
                    yield return t;
                SPo.Enqueue(cacheList);
            }
            else
                foreach (var pair in SPo.Dequeue())
                    yield return createResult(pair.Key, pair.Value);
        }

        public IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants p, Func<ObjectVariants, ObjectVariants, T> createResult)
        {

            if (TrainingMode)
            {
                var cacheList = new List<KeyValuePair<ObjectVariants, ObjectVariants>>();
                foreach (var t in g.GetTriplesWithPredicate(p, (s, o) =>
                {
                    cacheList.Add(new KeyValuePair<ObjectVariants, ObjectVariants>(s, o));
                    return createResult(s, o);
                }))
                    yield return t;
                SpO.Enqueue(cacheList);
            }
            else
                foreach (var pair in SpO.Dequeue())
                    yield return createResult(pair.Key, pair.Value);

        }

        public IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants s,
            Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            if (TrainingMode)
            {
                var cacheList = new List<KeyValuePair<ObjectVariants, ObjectVariants>>();
                foreach (var t in g.GetTriplesWithSubject(s, (p, o) =>
                {
                    cacheList.Add(new KeyValuePair<ObjectVariants, ObjectVariants>(p, o));
                    return createResult(p, o);
                }))
                    yield return t;
                sPO.Enqueue(cacheList);
            }
            else
                foreach (var pair in sPO.Dequeue())
                    yield return createResult(pair.Key, pair.Value);
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            if (TrainingMode)
            {
                var cacheList = new List<ObjectVariants>();
                foreach (var o in g.GetTriplesWithSubjectPredicate(subj, pred))
                {
                    cacheList.Add(o);
                   yield return o;
                }
                    
                spO.Enqueue(cacheList);
            }
            else
                foreach (var o in spO.Dequeue())
                    yield return o;
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();
       
        }

        public IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            if (TrainingMode)
            {
                var cacheList = new List<ObjectVariants>();
                foreach (var s in g.GetTriplesWithPredicateObject(pred, obj))
                {
                    cacheList.Add(s);
                    yield return s;
                }

                Spo.Enqueue(cacheList);
            }
            else
                foreach (var s in Spo.Dequeue())
                    yield return s;
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
            if (TrainingMode)
            {
                var contains = g.Contains(subject, predicate, obj);

                spo.Enqueue(contains);
                return contains;
            }
            else
                return spo.Dequeue();
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
            throw new NotImplementedException();

        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public void FromTurtle(string path)
        {
            //table.Clear();
          g.FromTurtle(path);

        }

    }
}