using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RDFCommon;

namespace SparqlParseRun.SparqlClasses.Expressions
{
  public  class SparqlSHA512 : SparqlExpression
    {
      readonly SHA512 hash=new SHA512CryptoServiceProvider();
            public SparqlSHA512(SparqlExpression value)
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
                hash.ComputeHash(Encoding.UTF8.GetBytes(f)).Select( b => b.ToString("x2")));
        }
    }
}
