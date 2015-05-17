using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RDFCommon;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlMD5 : SparqlExpression
    {
        readonly MD5 md5 = new MD5CryptoServiceProvider();

             public SparqlMD5(SparqlExpression value)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            TypedOperator = result => value.TypedOperator(result).Change(o => CreateHash(o));

        }

        private string CreateHash(string f)
        {
            return string.Join("",
                md5.ComputeHash(Encoding.UTF8.GetBytes(f)).Select( b => b.ToString("x2")));
        }
    }
}
