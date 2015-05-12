using System;
using System.Linq;
using System.Security.Cryptography;
using RDFCommon;
using System.Text;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
   
    class SparqlSHA1 : SparqlExpression
    {
        private readonly SHA1 hash;
             public SparqlSHA1(SparqlExpression value)
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
