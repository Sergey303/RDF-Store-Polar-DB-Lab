using System;
using RDFCommon;
using RDFCommon.OVns;


namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlLang  : SparqlExpression
    {
        public SparqlLang(SparqlExpression value, INodeGenerator q)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            Func = result =>
            {
                var f = value.Func(result);
                if (f is ILanguageLiteral)
                    return new OV_string(((ILanguageLiteral)f).Lang.Substring(1));
                throw new ArgumentException();
            };
        }
    }
}
