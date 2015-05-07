using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlIsLiteral : SparqlExpression
    {
        public SparqlIsLiteral(SparqlExpression value)
        {
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            SetVariablesTypes(ExpressionType.@bool);

            Func = result =>
            {
                var func = value.Func(result);
                return new OV_bool(func is ILiteralNode); 
            };
        }
    }
}
