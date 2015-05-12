using System;
using RDFCommon;
using RDFCommon.OVns;


namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlContains : SparqlExpression
    {
        public SparqlContains(SparqlExpression str, SparqlExpression pattern)
        {
            IsAggragate = pattern.IsAggragate || str.IsAggragate;
            IsDistinct = pattern.IsDistinct || str.IsDistinct;
            SetExprType(ObjectVariantEnum.Bool);
            str.SetExprType(ExpressionTypeEnum.stringOrWithLang); str.SetExprType(ExpressionTypeEnum.stringOrWithLang);
            pattern.SetExprType(ExpressionTypeEnum.stringOrWithLang); str.SetExprType(ExpressionTypeEnum.stringOrWithLang);

            TypedOperator = result =>
            {
                var s = str.TypedOperator(result);
                var ps = pattern.TypedOperator(result);
                if ((s is OV_langstring && ps is OV_langstring) ||
                    (s is ILanguageLiteral && ps is OV_string) || (s is OV_string && ps is OV_string))
                    return new OV_bool(((string)s.Content).Contains((string)ps.Content));
                
                throw new ArgumentException();
            };
        }
    }
}
