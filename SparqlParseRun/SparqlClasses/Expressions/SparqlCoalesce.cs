using System;
using System.Collections.Generic;
using System.Linq;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlCoalesce : SparqlExpression
    {
        public SparqlCoalesce(List<SparqlExpression> list)
        {
            //todo
                   IsAggragate = list.Any(value=> value.IsAggragate);
            IsDistinct = list.Any(value=>  value.IsDistinct);
            TypedOperator = result =>
                    
            {
                foreach (var sparqlExpression in list)
                    try
                    {
                        var test = sparqlExpression.TypedOperator(result);
                      //  if(test is SparqlUnDefinedNode) continue;
                        return test;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                throw new Exception("Coalesce ");
            };
        }
    }
}
