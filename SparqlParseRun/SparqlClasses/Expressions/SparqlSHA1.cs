using System;
using System.Linq;
using System.Security.Cryptography;
using RDFCommon;
using System.Text;

namespace SparqlParseRun.SparqlClasses.Expressions
{
   
    class SparqlSHA1 : SparqlExpression
    {
        private readonly SHA1 hash;
             public SparqlSHA1(SparqlExpression value)
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
