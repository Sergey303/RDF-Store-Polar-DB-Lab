using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RDFCommon;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    internal class SparqlSHA384 : SparqlExpression
    {
        private readonly SHA384 hash = new SHA384CryptoServiceProvider();

        public SparqlSHA384(SparqlExpression value)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            SetVariablesTypes(ExpressionType.@string);
            value.SetVariablesTypes(ExpressionType.@string);

            Func = result => value.Func(result).Change(o => CreateHash(o));
        }

        private string CreateHash(string f)
        {
            return string.Join("",
                hash.ComputeHash(Encoding.UTF8.GetBytes(f)).Select(b => b.ToString("x2")));
        }
    }
}
