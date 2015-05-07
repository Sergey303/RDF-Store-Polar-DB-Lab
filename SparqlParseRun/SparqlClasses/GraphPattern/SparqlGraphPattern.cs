using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Path;

namespace SparqlParseRun.SparqlClasses.GraphPattern
{
    public class SparqlGraphPattern : SparqlQuardsPattern
    {

        internal void CreateTriple( ObjectVariants subj, ObjectVariants predicate, ObjectVariants obj, RdfQuery11Translator q)
        {
            if(predicate is SparqlPathTranslator)
                AddRange(((SparqlPathTranslator)predicate).CreateTriple(subj, obj, q));
            else
            Add(new SparqlTriple(subj, predicate, obj, q));
        }
        
    }
}

