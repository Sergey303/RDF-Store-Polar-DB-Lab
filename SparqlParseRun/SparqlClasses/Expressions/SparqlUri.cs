using System;
using RDFCommon;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlUri : SparqlExpression
    {
        public SparqlUri(SparqlExpression value, RdfQuery11Translator q)
        {
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            Func = result => new OV_iri(q.prolog.GetFromString(value.Func(result).Content)); 
        }
    }
}
