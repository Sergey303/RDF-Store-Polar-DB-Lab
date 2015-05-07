using System;
using RDFCommon;
using RDFCommon.OVns;


namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlDataType : SparqlExpression
    {
        public SparqlDataType(SparqlExpression value, INodeGenerator q)
        {
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
         SetVariablesTypes(ExpressionType.iri);
         value.SetVariablesTypes(ExpressionType.literal);
            Func = result =>
            {
                var r = value.Func(result);
                var literalNode = r as ILiteralNode;
                if (literalNode != null)
                    return new OV_iri(literalNode.DataType);
                throw new ArgumentException();
            };
        }
    }
}