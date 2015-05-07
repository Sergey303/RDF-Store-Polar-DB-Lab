using RDFCommon;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlToString : SparqlExpression
    {
       // private SparqlExpression sparqlExpression;

        public SparqlToString(SparqlExpression value, INodeGenerator q)
        {
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;

            Func = result => new OV_string(value.Func(result).Content.ToString());
        }
    }
}
