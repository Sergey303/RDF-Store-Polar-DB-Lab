using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
  public  class SparqlSHA512 : SparqlExpression
    {
      readonly SHA512 hash=new SHA512CryptoServiceProvider();
            public SparqlSHA512(SparqlExpression value)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            SetExprType(ObjectVariantEnum.Str);
            value.SetExprType(ObjectVariantEnum.Str);

            TypedOperator = result => value.TypedOperator(result).Change(o => CreateHash(o));
        }

        private string CreateHash(string f)
        {
            return string.Join("",
                hash.ComputeHash(Encoding.UTF8.GetBytes(f)).Select( b => b.ToString("x2")));
        }
    }
}
