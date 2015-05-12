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
            SetExprType(ObjectVariantEnum.Str);
            value.SetExprType(ExpressionTypeEnum.stringOrWithLang);
            TypedOperator = result =>
            {
                var f = value.TypedOperator(result);
                if (f is string)
                    //todo
                {
                    //return new OV_string(HttpUtility.UrlEncode(f));
                }
                if (f is ILanguageLiteral)
                {
                    return new OV_string(HttpUtility.UrlEncode((string) f.Content));
                }

                throw new ArgumentException();
            };
        }
    }
}
