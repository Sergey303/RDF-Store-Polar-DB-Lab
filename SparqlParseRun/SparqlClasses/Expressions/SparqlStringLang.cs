using System;
using RDFCommon;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStringLang  :SparqlExpression
    {
        public SparqlStringLang(SparqlExpression literalExpression, SparqlExpression langExpression, INodeGenerator q)
        {
            IsAggragate = langExpression.IsAggragate || literalExpression.IsAggragate;
            IsDistinct = langExpression.IsDistinct || literalExpression.IsDistinct;
            Func = result =>
            {
                var literal = literalExpression.Func(result).Content;
                var lang = langExpression.Func(result).Content;
                
                if (literal is string && lang is string)
                {
                    return new OV_langstring(literal, lang);
                }
                throw new ArgumentException();
            };
        }
    }
}
