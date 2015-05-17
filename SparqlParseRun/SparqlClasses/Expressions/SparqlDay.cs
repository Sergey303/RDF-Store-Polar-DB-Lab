using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlDay : SparqlExpression
    {
        private SparqlExpression sparqlExpression;

        public SparqlDay(SparqlExpression value)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            value.SetExprType(ExpressionTypeEnum.Date);
            SetExprType(ObjectVariantEnum.Int);
            TypedOperator = result =>
            {
                var f = value.TypedOperator(result).Content;
                if (f is DateTime)
                    return new OV_int(((DateTime)f).Day);
                if (f is DateTimeOffset)
                    return new OV_int(((DateTimeOffset)f).Day);
                throw new ArgumentException();
            };
        }
    }
}
