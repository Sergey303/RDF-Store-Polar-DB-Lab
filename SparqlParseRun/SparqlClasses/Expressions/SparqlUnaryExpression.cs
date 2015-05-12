using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlUnaryExpression : SparqlExpression
    {
        

        public SparqlUnaryExpression(Func<dynamic,dynamic> @operator, SparqlExpression child)
        {
            IsAggragate = child.IsAggragate;
            IsDistinct = child.IsDistinct;
        
            var childConst = child.Const;
            if (childConst != null) Const = childConst.Change(@operator);
            else
            {
                Operator = @operator;
                TypedOperator = result => child.TypedOperator(result).Change(@operator);
            }
        }
    }
}
