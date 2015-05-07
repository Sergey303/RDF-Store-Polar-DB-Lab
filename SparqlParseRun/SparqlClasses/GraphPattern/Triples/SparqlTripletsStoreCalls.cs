using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples
{
    public  class SparqlTripletsStoreCalls : ISparqlTripletsStoreCalls
    {
        private IStore store;

        public SparqlTripletsStoreCalls(IStore store)
        {
            this.store = store;
        }

        public IEnumerable<SparqlResult> spO(ObjectVariants subjNode, ObjectVariants predicateNode,
            VariableNode obj, SparqlResult variablesBindings)
        {
                return store
                    .GetTriplesWithSubjectPredicate(subjNode, predicateNode)
                    .Select(node => new SparqlResult(variablesBindings, node, obj));
        }

        // from merged named graphs
        public IEnumerable<SparqlResult> spOVarGraphs(ObjectVariants subjNode, ObjectVariants predicateNode,
            VariableNode obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            if(variableDataSet.Any())           
                return  variableDataSet.SelectMany(g => store
                .NamedGraphs
                .GetObject(subjNode, predicateNode, g)
                .Select(o => new SparqlResult(variablesBindings, o, obj, g, variableDataSet.Variable)));
            else return store
                .NamedGraphs
                .GetTriplesWithSubjectPredicate(subjNode, predicateNode,
                    ((o, g) => new SparqlResult(variablesBindings, o, obj, g, variableDataSet.Variable)));
                // if graphVariable is null, ctor check this.
        }

        public IEnumerable<SparqlResult> spOGraphs( ObjectVariants subjNode, ObjectVariants predicateNode,
            VariableNode obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(graph => store
                .NamedGraphs
                .GetObject(subjNode, predicateNode, graph) // if graphs is empty, Gets All named graphs
                .Select(o => new SparqlResult(variablesBindings, o, obj)));
        }


        public IEnumerable<SparqlResult> Spo( VariableNode subj, ObjectVariants predicateNode, ObjectVariants objectNode, SparqlResult variablesBindings)
        {
            return store
                .GetTriplesWithPredicateObject(predicateNode, objectNode)
                .Select(node => new SparqlResult(variablesBindings, node, subj));

            // from merged named graphs
        }


        public IEnumerable<SparqlResult> SpoGraphs( VariableNode subj, ObjectVariants predicateNode,
            ObjectVariants objectNode, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(g=>
                 store.NamedGraphs
                .GetSubject(predicateNode, objectNode, g)
                // if graphs is empty, Gets All named graphs
                .Select(node =>new SparqlResult(variablesBindings, node, subj)));
                // if graphVariable is null, ctor check this.
        }

        public IEnumerable<SparqlResult> SpoVarGraphs( VariableNode subj, ObjectVariants predicateNode,
            ObjectVariants objectNode, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            if (variableDataSet.Any())
                return variableDataSet.SelectMany(g => store
                .NamedGraphs
                .GetSubject(predicateNode, objectNode, g)
                .Select(s => new SparqlResult(variablesBindings, s, subj, g, variableDataSet.Variable)));
            else 
            return store
                .NamedGraphs
                .GetTriplesWithPredicateObject(predicateNode, objectNode,
                    (s, g) => new SparqlResult(variablesBindings, s, subj, g, variableDataSet.Variable));
        }

        public IEnumerable<SparqlResult> SpO( VariableNode sVar, ObjectVariants predicate, VariableNode oVar, SparqlResult variablesBindings)
        {
            return store
                .GetTriplesWithPredicate(predicate, (s,o)=>
                new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
                {
                    {sVar, new SparqlVariableBinding(sVar,s)},
                    {oVar, new SparqlVariableBinding(oVar,o)}
                }));
        }

        public IEnumerable<SparqlResult> SpOGraphs(VariableNode sVar, ObjectVariants predicate, VariableNode oVar, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(graph =>
                store
                 .NamedGraphs
            .GetTriplesWithPredicateFromGraph(predicate, graph,
            (s,o) =>new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {                                  
                  {sVar, new SparqlVariableBinding(sVar, s)},
                  {oVar, new SparqlVariableBinding(oVar, o)}
              })));
        }

        public IEnumerable<SparqlResult> SpOVarGraphs(VariableNode sVar, ObjectVariants predicate, VariableNode oVar, SparqlResult variablesBindings, VariableDataSet graphs)
        {
            if (graphs.Any())
                return
                    graphs.SelectMany(g =>
                        store.NamedGraphs.GetTriplesWithPredicateFromGraph(predicate, g, (s, o) =>
                            new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
                            {
                                {sVar, new SparqlVariableBinding(sVar, s)},
                                {oVar, new SparqlVariableBinding(oVar, o)},
                                {graphs.Variable, new SparqlVariableBinding(graphs.Variable, g)}
                            })));
            else
                return store.NamedGraphs.GetTriplesWithPredicate(predicate, (s, o, g) =>
                    new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
                    {
                        {sVar, new SparqlVariableBinding(sVar, s)},
                        {oVar, new SparqlVariableBinding(oVar, o)},
                        {graphs.Variable, new SparqlVariableBinding(graphs.Variable, g)}
                    }));
        }


        public IEnumerable<SparqlResult> sPo( ObjectVariants subj, VariableNode pred, ObjectVariants obj, SparqlResult variablesBindings)
        {
            return store
                .GetTriplesWithSubjectObject(subj, obj)
                .Select(newObj => new SparqlResult(variablesBindings, newObj, pred));

        }

        public IEnumerable<SparqlResult> sPoGraphs(ObjectVariants subj, VariableNode pred, ObjectVariants obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(graph =>
                store
                    .NamedGraphs
                    .GetPredicate(subj, obj, graph)
                    .Select(p => new SparqlResult(variablesBindings, p, pred)));
        }

        public IEnumerable<SparqlResult> sPoVarGraphs( ObjectVariants subj, VariableNode pred, ObjectVariants obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            if (variableDataSet.Any())
                return variableDataSet.SelectMany(g =>
                    store.NamedGraphs.GetPredicate(subj, obj, g)
                        .Select(p => new SparqlResult(variablesBindings, p, pred, g, variableDataSet.Variable)));
            else   return store
                .NamedGraphs
                .GetTriplesWithSubjectObject(subj, obj, (p, g) =>
                    new SparqlResult(variablesBindings, p, pred, g, variableDataSet.Variable));
        }


        public IEnumerable<SparqlResult> sPO( ObjectVariants subj, VariableNode pred, VariableNode obj, SparqlResult variablesBindings)
        {
            return store
              .GetTriplesWithSubject(subj, (p,o) => 
                  new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {pred, new SparqlVariableBinding(pred,p)},
                  {obj, new SparqlVariableBinding(obj,o)}
              }));
        }

        public IEnumerable<SparqlResult> sPOGraphs( ObjectVariants subj, VariableNode pred,
            VariableNode obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(g=> store
                 .NamedGraphs
              .GetTriplesWithSubjectFromGraph(subj, g, (p,o)=>
              new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {pred, new SparqlVariableBinding(pred, p)},
                  {obj, new SparqlVariableBinding(obj, o)}
              })));
        }

        public IEnumerable<SparqlResult> sPOVarGraphs( ObjectVariants subj, VariableNode pred,
           VariableNode obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            if (variableDataSet.Any())
                return variableDataSet.SelectMany(g =>
                    store
                        .NamedGraphs
                        .GetTriplesWithSubjectFromGraph(subj, g, (p, o)=>
                        new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
                        {
                            {pred, new SparqlVariableBinding(pred, p)},
                            {obj, new SparqlVariableBinding(obj, o)},
                            {variableDataSet.Variable, new SparqlVariableBinding(variableDataSet.Variable, g)},
                        })));
            else
                return store
                    .NamedGraphs
                    .GetTriplesWithSubject(subj, (p, o, g) =>
                        new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
                        {
                            {pred, new SparqlVariableBinding(pred, p)},
                            {obj, new SparqlVariableBinding(obj, o)},
                            {variableDataSet.Variable, new SparqlVariableBinding(variableDataSet.Variable, g)},
                        }));
        }

        public IEnumerable<SparqlResult> SPo( VariableNode subj, VariableNode predicate, ObjectVariants obj, SparqlResult variablesBindings)
        {
            return store
                .GetTriplesWithObject(obj, (s, p) =>
                    new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
                    {
                        {predicate, new SparqlVariableBinding(predicate, p)},
                        {subj, new SparqlVariableBinding(subj, s)} 
                    }));
        }

        public IEnumerable<SparqlResult> SPoGraphs( VariableNode subj, VariableNode pred,
    ObjectVariants obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(g=>
                store
                 .NamedGraphs
            .GetTriplesWithObjectFromGraph(obj, g, (s,p)=>
             new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {pred, new SparqlVariableBinding(pred,p)},
                  {subj, new SparqlVariableBinding(subj,s)}
              })));
        }

        public IEnumerable<SparqlResult> SPoVarGraphs( VariableNode subj, VariableNode pred,
           ObjectVariants obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            if (variableDataSet.Any())
                return variableDataSet.SelectMany(g =>
                    store
                        .NamedGraphs
                        .GetTriplesWithObjectFromGraph(obj, g, (s, p) =>
                            new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
                            {
                                {pred, new SparqlVariableBinding(pred, p)},
                                {subj, new SparqlVariableBinding(subj, s)},
                                {
                                    variableDataSet.Variable,
                                    new SparqlVariableBinding(variableDataSet.Variable, g)
                                },
                            })));
            else
            return store
                 .NamedGraphs
            .GetTriplesWithObject(obj, (s, p, g)=>
                 new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {pred, new SparqlVariableBinding(pred, p)},
                  {subj, new SparqlVariableBinding(subj, s)},
                  {variableDataSet.Variable, new SparqlVariableBinding(variableDataSet.Variable, g)},
              }));
        }


        public IEnumerable<SparqlResult> SPO( VariableNode subj, VariableNode predicate, VariableNode obj, SparqlResult variablesBindings)
        {
            return store
             .GetTriples((s,p,o)
                 => new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {subj, new SparqlVariableBinding(subj,s)} ,
                  {predicate, new SparqlVariableBinding(predicate,p)},
                  {obj, new SparqlVariableBinding(obj, o)}
              }));

        }

        public IEnumerable<SparqlResult> SPOGraphs(VariableNode subj, VariableNode predicate, VariableNode obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(g =>
                store.NamedGraphs
                    .GetTriplesFromGraph(g,(s, p, o) =>
                        new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
                        {
                            {subj, new SparqlVariableBinding(subj, s)},
                            {predicate, new SparqlVariableBinding(predicate, p)},
                            {obj, new SparqlVariableBinding(obj, o)}
                        })));

        }

        public IEnumerable<SparqlResult> SPOVarGraphs( VariableNode subj, VariableNode predicate, VariableNode obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            if (variableDataSet.Any())
                return variableDataSet.SelectMany(g =>
                store
                 .NamedGraphs
             .GetTriplesFromGraph(g, (s, p, o)=>
             new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {subj, new SparqlVariableBinding(subj,s)} ,
                  {predicate, new SparqlVariableBinding(predicate, p)},
                  {obj, new SparqlVariableBinding(obj, o)} ,
                  {variableDataSet.Variable, new SparqlVariableBinding(variableDataSet.Variable, g)}
              })));
            return store
                 .NamedGraphs
             .GetAll((s, p, o, g)=>
                new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {subj, new SparqlVariableBinding(subj, s)} ,
                  {predicate, new SparqlVariableBinding(predicate, p)},
                  {obj, new SparqlVariableBinding(obj, o)},
                  {variableDataSet.Variable, new SparqlVariableBinding(variableDataSet.Variable, g)}
              }));

        }

        public IEnumerable<SparqlResult> spoGraphs(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants objectNode, SparqlResult variablesBindings,
            DataSet graphs)
        {                                                                       
            if (graphs.Any(g=> store.NamedGraphs.Contains(subjectNode, predicateNode, objectNode, g)))
              yield return  variablesBindings;
        }

    

        public IEnumerable<SparqlResult> spoVarGraphs(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants objectNode,
            SparqlResult variablesBindings, VariableDataSet graphs)
        {
            return (graphs.Any()
                ? graphs.Where(g => store.NamedGraphs.Contains(subjectNode, predicateNode, objectNode, g))
                : store.NamedGraphs.GetGraph(subjectNode, predicateNode, objectNode))
                    .Select(g => new SparqlResult(variablesBindings, g, graphs.Variable));
        }

        public IEnumerable<SparqlResult> spo(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants objNode, SparqlResult variablesBindings)
        {
            if (store.Contains(subjectNode, predicateNode, objNode))
                yield return variablesBindings;
        }
    }
}