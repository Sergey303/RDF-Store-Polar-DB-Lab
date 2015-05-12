using System;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlBoolExpression : SparqlExpression
    {
        public SparqlBoolExpression(Func<SparqlResult, bool> op)
        {
            
            Operator = result => op(result);
            TypedOperator = result => new OV_bool(op(result));
            SetExprType(ObjectVariantEnum.Bool);
        }
    }
}