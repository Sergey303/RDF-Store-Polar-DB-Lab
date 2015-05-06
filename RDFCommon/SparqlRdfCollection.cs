using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;

namespace RDFCommon
{
    public class SparqlRdfCollection    
    {
        public List<IAllNode> nodes = new List<IAllNode>();

        public ObjectVariants GetNode(Action<ObjectVariants, ObjectVariants, ObjectVariants> addTriple, INodeGenerator q)
        {


            ObjectVariants sparqlBlankNodeFirst = q.CreateBlankNode();
                ObjectVariants sparqlBlankNodeNext = q.CreateBlankNode();
            foreach (var node in nodes.Take(nodes.Count - 1))
            {
                addTriple(sparqlBlankNodeNext, q.SpecialTypes.first, (ObjectVariants)node);
                addTriple(sparqlBlankNodeNext, q.SpecialTypes.rest, sparqlBlankNodeNext = q.CreateBlankNode());
            }
            addTriple(sparqlBlankNodeNext, q.SpecialTypes.first, (ObjectVariants)nodes[nodes.Count - 1]);
            addTriple(sparqlBlankNodeNext, q.SpecialTypes.rest, q.SpecialTypes.nil);
            return sparqlBlankNodeFirst;
        }
    }
}