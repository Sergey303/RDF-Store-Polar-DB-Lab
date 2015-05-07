using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Expressions;
using SparqlParseRun.SparqlClasses.GraphPattern;

namespace SparqlParseRun.SparqlClasses
{
    public class SparqlExistsExpression :SparqlExpression
    {
        //private SparqlGraphPattern sparqlGraphPattern;

        public SparqlExistsExpression(ISparqlGraphPattern sparqlGraphPattern)
        {
            // TODO: Complete member initialization
            //this.sparqlGraphPattern = sparqlGraphPattern;
            SetVariablesTypes(ExpressionType.@bool);
            Func = variableBinding => new OV_bool(sparqlGraphPattern.Run(Enumerable.Repeat(variableBinding, 1)).Any()); 
        }

    }
}
