using RDFCommon;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlToString : SparqlExpression
    {
       // private SparqlExpression sparqlExpression;

        public SparqlToString(SparqlExpression child, NodeGenerator q)
           
        {
           IsAggragate = child.IsAggragate;
            IsDistinct = child.IsDistinct;
        
            var childConst = child.Const;
            if (childConst != null) Const =new OV_string(childConst.Content.ToString());
            else
            {
                Operator = result => child.Operator(result).ToString();
                                TypedOperator = result => new OV_string(child.Operator(result).ToString());                
            
            }
        }
    }
}
