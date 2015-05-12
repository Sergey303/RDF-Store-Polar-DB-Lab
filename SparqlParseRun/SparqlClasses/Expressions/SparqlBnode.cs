using System;
using RDFCommon;
using RDFCommon.OVns;


namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlBnode : SparqlExpression
    {

        public SparqlBnode(SparqlExpression value, RdfQuery11Translator q)
        {
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            value.SetExprType(ObjectVariantEnum.Str);
            SetExprType(ObjectVariantEnum.Iri);
            var litConst = value.Const;
            if (litConst != null)
                Const = q.Store.NodeGenerator.CreateBlankNode((string) litConst.Content);
            else
            {
                Operator =
                    TypedOperator =
                        result =>
                        {
                            var str = value.TypedOperator(result);
                            if (str is IStringLiteralNode)
                                return q.Store.NodeGenerator.CreateBlankNode((string)str.Content);
                            throw new ArgumentException();
                        };
            }
        }

        public SparqlBnode(RdfQuery11Translator q)
        {
            SetExprType(ObjectVariantEnum.Iri);
            TypedOperator = result => q.Store.NodeGenerator.CreateBlankNode();
        }

        
    }
}
