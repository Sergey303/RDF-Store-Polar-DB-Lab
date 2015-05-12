﻿using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Expressions;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern
{
    public class SparqlExpressionAsVariable : IVariableNode, ISparqlGraphPattern
    {
        public VariableNode variableNode;
        public SparqlExpression sparqlExpression;
        private readonly RdfQuery11Translator q;

        public SparqlExpressionAsVariable(VariableNode variableNode, SparqlExpression sparqlExpression, RdfQuery11Translator q)
        {
            // TODO: Complete member initialization
            this.variableNode = variableNode;
            this.sparqlExpression = sparqlExpression;
            this.q = q;
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            return variableBindings.Select(
                variableBinding =>
                {
                    variableBinding.Add(variableNode,RunExpressionCreateBind(variableBinding));
                    return variableBinding;
                });
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.Bind;} }

        public ObjectVariants RunExpressionCreateBind(SparqlResult variableBinding)
        {
            return sparqlExpression.TypedOperator(variableBinding);
        }


      
    }

    
}
