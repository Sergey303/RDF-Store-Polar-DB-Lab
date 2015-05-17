using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
   public class SparqlSubstringExpression  : SparqlExpression
   {
       private SparqlExpression strExpression, startExpression;
       private RdfQuery11Translator q;

       internal void SetString(SparqlExpression value)
       {
           IsAggragate = value.IsAggragate;
           IsDistinct = value.IsDistinct;

           strExpression = value;
       }
       public void SetStartPosition(SparqlExpression value)
       {

           IsAggragate = value.IsAggragate;
           IsDistinct = value.IsDistinct;

           startExpression = value;
           TypedOperator = result => strExpression.TypedOperator(result).Change(o => o.Substring(startExpression.TypedOperator(result).Content));
       }

       internal void SetLength(SparqlExpression lengthExpression)
       {
           IsAggragate = lengthExpression.IsAggragate;
           IsDistinct = lengthExpression.IsDistinct;
           TypedOperator = result => strExpression.TypedOperator(result).Change(o => o.Substring(startExpression.TypedOperator(result).Content, lengthExpression.TypedOperator(result).Content));
           
       }
    }
}
