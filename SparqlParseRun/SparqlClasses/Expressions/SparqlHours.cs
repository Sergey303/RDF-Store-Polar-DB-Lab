using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlHours : SparqlExpression
    {
        public SparqlHours(SparqlExpression value)
        {
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            value.SetExprType(ExpressionTypeEnum.Date);
            SetExprType(ObjectVariantEnum.Int);
            TypedOperator = result =>
            {
                var f = value.TypedOperator(result).Content;
                if (f is DateTimeOffset)
                    return new OV_int(((DateTimeOffset)f).Hour);
                throw new ArgumentException();
            };
        }
    }
}
