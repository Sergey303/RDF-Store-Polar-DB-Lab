using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples
{
    public class SparqlTriple : ISparqlGraphPattern
    {
        public ObjectVariants Subject { get; private set; }
        public ObjectVariants Predicate { get; private set; }
        public ObjectVariants Object { get; private set; }
        private DataSet graphs;
        private bool isGKnown;

        private readonly VariableNode sVariableNode;
        private readonly VariableNode pVariableNode;
        private readonly VariableNode oVariableNode;
        private readonly VariableDataSet variableDataSet;
        private readonly RdfQuery11Translator q;
        private readonly bool isDefaultGraph;


        public SparqlTriple(ObjectVariants subj, ObjectVariants pred, ObjectVariants obj, RdfQuery11Translator q)
        {
            this.q = q;
            Subject = subj;
            Predicate = pred;
            Object = obj;
            //if(!(subj is ObjectVariants)) throw new ArgumentException();
            graphs = q.ActiveGraphs;
            //this.Graph = graph;
            sVariableNode = subj as VariableNode;
            pVariableNode = pred as VariableNode;
            oVariableNode = obj as VariableNode;
            variableDataSet = (q.ActiveGraphs as VariableDataSet);
            isDefaultGraph = variableDataSet == null && graphs.Count == 0;

        }


        public virtual IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        // var backup = result.BackupMask();
        {
            return variableBindings.SelectMany(CreateBindings);
            //  result.Restore(backup);
        }

        public SparqlGraphPatternType PatternType { get { return SparqlGraphPatternType.SparqlTriple; } }

        private IEnumerable<SparqlResult> CreateBindings(SparqlResult variableBinding)
        {
            Subject = sVariableNode != null ? variableBinding[sVariableNode] : Subject;
            Predicate = pVariableNode != null ? variableBinding[pVariableNode] : Predicate;
            Object = oVariableNode != null ? variableBinding[oVariableNode] : Object;

            if (!isDefaultGraph && variableDataSet != null)
            {
                var graphFromVar = variableBinding[variableDataSet.Variable];
                graphs = graphFromVar != null ? new DataSet() { graphFromVar } : null;
                isGKnown = graphs != null;
            }
            else isGKnown = true;

            int @case = ((Subject != null ? 0 : 1) << 2) | ((Predicate != null ? 0 : 1) << 1) | (Object != null ? 0 : 1);
            if (!isDefaultGraph)
                @case |= 1 << (isGKnown ? 3 : 4);
            return ClearNewValues(Subject != null, Predicate!=null, Object!=null, isGKnown, variableBinding,
                SetVariablesValues(variableBinding, (StoreCallCase)@case));

        }

        private IEnumerable<SparqlResult> ClearNewValues(bool clearSubject, bool clearPredicate, bool clearObject, bool clearGraph, SparqlResult sourceResult, IEnumerable<SparqlResult> sparqlResults)
        {
            foreach (var result in sparqlResults)
                yield return result;
            if (clearSubject)
                sourceResult[sVariableNode] = null;
            if (clearPredicate)
                sourceResult[pVariableNode] = null;
            if (clearObject)
                sourceResult[oVariableNode] = null;
            if (clearGraph)
                sourceResult[variableDataSet.Variable] = null;

        }

        private enum StoreCallCase
        {
            spo = 0, spO = 1, sPo = 2, sPO = 3, Spo = 4, SpO = 5, SPo = 6, SPO = 7,
            gspo = 8, gspO = 9, gsPo = 10, gsPO = 11, gSpo = 12, gSpO = 13, gSPo = 14, gSPO = 15,
            Gspo = 16, GspO = 17, GsPo = 18, GsPO = 19, GSpo = 20, GSpO = 21, GSPo = 22, GSPO = 23,
        }


        private IEnumerable<SparqlResult> SetVariablesValues(SparqlResult variableBinding, StoreCallCase @case)
        {
            switch (@case)
            {
                case StoreCallCase.spo:
                    return q.StoreCalls.spo(Subject, Predicate, Object, variableBinding);
                case StoreCallCase.spO:
                    return q.StoreCalls.spO(Subject, Predicate, oVariableNode, variableBinding);
                case StoreCallCase.sPo:
                    return q.StoreCalls.sPo(Subject, pVariableNode, Object, variableBinding);
                    
                case StoreCallCase.sPO:
                    return q.StoreCalls.sPO(Subject, pVariableNode, oVariableNode, variableBinding);
                    
                case StoreCallCase.Spo:
                    return q.StoreCalls.Spo(sVariableNode, Predicate, Object, variableBinding);
                    
                case StoreCallCase.SpO:
                    return q.StoreCalls.SpO(sVariableNode, Predicate, oVariableNode, variableBinding);
                    
                case StoreCallCase.SPo:
                    return q.StoreCalls.SPo(sVariableNode, pVariableNode, Object, variableBinding);
                    
                case StoreCallCase.SPO:
                    return q.StoreCalls.SPO(sVariableNode, pVariableNode, oVariableNode, variableBinding);
                    

                case StoreCallCase.gspo:
                    return q.StoreCalls.spoGraphs(Subject, Predicate, Object, variableBinding, graphs); 
                case StoreCallCase.gspO:
                    return q.StoreCalls.spOGraphs(Subject, Predicate, oVariableNode, variableBinding, graphs); 
                case StoreCallCase.gsPo:
                    return q.StoreCalls.sPoGraphs(Subject, pVariableNode, Object, variableBinding, graphs); 
                case StoreCallCase.gsPO:
                    return q.StoreCalls.sPOGraphs(Subject, pVariableNode, oVariableNode, variableBinding, graphs); 
                case StoreCallCase.gSpo:
                    return q.StoreCalls.SpoGraphs(sVariableNode, Predicate, Object, variableBinding, graphs); 
                case StoreCallCase.gSpO:
                    return q.StoreCalls.SpOGraphs(sVariableNode, Predicate, oVariableNode, variableBinding, graphs); 
                case StoreCallCase.gSPo:
                    return q.StoreCalls.SPoGraphs(sVariableNode, pVariableNode, Object, variableBinding, graphs); 
                case StoreCallCase.gSPO:
                    return q.StoreCalls.SPOGraphs(sVariableNode, pVariableNode, oVariableNode, variableBinding, graphs); 

                case StoreCallCase.Gspo:
                    return q.StoreCalls.spoVarGraphs(Subject, Predicate, Object, variableBinding, variableDataSet); 
                case StoreCallCase.GspO:
                    return q.StoreCalls.spOVarGraphs(Subject, Predicate, oVariableNode, variableBinding, variableDataSet); 
                case StoreCallCase.GsPo:
                    return q.StoreCalls.sPoVarGraphs(Subject, pVariableNode, Object, variableBinding, variableDataSet); 
                case StoreCallCase.GsPO:
                    return q.StoreCalls.sPOVarGraphs(Subject, pVariableNode, oVariableNode, variableBinding, variableDataSet); 
                case StoreCallCase.GSpo:
                    return q.StoreCalls.SpoVarGraphs(sVariableNode, Predicate, Object, variableBinding, variableDataSet); 
                case StoreCallCase.GSpO:
                    return q.StoreCalls.SpOVarGraphs(sVariableNode, Predicate, oVariableNode, variableBinding, variableDataSet); 
                case StoreCallCase.GSPo:
                    return q.StoreCalls.SPoVarGraphs(sVariableNode, pVariableNode, Object, variableBinding, variableDataSet); 
                case StoreCallCase.GSPO:
                    return q.StoreCalls.SPOVarGraphs(sVariableNode, pVariableNode, oVariableNode, variableBinding, variableDataSet); 
                default:
                    throw new ArgumentOutOfRangeException("case");
            }
        }

        public void Substitution(SparqlResult variableBinding,
            Action<ObjectVariants, ObjectVariants, ObjectVariants> actTriple, string name = null)
        {
            var subject = sVariableNode is IBlankNode
                 ? q.Store.NodeGenerator.CreateBlankNode((string)sVariableNode.Content, name)
                 : (sVariableNode != null ? variableBinding[sVariableNode] : Subject);

            var predicate = pVariableNode != null ? variableBinding[pVariableNode] : Predicate;

            var @object = oVariableNode is IBlankNode
                 ? q.Store.NodeGenerator.CreateBlankNode((string)oVariableNode.Content, name)
                 : (oVariableNode != null ? variableBinding[oVariableNode] : Object);
            actTriple(subject, predicate, @object);
        }

        public void Substitution(SparqlResult variableBinding, ObjectVariants g,
            Action<ObjectVariants, ObjectVariants, ObjectVariants, ObjectVariants> actQuard)
        {
            var subject = sVariableNode is IBlankNode
                ? q.Store.NodeGenerator.CreateBlankNode((string)(sVariableNode).Content, ((IIriNode)g).UriString)
                : (sVariableNode != null ? variableBinding[sVariableNode] : Subject);
            var predicate = pVariableNode != null ? variableBinding[pVariableNode] : Predicate;
            var @object = Object == null && oVariableNode is IBlankNode
                  ? q.Store.NodeGenerator.CreateBlankNode((string)(oVariableNode).Content, ((IIriNode)g).UriString)
                  : (oVariableNode != null ? variableBinding[oVariableNode] : Object);

            actQuard(g, subject, predicate, @object);
        }

        public void Substitution(SparqlResult variableBinding, VariableNode gVariableNode,
            Action<ObjectVariants, ObjectVariants, ObjectVariants, ObjectVariants> actQuard)
        {
            ObjectVariants g;
            g = variableBinding[gVariableNode];
            if (g == null)
                throw new Exception("graph hasn't value");

            Substitution(variableBinding, g, actQuard);
        }

    }
}