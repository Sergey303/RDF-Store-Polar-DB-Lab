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
            SetExprType(ObjectVariantEnum.Bool);

            TypedOperator = result =>
            {
                var func = value.TypedOperator(result);
                return new OV_bool(func is ILiteralNode); 
            };
        }
    }
}
