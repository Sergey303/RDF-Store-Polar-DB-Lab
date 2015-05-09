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
        
            ObjectVariants s = null, o = null;
           IEnumerable<SparqlResult> bindings = variableBindings as SparqlResult[] ?? variableBindings.ToArray();
            foreach (var variableBinding in bindings)
            {

                s = firstVar == null ? sNode : variableBinding[firstVar];
                o = secondVar == null ? oNode : variableBinding[secondVar]; 
                bool isSKnowns = s!=null;
                bool isOKnowns = o!=null;
                if (isSKnowns)
                {
                    if (isOKnowns)
                    {
                        if (s.Equals(o)) yield return variableBinding;
                    }  else
                     yield return variableBinding.Add(s, secondVar);
                }
                else if (isOKnowns) yield return variableBinding.Add(o, firstVar);
                else
                    foreach (var subjectNode in q.Store.GetAllSubjects())
                    {
                       yield return variableBinding.Add(subjectNode, firstVar, subjectNode, secondVar);
                    }
            }
            foreach (var tr in triples.Aggregate(bindings, (current, sparqlGraphPattern) => sparqlGraphPattern.Run(current)))
                yield return tr;
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.SparqlTriple;} }
    }
}