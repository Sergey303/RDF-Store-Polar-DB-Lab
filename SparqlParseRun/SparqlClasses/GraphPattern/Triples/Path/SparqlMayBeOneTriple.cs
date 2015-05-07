using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples.Path
{
    public class SparqlMayBeOneTriple:  ISparqlGraphPattern
    {
        private readonly IEnumerable<ISparqlGraphPattern> triples;
        private readonly ObjectVariants sNode;
        private readonly ObjectVariants oNode;               
        private readonly RdfQuery11Translator q;

        public SparqlMayBeOneTriple(IEnumerable<ISparqlGraphPattern> triples, ObjectVariants s, ObjectVariants o, RdfQuery11Translator q)
        {
            oNode = o;
            this.q = q;
            this.triples = triples;
            this.sNode = s;
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            var firstVar = sNode as VariableNode;
            var secondVar = oNode as VariableNode;
            SparqlVariableBinding firstVarValue;
            SparqlVariableBinding secondVarValue;
            ObjectVariants s = null, o = null;
           IEnumerable<SparqlResult> bindings = variableBindings as SparqlResult[] ?? variableBindings.ToArray();
            foreach (var variableBinding in bindings)
            {
                bool isSKnowns = true;
                bool isOKnowns = true;
                if (firstVar == null)
                {
                    s = sNode;
                }
                else if (variableBinding.row.TryGetValue(firstVar, out firstVarValue))
                {
                    s = firstVarValue.Value;
                }
                else isSKnowns = false;
                if (secondVar == null)
                {
                    o = sNode;
                }
                else if (variableBinding.row.TryGetValue(secondVar, out secondVarValue))
                {
                    o = secondVarValue.Value;
                }
                else isOKnowns = false;
                if (isSKnowns)
                {
                    if (isOKnowns)
                    {
                        if (s.Equals(o)) yield return variableBinding;
                    }  else
                     yield return new SparqlResult(variableBinding,s, secondVar);
                }
                else if (isOKnowns) yield return new SparqlResult(variableBinding, o, firstVar);
                else
                    foreach (var subjectNode in q.Store.GetAllSubjects())
                    {
                       yield return new SparqlResult(variableBinding, subjectNode, firstVar, subjectNode, secondVar);
                    }
            }
            foreach (var tr in triples.Aggregate(bindings, (current, sparqlGraphPattern) => sparqlGraphPattern.Run(current)))
                yield return tr;
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.SparqlTriple;} }
    }
}