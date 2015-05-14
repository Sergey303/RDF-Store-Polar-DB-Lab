using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlIri : SparqlExpression
    {
        public SparqlIri(SparqlExpression value, NodeGenerator q)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
         Func = result =>
            {
                var f = value.Func(result);
                if (f is IIriNode)
                    return f;
                if(f is ILiteralNode)      //TODO
                    return new OV_iri(f.Content);
                throw new ArgumentException();  
            }; 
        }
    }
}
