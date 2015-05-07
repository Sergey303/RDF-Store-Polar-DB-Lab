﻿using RDFCommon;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlVarExpression : SparqlExpression
    {
        public VariableNode Variable;

        public SparqlVarExpression(VariableNode variableNode)
        {
            // TODO: Complete member initialization
            Variable = variableNode;
            Func = result =>
            {
                SparqlVariableBinding sparqlVariableBinding;
                if (result.row.TryGetValue(Variable, out sparqlVariableBinding))
                    return sparqlVariableBinding.Value;
                else return new SparqlUnDefinedNode();
            };
        }

       
       

      
    }
}
