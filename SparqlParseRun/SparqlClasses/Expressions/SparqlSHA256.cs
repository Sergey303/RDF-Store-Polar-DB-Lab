using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlSHA256 : SparqlExpression
    {
        private SparqlExpression sparqlExpression;
        readonly SHA256 hash=new SHA256CryptoServiceProvider();
        public SparqlSHA256(SparqlExpression value)
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
