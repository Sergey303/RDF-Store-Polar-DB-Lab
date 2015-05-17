using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlOrExpression : SparqlExpression
    {
        public SparqlOrExpression(SparqlExpression l, SparqlExpression r)
            
        {
            IsAggragate = l.IsAggragate || r.IsAggragate;
            IsDistinct = l.IsDistinct || r.IsDistinct;
            l.SetExprType(ObjectVariantEnum.Bool);
            r.SetExprType(ObjectVariantEnum.Bool);
            SetExprType(ObjectVariantEnum.Bool);
            TypedOperator = result => l.TypedOperator(result).Change(ll => ll || r.TypedOperator(result).Content);
        }

        public static SparqlExpression Create(SparqlExpression l, SparqlExpression r)
        {
            return new SparqlOrExpression(l,r);
        }
    }
}
