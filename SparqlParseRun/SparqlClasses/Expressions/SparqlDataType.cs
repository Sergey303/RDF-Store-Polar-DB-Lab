using System;
using RDFCommon;
using RDFCommon.OVns;


namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlDataType : SparqlExpression
    {
        public SparqlDataType(SparqlExpression value, NodeGenerator q)
        {
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            SetExprType(ObjectVariantEnum.Iri);
         value.SetExprType(ExpressionTypeEnum.literal);
            TypedOperator = result =>
            {
                var r = value.TypedOperator(result);
                var literalNode = r as ILiteralNode;
                if (literalNode != null)
                    return new OV_iri(literalNode.DataType);
                throw new ArgumentException();
            };
        }
    }
}