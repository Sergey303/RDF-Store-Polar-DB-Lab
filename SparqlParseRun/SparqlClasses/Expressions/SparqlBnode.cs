using System;
using RDFCommon;


namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlBnode : SparqlExpression
    {
        public SparqlBnode(SparqlExpression value, RdfQuery11Translator q)
        {
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            value.SetVariablesTypes(ExpressionType.@string);
            SetVariablesTypes(ExpressionType.BlankNode);
            Func = result =>
            {
                var str = value.Func(result);
                if (str is IStringLiteralNode)
                    return q.Store.NodeGenerator.CreateBlankNode(str.Content);
                throw new ArgumentException();
            };
        }

        public SparqlBnode(RdfQuery11Translator q)
        {
            SetVariablesTypes(ExpressionType.BlankNode);
            Func = result => q.Store.NodeGenerator.CreateBlankNode();
        }
    }
}
