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
           Func = result => strExpression.Func(result).Change(o => o.Substring(startExpression.Func(result).Content));
       }

       internal void SetLength(SparqlExpression lengthExpression)
       {
           IsAggragate = lengthExpression.IsAggragate;
           IsDistinct = lengthExpression.IsDistinct;
           Func = result => strExpression.Func(result).Change(o => o.Substring(startExpression.Func(result).Content, lengthExpression.Func(result).Content));
           
       }
    }
}
