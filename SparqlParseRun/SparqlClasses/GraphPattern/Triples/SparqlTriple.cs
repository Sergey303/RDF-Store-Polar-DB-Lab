using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
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
        protected readonly DataSet graphs;
        private bool isSKnown = true;
        private bool isPKnown = true;
        private bool isOKnown = true;
   
        protected readonly VariableNode sVariableNode;
        private readonly VariableNode pVariableNode;
        protected readonly VariableNode oVariableNode;
        private readonly VariableDataSet variableDataSet;
        protected readonly RdfQuery11Translator q;


        public SparqlTriple(ObjectVariants subj, ObjectVariants pred, ObjectVariants  obj, RdfQuery11Translator q)
        {
            this.q = q;
            Subject = (ObjectVariants) subj;
            Predicate = (ObjectVariants) pred;
            Object = (ObjectVariants) obj;
            //if(!(subj is ObjectVariants)) throw new ArgumentException();
            graphs = q.ActiveGraphs;
             //this.Graph = graph;
            sVariableNode = subj as VariableNode;
            pVariableNode = pred as VariableNode;
            oVariableNode = obj as VariableNode;
            variableDataSet = (q.ActiveGraphs as VariableDataSet);
        }


        public virtual IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            var sparqlResults = variableBindings.SelectMany(CreateBindings).ToArray();
            return sparqlResults;
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.SparqlTriple;} }

        private IEnumerable<SparqlResult> CreateBindings(SparqlResult variableBinding)
        {

            ObjectVariants sValue = null;
            ObjectVariants pValue = null;
            ObjectVariants oValue = null;
            TryGetSpoVariablesValues(variableBinding, out sValue, out pValue, out oValue);


            if (variableDataSet == null)
                if (graphs.Count == 0) return SetVariablesValues(variableBinding, sValue, pValue, oValue); //todo
                else return SetVariablesValuesFromGraphs(variableBinding, sValue, pValue, oValue, graphs);
            else
            {
                ObjectVariants varGraphValue = variableBinding[variableDataSet.Variable];
                if (varGraphValue != null)
                    return SetVariablesValuesFromGraphs(variableBinding, sValue, pValue, oValue,
                        new DataSet() {varGraphValue});
                else return SetVariablesValuesVarGraphs(variableBinding, sValue, pValue, oValue);
            }
        }

      
                                       
        private IEnumerable<SparqlResult> SetVariablesValues(SparqlResult variableBinding, ObjectVariants sValue, ObjectVariants pValue, ObjectVariants oValue)
        {
            if (!isSKnown)
                if (!isPKnown)
                    if (!isOKnown)
                        return
                            q.StoreCalls.SPO(sVariableNode, pVariableNode, oVariableNode,
                                variableBinding);
                    else
                        return
                            q.StoreCalls.SPo(sVariableNode, pVariableNode, oValue,
                                variableBinding);
                else
            if (!isOKnown)

                return
                    q.StoreCalls.SpO(sVariableNode, pValue, oVariableNode,
                        variableBinding);

                else
            return
                q.StoreCalls.Spo(sVariableNode, pValue, oValue,
                    variableBinding);
            else
            {
                if (!isPKnown)
                    if (!isOKnown)
                        return
                            q.StoreCalls.sPO(sValue, pVariableNode, oVariableNode,
                                variableBinding);
                    else
                return
                    q.StoreCalls.sPo(sValue, pVariableNode, oValue,
                        variableBinding);
                           
                else
                {
                    if (!isOKnown)
                        
                        return
                            q.StoreCalls.spO(sValue, pValue, oVariableNode,
                                variableBinding);
                    else
                        return q.StoreCalls.spo(sValue, pValue, oValue, variableBinding);
                }
            }
        }

        private IEnumerable<SparqlResult> SetVariablesValuesFromGraphs(SparqlResult variableBinding, ObjectVariants sValue, ObjectVariants pValue, ObjectVariants oValue, DataSet namedGraphs)
        {
            if (!isSKnown)
                if (!isPKnown)
                    if (!isOKnown)
                        return
                            q.StoreCalls.SPOGraphs(sVariableNode, pVariableNode, oVariableNode,
                                variableBinding, namedGraphs);
                    else
                        return
                            q.StoreCalls.SPoGraphs(sVariableNode, pVariableNode, oValue,
                                variableBinding, namedGraphs);
                else if (!isOKnown)

                    return
                        q.StoreCalls.SpOGraphs(sVariableNode, pValue, oVariableNode,
                            variableBinding, namedGraphs);

                else
                    return
                        q.StoreCalls.SpoGraphs(sVariableNode, pValue, oValue,
                            variableBinding, namedGraphs);
            else
            {
                if (!isPKnown)
                    if (!isOKnown)
                        return
                            q.StoreCalls.sPOGraphs(sValue, pVariableNode, oVariableNode,
                                variableBinding, namedGraphs);
                    else
                        return
                            q.StoreCalls.sPoGraphs(sValue, pVariableNode, oValue,
                                variableBinding, namedGraphs);
                else
                {
                    if (!isOKnown)
                        return
                            q.StoreCalls.spOGraphs(sValue, pValue, oVariableNode,
                                variableBinding, namedGraphs);

                    else
                        return q.StoreCalls.spoGraphs(sValue, pValue, oValue, variableBinding, namedGraphs);
                }
            }
        }

        private IEnumerable<SparqlResult> SetVariablesValuesVarGraphs(SparqlResult variableBinding,  ObjectVariants sValue, ObjectVariants pValue, ObjectVariants oValue)
        {
            if (!isSKnown)
                if (!isPKnown)
                    if (!isOKnown)
                        return
                            q.StoreCalls.SPOVarGraphs(sVariableNode, pVariableNode, oVariableNode,
                                variableBinding, variableDataSet);
                    else
                        return
                            q.StoreCalls.SPoVarGraphs(sVariableNode, pVariableNode, oValue,
                                variableBinding, variableDataSet);
                else if (!isOKnown)

                    return
                        q.StoreCalls.SpOVarGraphs(sVariableNode, pValue, oVariableNode,
                            variableBinding, variableDataSet);

                else
                    return
                        q.StoreCalls.SpoVarGraphs(sVariableNode, pValue, oValue,
                            variableBinding, variableDataSet);
            else
            {
                if (!isPKnown)
                    if (!isOKnown)
                        return
                            q.StoreCalls.sPOVarGraphs(sValue, pVariableNode, oVariableNode,
                                variableBinding, variableDataSet);
                    else
                        return
                            q.StoreCalls.sPoVarGraphs(sValue, pVariableNode, oValue,
                                variableBinding, variableDataSet);
                else
                {
                    if (!isOKnown)
                        return
                            q.StoreCalls.spOVarGraphs(sValue, pValue, oVariableNode,
                                variableBinding, variableDataSet);
                    else
                    {
                        return
                            q.StoreCalls.spoVarGraphs(sValue, pValue, oValue, variableBinding, variableDataSet);
                    }
                }
            }
        }

        private void TryGetSpoVariablesValues(SparqlResult variableBinding, out ObjectVariants sValue,
            out ObjectVariants pValue,
            out ObjectVariants oValue)
        {   
            sValue = sVariableNode != null ? variableBinding[sVariableNode] : Subject;
            pValue = pVariableNode != null ? variableBinding[pVariableNode] : Predicate;   
            oValue = oVariableNode != null ? variableBinding[oVariableNode] : Object;

            isSKnown = sValue != null;
            isPKnown = pValue != null;
            isOKnown = oValue != null;
        }

        public void Substitution(SparqlResult variableBinding, Action<ObjectVariants, ObjectVariants, ObjectVariants> actTriple, string name = null)
        {                          
           ObjectVariants sValue;
           ObjectVariants pValue;
           ObjectVariants oValue;
           TryGetSpoVariablesValues(variableBinding, out sValue, out pValue, out oValue);
           if (!isSKnown && sVariableNode is IBlankNode) sValue =q.Store.NodeGenerator.CreateBlankNode((sVariableNode).Content, name);
           if (!isOKnown && oVariableNode is IBlankNode) oValue = q.Store.NodeGenerator.CreateBlankNode((oVariableNode).Content, name);
          // if (!isPKnown && pVariableNode is IBlankNode) pValue = ((SparqlBlankNode)pVariableNode).RdfBlankNode;
          // if ((isSKnown || sVariableNode is SparqlBlankNode) && (isPKnown || pVariableNode is SparqlBlankNode) && (isOKnown || oVariableNode is SparqlBlankNode))
       actTriple(sValue, pValue, oValue);
         //  throw new Exception();
        }

        public void Substitution(SparqlResult variableBinding, ObjectVariants g,
            Action<ObjectVariants, ObjectVariants, ObjectVariants, ObjectVariants> actQuard)
        {                 
            ObjectVariants sValue;
            ObjectVariants pValue;
            ObjectVariants oValue;

            TryGetSpoVariablesValues(variableBinding, out sValue, out pValue, out oValue);
                 
            if (!isSKnown && sVariableNode is IBlankNode) 
                sValue = q.Store.NodeGenerator.CreateBlankNode((sVariableNode).Content, ((IIriNode)g).UriString );
            if (!isOKnown && oVariableNode is IBlankNode) 
                oValue = q.Store.NodeGenerator.CreateBlankNode((oVariableNode).Content, ((IIriNode)g).UriString);
            
            actQuard(g, sValue, pValue, oValue);  
        }

        public void Substitution(SparqlResult variableBinding, VariableNode gVariableNode,
            Action<ObjectVariants, ObjectVariants, ObjectVariants, ObjectVariants> actQuard)
        {
            ObjectVariants g;
            g = variableBinding[pVariableNode];
            if (g == null)
                throw new Exception("graph hasn't value");

            Substitution(variableBinding, g, actQuard);
        }

    }
}