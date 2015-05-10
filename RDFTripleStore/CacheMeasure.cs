using System;
using System.Collections.Generic;
using RDFCommon;
using RDFCommon.OVns;
using IGraph = RDFCommon.IGraph;

namespace TestingNs
{
    public class CacheMeasure : IGraph
    {
        private SecondStringGraph g;
        private readonly Queue<bool> spo;
        private readonly Queue<List<ObjectVariants>> spO;
        private readonly Queue<List<ObjectVariants>> sPo;
        private readonly Queue<List<ObjectVariants>> Spo;
        private readonly Queue<List<KeyValuePair<ObjectVariants, ObjectVariants>>> SPo;
        private readonly Queue<List<KeyValuePair<ObjectVariants, ObjectVariants>>> SpO;
        private readonly Queue<List<KeyValuePair<ObjectVariants, ObjectVariants>>> sPO;

        public readonly Queue<bool> usespo = new Queue<bool>();
        public readonly Queue<bool> useSpo = new Queue<bool>();
        public readonly Queue<bool> usesSo = new Queue<bool>();
        public readonly Queue<bool> usespO = new Queue<bool>();
        public readonly Queue<bool> useSPo = new Queue<bool>();
        public readonly Queue<bool> useSpO = new Queue<bool>();
        public readonly Queue<bool> usesPO = new Queue<bool>();

        private readonly HashSet<ObjectVariants> hasS=new HashSet<ObjectVariants>();
        private readonly HashSet<ObjectVariants> hasP=new HashSet<ObjectVariants>();
        private readonly HashSet<ObjectVariants> hasO=new HashSet<ObjectVariants>();
        private readonly HashSet<KeyValuePair<ObjectVariants, ObjectVariants>> hasSP=new HashSet<KeyValuePair<ObjectVariants, ObjectVariants>>();
        private readonly HashSet<KeyValuePair<ObjectVariants, ObjectVariants>> hasSO=new HashSet<KeyValuePair<ObjectVariants, ObjectVariants>>();
        private readonly HashSet<KeyValuePair<ObjectVariants, ObjectVariants>> hasPO = new HashSet<KeyValuePair<ObjectVariants, ObjectVariants>>();
        private readonly HashSet<Tuple<ObjectVariants, ObjectVariants, ObjectVariants>> hasSPO = new HashSet<Tuple<ObjectVariants, ObjectVariants, ObjectVariants>>();



        public bool TrainingMode { get; set; }

        public CacheMeasure(SecondStringGraph g)
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
                if (hasO.Contains(o))
                {
                    useSPo.Enqueue(true);
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
                {
                     hasO.Add(o);
                    useSPo.Enqueue(false);
                    foreach (var t in g.GetTriplesWithObject(o, createResult))
                        yield return t;
                }
            else
            {
                if (useSPo.Dequeue())
                    foreach (var pair in SPo.Dequeue())
                        yield return createResult(pair.Key, pair.Value);
                else
                    foreach (var t in g.GetTriplesWithObject(o, createResult))
                        yield return t;
            }
        }

        public IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants p, Func<ObjectVariants, ObjectVariants, T> createResult)
        {

            if (TrainingMode)
            {
                if (hasP.Contains(p))
                {
                    useSpO.Enqueue(true);
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
                {
                    hasP.Add(p);
                    useSPo.Enqueue(false);
                    foreach (var t in g.GetTriplesWithPredicate(p, createResult))
                        yield return t;
                }
            }
            else
                if (useSPo.Dequeue())
                    foreach (var pair in SpO.Dequeue())
                    yield return createResult(pair.Key, pair.Value);
                else foreach (var t in g.GetTriplesWithPredicate(p, createResult))
                        yield return t;

        }

        public IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants s,
            Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            if (TrainingMode)
            {
                if (hasS.Contains(s))
                {
                    usesPO.Enqueue(true);
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
                {
                    hasS.Add(s);
                    usesPO.Enqueue(false);
                    foreach (var t in g.GetTriplesWithSubject(s, createResult))
                        yield return t;
                }
            }
            else if (useSPo.Dequeue())
            {
                foreach (var pair in sPO.Dequeue())
                    yield return createResult(pair.Key, pair.Value);
            }
            else
                foreach (var t in g.GetTriplesWithSubject(s, createResult))
                    yield return t;
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            if (TrainingMode)
            {
                var key = new KeyValuePair<ObjectVariants, ObjectVariants>(subj, pred);
                if (hasSP.Contains(key))
                {
                    usespO.Enqueue(true);
                    var cacheList = new List<ObjectVariants>();
                    foreach (var o in g.GetTriplesWithSubjectPredicate(subj, pred))
                    {
                        cacheList.Add(o);
                        yield return o;
                    }
                    spO.Enqueue(cacheList);
                }
                else
                {
                    hasSP.Add(key);
                    usespO.Enqueue(false);
                    foreach (var o in g.GetTriplesWithSubjectPredicate(subj, pred))
                        yield return o;
                }
            }
            else
            {
                if (usespO.Dequeue())
                    foreach (var o in spO.Dequeue())
                        yield return o;
                else
                    foreach (var o in g.GetTriplesWithSubjectPredicate(subj, pred))
                        yield return o;
            }
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();

        }

        public IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            if (TrainingMode)
            {
                var key = new KeyValuePair<ObjectVariants, ObjectVariants>(pred, obj);
                if (hasPO.Contains(key))
                {
                   useSpo.Enqueue(true);
                    var cacheList = new List<ObjectVariants>();
                    foreach (var s in g.GetTriplesWithPredicateObject(pred, obj))
                    {
                        cacheList.Add(s);
                        yield return s;
                    }     
                    Spo.Enqueue(cacheList);
                }
                else
                {
                    hasPO.Add(key);
                    useSpo.Enqueue(false);
                    foreach (var s in g.GetTriplesWithPredicateObject(pred, obj))
                        yield return s;
                }
            }
            else
            {
                if (useSpo.Dequeue())
                    foreach (var s in Spo.Dequeue())
                        yield return s;
                else
                    foreach (var s in g.GetTriplesWithPredicateObject(pred, obj))
                        yield return s;
            }
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
                var key = new Tuple<ObjectVariants, ObjectVariants, ObjectVariants>(subject, predicate, obj);
                if (hasSPO.Contains(key))
                {
                    usespo.Enqueue(true);
                    var contains = g.Contains(subject, predicate, obj);
                    spo.Enqueue(contains);
                    return contains;
                }
                else
                {
                    hasSPO.Add(key);
                    usespo.Enqueue(false);
                    return g.Contains(subject, predicate, obj);
                }
            }
            else
                return usespo.Dequeue() ? spo.Dequeue() : g.Contains(subject, predicate, obj);
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