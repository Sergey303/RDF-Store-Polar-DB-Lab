using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlAndExpression : SparqlBinaryExpression
    {
      


        public SparqlAndExpression(SparqlExpression l, SparqlExpression r)
            : base(l, r, (o, o1) => (bool)o && (bool)o1)
        {      
            l.SetExprType(ObjectVariantEnum.Bool); 
            r.SetExprType(ObjectVariantEnum.Bool);
            SetExprType(ObjectVariantEnum.Bool);        
        }

     
    }
    /// <summary>
    /// Nullable Pair
    /// </summary>
    public  enum NP{ bothNull, leftNull, rigthNull, bothNotNull}

    public static class NullablePairExt
{
        public static NP Get(object left, object right)
        {
            return (NP)(((left != null ? 1 : 0) << 1) | (right != null ? 1 : 0));
        }
}
}
