using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using IGraph = RDFCommon.IGraph;

namespace TestingNs
{
    public class CacheMeasure : SecondStringGraph, IGraph
    {
    
     

        public readonly Queue<bool> useCache = new Queue<bool>();


        private readonly HashSet<ObjectVariants> hasS=new HashSet<ObjectVariants>();
        private readonly HashSet<ObjectVariants> hasP=new HashSet<ObjectVariants>();
        private readonly HashSet<ObjectVariants> hasO=new HashSet<ObjectVariants>();
        private readonly HashSet<KeyValuePair<ObjectVariants, ObjectVariants>> hasSP=new HashSet<KeyValuePair<ObjectVariants, ObjectVariants>>();
        private readonly HashSet<KeyValuePair<ObjectVariants, ObjectVariants>> hasSO=new HashSet<KeyValuePair<ObjectVariants, ObjectVariants>>();
        private readonly HashSet<KeyValuePair<ObjectVariants, ObjectVariants>> hasPO = new HashSet<KeyValuePair<ObjectVariants, ObjectVariants>>();
        private readonly HashSet<Tuple<ObjectVariants, ObjectVariants, ObjectVariants>> hasSPO = new HashSet<Tuple<ObjectVariants, ObjectVariants, ObjectVariants>>();
        protected readonly Queue<object> history = new Queue<object>();



       

        public CacheMeasure(string path) : base(path)
        {

        }

        

        public override IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants o,
            Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            if (TrainingMode)
                if (hasO.Contains(o))
                {
                    useCache.Enqueue(true);
                    var cacheList = new List<KeyValuePair<ObjectVariants, ObjectVariants>>();
                    foreach (var t in base.GetTriplesWithObject(o, (s, p) =>
                    {
                        cacheList.Add(new KeyValuePair<ObjectVariants, ObjectVariants>(s, p));
                        return createResult(s, p);
                    }))
                        yield return t;
                    history.Enqueue(cacheList);         
                }
                else
                {
                     hasO.Add(o);
                     useCache.Enqueue(false);
                    foreach (var t in base.GetTriplesWithObject(o, createResult))
                        yield return t;
                }
            else
            {
                if (useCache.Dequeue())
                    foreach (var pair in (List<KeyValuePair<ObjectVariants, ObjectVariants>>)history.Dequeue())
                        yield return createResult(pair.Key, pair.Value);
                else
                    foreach (var t in base.GetTriplesWithObject(o, createResult))
                        yield return t;
            }
        }

        public bool TrainingMode { get; set; }

        public override IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants p, Func<ObjectVariants, ObjectVariants, T> createResult)
        {

            if (TrainingMode)
            {
                if (hasP.Contains(p))
                {
                    useCache.Enqueue(true);
                    var cacheList = new List<KeyValuePair<ObjectVariants, ObjectVariants>>();
                    foreach (var t in base.GetTriplesWithPredicate(p, (s, o) =>
                    {
                        cacheList.Add(new KeyValuePair<ObjectVariants, ObjectVariants>(s, o));
                        return createResult(s, o);
                    }))
                        yield return t;
                    history.Enqueue(cacheList);
                }
                else
                {
                    hasP.Add(p);
                    useCache.Enqueue(false);
                    foreach (var t in base.GetTriplesWithPredicate(p, createResult))
                        yield return t;
                }
            }
            else
                if (useCache.Dequeue())
                    foreach (var pair in (List<KeyValuePair<ObjectVariants, ObjectVariants>>)history.Dequeue())
                    yield return createResult(pair.Key, pair.Value);
                else foreach (var t in base.GetTriplesWithPredicate(p, createResult))
                        yield return t;

        }

        public override IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants s,
            Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            if (TrainingMode)
            {
                if (hasS.Contains(s))
                {
                    useCache.Enqueue(true);
                    var cacheList = new List<KeyValuePair<ObjectVariants, ObjectVariants>>();
                    foreach (var t in base.GetTriplesWithSubject(s, (p, o) =>
                    {
                        cacheList.Add(new KeyValuePair<ObjectVariants, ObjectVariants>(p, o));
                        return createResult(p, o);
                    }))
                        yield return t;
                    history.Enqueue(cacheList);
                }
                else
                {
                    hasS.Add(s);
                    useCache.Enqueue(false);
                    foreach (var t in base.GetTriplesWithSubject(s, createResult))
                        yield return t;
                }
            }
            else if (useCache.Dequeue())
            {
                foreach (var pair in (List<KeyValuePair<ObjectVariants, ObjectVariants>>)history.Dequeue())
                    yield return createResult(pair.Key, pair.Value);
            }
            else
                foreach (var t in base.GetTriplesWithSubject(s, createResult))
                    yield return t;
        }

        public override IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            if (TrainingMode)
            {
                var key = new KeyValuePair<ObjectVariants, ObjectVariants>(subj, pred);
                if (hasSP.Contains(key))
                {
                    useCache.Enqueue(true);
                    var source = base.GetTriplesWithSubjectPredicate(subj, pred).ToArray();        
                    history.Enqueue(source);
                    return source;
                }
                else
                {
                    hasSP.Add(key);
                    useCache.Enqueue(false);
                    return base.GetTriplesWithSubjectPredicate(subj, pred).ToArray();
                }
            }
            else
            {
                if (useCache.Dequeue())
                    return (ObjectVariants[]) history.Dequeue();

                else
                    return base.GetTriplesWithSubjectPredicate(subj, pred).ToArray();
            }
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();

        }

        public override IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            if (TrainingMode)
            {
                var key = new KeyValuePair<ObjectVariants, ObjectVariants>(pred, obj);
                if (hasPO.Contains(key))
                {
                    useCache.Enqueue(true);
                    var results = base.GetTriplesWithPredicateObject(pred, obj).ToArray();
                    
                    history.Enqueue(results);
                    return results;
                }
                else
                {
                    hasPO.Add(key);
                    useCache.Enqueue(false);
                    return base.GetTriplesWithPredicateObject(pred, obj).ToArray();
                }
            }
            else
            {
                if (useCache.Dequeue())
                    return (ObjectVariants[])history.Dequeue();

                else
                    return base.GetTriplesWithPredicateObject(pred, obj).ToArray();
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

        public override bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            if (TrainingMode)
            {
                var key = new Tuple<ObjectVariants, ObjectVariants, ObjectVariants>(subject, predicate, obj);
                if (hasSPO.Contains(key))
                {
                    useCache.Enqueue(true);
                    var contains = base.Contains(subject, predicate, obj);
                    history.Enqueue(contains);
                    return contains;
                }
                else
                {
                    hasSPO.Add(key);
                    useCache.Enqueue(false);
                    return base.Contains(subject, predicate, obj);
                }
            }
            else
                return useCache.Dequeue() ? (bool)history.Dequeue() : base.Contains(subject, predicate, obj);
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
            base.FromTurtle(path);

        }

    }
}