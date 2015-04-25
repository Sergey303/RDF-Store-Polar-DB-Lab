using System;
using System.Collections.Generic;
using System.Linq;

namespace RDFCommon
{
    public class SparqlRdfCollection    
    {
        public List<IAllNode> nodes = new List<IAllNode>();

        public IBlankNode GetNode(Action<Triple<ISubjectNode, IPredicateNode, IObjectNode>> addTriple, INodeGenerator q, Func<IBlankNode> createBlank)
        {   
        
           
                IBlankNode sparqlBlankNodeFirst = createBlank();
                IBlankNode sparqlBlankNodeNext = createBlank();
            foreach (var node in nodes.Take(nodes.Count - 1))
            {
                addTriple(new Triple<ISubjectNode, IPredicateNode, IObjectNode>(sparqlBlankNodeNext, q.SpecialTypes.first, (IObjectNode) node));
                addTriple(new Triple<ISubjectNode, IPredicateNode, IObjectNode>(sparqlBlankNodeNext, q.SpecialTypes.rest, sparqlBlankNodeNext = createBlank()));
            }
            addTriple(new Triple<ISubjectNode, IPredicateNode, IObjectNode>(sparqlBlankNodeNext, q.SpecialTypes.first, (IObjectNode)nodes[nodes.Count - 1]));
            addTriple(new Triple<ISubjectNode, IPredicateNode, IObjectNode>(sparqlBlankNodeNext, q.SpecialTypes.rest, q.SpecialTypes.nil));
            return sparqlBlankNodeFirst;
        }
    }
}