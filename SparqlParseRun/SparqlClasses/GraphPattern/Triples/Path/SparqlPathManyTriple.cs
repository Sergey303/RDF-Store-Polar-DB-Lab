using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples.Path
{
    public class SparqlPathManyTriple : ISparqlGraphPattern
    {
        private readonly SparqlPathTranslator predicatePath;           
        private Dictionary<ObjectVariants, HashSet<ObjectVariants>> bothVariablesCacheBySubject, bothVariablesCacheByObject;
        private KeyValuePair<ObjectVariants, ObjectVariants>[] bothVariablesChache;
        private VariableNode sVariableNode;
        private VariableNode oVariableNode;
        private RdfQuery11Translator q;

        public SparqlPathManyTriple(ObjectVariants subject, SparqlPathTranslator pred, ObjectVariants @object, RdfQuery11Translator q)
        {
            this.predicatePath = pred;
            Subject = subject;
            Object = @object;
            this.q = q;
            sVariableNode = Subject as VariableNode;
            oVariableNode = Object as VariableNode;
        }
        

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            var bindings = variableBindings;




            Queue<ObjectVariants> newSubjects = new Queue<ObjectVariants>();
            IEnumerable<ObjectVariants> fromVariable = null;
            if (sVariableNode == null)
            {
                if (oVariableNode == null)
                {
                    newSubjects.Enqueue(Subject);
                    if (TestSOConnection(Subject, Object))
                        foreach (var r in bindings)
                            yield return r;
                }
                else
                {
                    ObjectVariants o = null;
                    newSubjects.Enqueue(Subject);
                    foreach (var binding in bindings)
                    {
                        o = binding[oVariableNode];
                        if (o!=null)
                        {
                            if (TestSOConnection(Subject, o))
                                yield return binding;
                        }
                        else
                        {
                            if (fromVariable == null)
                                fromVariable = GetAllSConnections(Subject);
                            foreach (var node in fromVariable)
                                yield return new SparqlResult(binding, node, oVariableNode);
                        }
                    }
                }
            }
            else
            {
                if (oVariableNode == null) //s variable o const
                {
                    ObjectVariants s = null;
                    foreach (var binding in bindings)
                    {
                        s = binding[sVariableNode];
                        if (s!=null)
                        {
                            if (TestSOConnection(s, Object))
                                yield return binding;
                        }
                        else
                        {
                            if (fromVariable == null)
                                fromVariable = GetAllOConnections(Object);
                            foreach (var node in fromVariable)
                                yield return new SparqlResult(binding, node, oVariableNode);
                        }
                    }
                }
                else // both variables
                {
                    ObjectVariants o = null;
                    ObjectVariants s = null;
                
                    if (bindings.All(
                            binding => binding.ContainsKey(sVariableNode) || binding.ContainsKey(oVariableNode)))
                        foreach (var binding in bindings)
                        {
                            s = binding[sVariableNode];
                            o = binding[oVariableNode];
                            if (s!=null)
                            {
                                if (o!=null)
                                {
                                    if (TestSOConnection(s, o))
                                        yield return binding;
                                }
                                else
                                {
                                    foreach (var node in  GetAllSConnections(s))
                                        yield return new SparqlResult(binding, node, oVariableNode);
                                }
                            }

                            else if (o != null)
                            {
                                foreach (var node in  GetAllOConnections(o))
                                    yield return new SparqlResult(binding, node, sVariableNode);
                            }
                            else // both unknowns
                            {
                                throw new Exception();
                            }
                        }
                    else
                    {
                        bothVariablesChache = predicatePath.CreateTriple(sVariableNode, oVariableNode, q)
                            .Aggregate(Enumerable.Repeat(new SparqlResult(q), 1),
                                (enumerable, triple) => triple.Run(enumerable))
                            .Select(r => new KeyValuePair<ObjectVariants, ObjectVariants>(r[sVariableNode], r[oVariableNode]))
                            .ToArray();

                        foreach (var binding in bindings)     
                        {
                            s = binding[sVariableNode];
                            o = binding[oVariableNode];
                            if (s!=null)
                                if (o!=null)
                                {
                                    if (TestSOConnectionFromCache(s, o))
                                        yield return binding;
                                }
                                else
                                {
                                    foreach (var node in GetAllSConnectionsFromCache(s))
                                        yield return new SparqlResult(binding, node, oVariableNode);
                                }
                            else if (o!=null)
                                foreach (var node in GetAllOConnectionsFromCache(o))
                                    yield return new SparqlResult(binding, node, sVariableNode);
                            else // both unknowns
                            {
                                if (bothVariablesCacheBySubject == null)
                                {
                                    bothVariablesCacheBySubject = new Dictionary<ObjectVariants, HashSet<ObjectVariants>>();
                                    foreach (var pair in bothVariablesChache)
                                    {
                                        HashSet<ObjectVariants> nodes;
                                        if (!bothVariablesCacheBySubject.TryGetValue(pair.Key, out nodes))
                                            bothVariablesCacheBySubject.Add(pair.Key, new HashSet<ObjectVariants>() { pair.Value });
                                        else nodes.Add(pair.Value);
                                    }
                                }

                                foreach (var sbj in bothVariablesCacheBySubject.Keys)
                                    foreach (var node in GetAllSConnectionsFromCache(sbj))
                                        yield return
                                            new SparqlResult(binding, sbj, sVariableNode, node, oVariableNode);
                            }}
                    }
                }
            }
        }

        public ObjectVariants Subject { get; private set; }
        public ObjectVariants Object { get; private set; }

        private IEnumerable<ObjectVariants> GetAllSConnections(ObjectVariants subj)
        {
            HashSet<ObjectVariants> history = new HashSet<ObjectVariants>(){subj};
              Queue<ObjectVariants> subjects =new Queue<ObjectVariants>();
                    subjects.Enqueue(subj);
            while (subjects.Count > 0)
                foreach (var objt in RunTriple(subjects.Dequeue(), oVariableNode)
                    .Select(sparqlResult => sparqlResult[oVariableNode])
                    .Where(objt =>
                    {
                        var isNewS = !history.Contains(objt);
                        if (isNewS)
                        {
                            history.Add(objt);
                            subjects.Enqueue(objt);
                        }
                        return isNewS;
                    }))
                    yield return objt;
        }

        private IEnumerable<ObjectVariants> GetAllSConnectionsFromCache(ObjectVariants subj)
        {
            if (bothVariablesCacheBySubject == null)
            {
                bothVariablesCacheBySubject = new Dictionary<ObjectVariants, HashSet<ObjectVariants>>();
                foreach (var pair in bothVariablesChache)
                {
                    HashSet<ObjectVariants> nodes;
                    if (!bothVariablesCacheBySubject.TryGetValue(pair.Key, out nodes))
                        bothVariablesCacheBySubject.Add(pair.Key, new HashSet<ObjectVariants>() { pair.Value });
                    else nodes.Add(pair.Value);
                }
            }
            HashSet<ObjectVariants> history = new HashSet<ObjectVariants>() { subj };
            Queue<ObjectVariants> subjects = new Queue<ObjectVariants>();
            subjects.Enqueue(subj);
                HashSet<ObjectVariants> objects;
            while (subjects.Count > 0)
                if(bothVariablesCacheBySubject.TryGetValue(subjects.Dequeue(), out objects))
                foreach (var objt in objects
                    .Where(objt =>
                    {
                        var isNewS = !history.Contains(objt);
                        if (isNewS)
                        {
                            history.Add(objt);
                            subjects.Enqueue(objt);
                        }
                        return isNewS;
                    }))
                    yield return objt;
        }

        private IEnumerable<ObjectVariants> GetAllOConnections(ObjectVariants objt)
        {
             HashSet<ObjectVariants> history=new HashSet<ObjectVariants>(){objt};
            Queue<ObjectVariants> objects = new Queue<ObjectVariants>();
                    objects.Enqueue(objt);                      

            while (objects.Count > 0)
                foreach (var subjt in RunTriple(sVariableNode, objects.Dequeue())
                    .Select(sparqlResult => sparqlResult[sVariableNode])
                    .Where(subjt =>
                    {
                        var isNewS = !history.Contains(subjt);
                        if (isNewS)
                        {
                            history.Add(subjt);
                            objects.Enqueue(subjt);
                        }
                        return isNewS;
                    }))
                    yield return subjt;
        }

        private IEnumerable<ObjectVariants> GetAllOConnectionsFromCache(ObjectVariants objt)
        {
            if (bothVariablesCacheByObject == null)
            {
                bothVariablesCacheByObject = new Dictionary<ObjectVariants, HashSet<ObjectVariants>>();
                foreach (var pair in bothVariablesChache)
                {
                    HashSet<ObjectVariants> nodes;
                    if (!bothVariablesCacheByObject.TryGetValue(pair.Value, out nodes))
                        bothVariablesCacheByObject.Add(pair.Value, new HashSet<ObjectVariants>() { pair.Key});
                    else nodes.Add(pair.Key);
                }
            }
            HashSet<ObjectVariants> history = new HashSet<ObjectVariants>() { objt };
            Queue<ObjectVariants> objects = new Queue<ObjectVariants>();
            objects.Enqueue(objt);

            HashSet<ObjectVariants> subjects=new HashSet<ObjectVariants>();
            while (objects.Count > 0)
                if(bothVariablesCacheByObject.TryGetValue(objects.Dequeue(), out subjects))
                foreach (var subjt in subjects
                    .Where(subjt =>
                    {
                        var isNewS = !history.Contains(subjt);
                        if (isNewS)
                        {
                            history.Add(subjt);
                            objects.Enqueue(subjt);
                        }
                        return isNewS;
                    }))
                    yield return subjt;
        }


        private bool TestSOConnection(ObjectVariants sbj, ObjectVariants objct)
        {
            HashSet<ObjectVariants> history = new HashSet<ObjectVariants>() { Subject };
            Queue<ObjectVariants> newSubjects = new Queue<ObjectVariants>();
            newSubjects.Enqueue(sbj);
            var subject = newSubjects.Peek();
            if (RunTriple(subject, objct).Any())  
                return true;
            var newVariable = (SparqlBlankNode)q.CreateBlankNode();
            while (newSubjects.Count > 0)
                if (RunTriple(newSubjects.Dequeue(), newVariable)
                    .Select(sparqlResult => sparqlResult[newVariable])
                    .Where(o => !history.Contains(o))
                    .Any(o =>
                    {
                        history.Add(o);
                        newSubjects.Enqueue(o);
                        return RunTriple(o, objct).Any();
                    }))    
                    return true;
            return false;
        }

        private bool TestSOConnectionFromCache(ObjectVariants sbj, ObjectVariants objct)
        {
            if (bothVariablesCacheBySubject==null)
            {
                bothVariablesCacheBySubject=new Dictionary<ObjectVariants, HashSet<ObjectVariants>>();
                foreach (var pair in bothVariablesChache)
            {
                HashSet<ObjectVariants> nodes;
                if (!bothVariablesCacheBySubject.TryGetValue(pair.Key, out nodes))
                    bothVariablesCacheBySubject.Add(pair.Key, new HashSet<ObjectVariants>() { pair.Value });
                else nodes.Add(pair.Value);    
            }}

            HashSet<ObjectVariants> history = new HashSet<ObjectVariants>() { Subject };
            HashSet<ObjectVariants> objects;
            if (bothVariablesCacheBySubject.TryGetValue(sbj, out objects) && objects.Contains(objct))
                return true;
            
            Queue<ObjectVariants> newSubjects = new Queue<ObjectVariants>();
            newSubjects.Enqueue(sbj);

            while (newSubjects.Count > 0)
                if (bothVariablesCacheBySubject.TryGetValue(newSubjects.Dequeue(), out objects)
                    && objects
                        .Where(o => !history.Contains(o))
                        .Any(o =>
                        {
                            history.Add(o);
                            newSubjects.Enqueue(o);
                            return bothVariablesCacheBySubject.TryGetValue(o, out objects) && objects.Contains(objct);
                        }))
                    return true;
            return false;
        }

        private IEnumerable<SparqlResult> RunTriple(ObjectVariants subject, ObjectVariants objct)
        {                                     
                return predicatePath.CreateTriple(subject, objct, q).Aggregate(Enumerable.Repeat(new SparqlResult(q), 1),
                    (enumerable, triple) => triple.Run(enumerable));
           
        }

        public new SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.PathTranslator;} }
    }
}