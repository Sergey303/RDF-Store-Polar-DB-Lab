using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlVarExpression : SparqlExpression
    {
     //  public VariableNode Variable;

        public SparqlVarExpression(VariableNode variableNode)
        {
            // TODO: Complete member initialization
            //Variable = variableNode;
            Operator = result =>
            {
                //ObjectVariants sparqlVariableBinding;
                ////if (result.TryGetValue(Variable, out sparqlVariableBinding))
                ////    return sparqlVariableBinding;
                ////else return new SparqlUnDefinedNode();
                return result[variableNode].Content;
            };
            TypedOperator = result =>
            {
                //ObjectVariants sparqlVariableBinding;
                ////if (result.TryGetValue(Variable, out sparqlVariableBinding))
                ////    return sparqlVariableBinding;
                ////else return new SparqlUnDefinedNode();
                return result[variableNode];
            };
        }

       
       

      
    }
}
