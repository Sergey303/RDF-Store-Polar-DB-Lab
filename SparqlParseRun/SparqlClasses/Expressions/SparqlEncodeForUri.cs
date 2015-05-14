using System;
using System.Web;
using RDFCommon;
using RDFCommon.OVns;


namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlEncodeForUri : SparqlExpression
    {
        public SparqlEncodeForUri(SparqlExpression value, RdfQuery11Translator q)
        {                    
            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            SetVariablesTypes(ExpressionType.@string);
            value.SetVariablesTypes(ExpressionType.stringOrWithLang);
            Func = result =>
            {
                var f = value.Func(result).Content;
                if (f is string)
                {
                    return new OV_string(HttpUtility.UrlEncode(f));
                }
                if (f is ILanguageLiteral)
                {
                    return new OV_string(HttpUtility.UrlEncode(f.Content));
                }

                throw new ArgumentException();
            };
        }
    }
}
