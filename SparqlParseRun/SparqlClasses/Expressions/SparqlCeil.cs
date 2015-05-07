using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlCeil : SparqlExpression
    {
        public SparqlCeil(SparqlExpression value)
        {
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            value.SetVariablesTypes(ExpressionType.numeric);
            SetVariablesTypes(ExpressionType.numeric);
            Func = result =>
            {
                var val = value.Func(result);
                switch (val.Variant)
                {
                    case ObjectVariantEnum.Decimal:
                        return new OV_decimal( Math.Ceiling(val.Content));
                        case ObjectVariantEnum.Double:
                        return new OV_double(Math.Ceiling(val.Content));
                        case ObjectVariantEnum.Float:
                        return new OV_float(Math.Ceiling(val.Content));
                        case ObjectVariantEnum.Int:
                        return val;

                }
             
                throw new ArgumentException("Ceil " + val);
            };
        }
    }
}