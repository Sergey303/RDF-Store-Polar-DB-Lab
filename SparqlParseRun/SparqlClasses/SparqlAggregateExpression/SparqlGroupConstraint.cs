﻿using System;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Expressions;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SparqlAggregateExpression
{
    public class SparqlGroupConstraint
    {
        public readonly Func<SparqlResult, ObjectVariants> Constrained;
        public bool IsDistinct;


        public SparqlGroupConstraint(SparqlExpression sparqlExpression)
        {
            // TODO: Complete member initialization
            Constrained = sparqlExpression.Func;
            IsDistinct = sparqlExpression.IsDistinct;
        }

        public SparqlGroupConstraint(SparqlFunctionCall sparqlFunctionCall)
        {
            // TODO: Complete member initialization
            Constrained = sparqlFunctionCall.Func;
            IsDistinct = sparqlFunctionCall.sparqlArgs.isDistinct;
        }

        public SparqlGroupConstraint(VariableNode variableNode)
        {
            // TODO: Complete member initialization
            Variable = variableNode;
            Constrained = (result => result[variableNode].Value);
        }

        public VariableNode Variable { get; set; }

        public SparqlGroupConstraint(SparqlExpressionAsVariable sparqlExpressionAsVariable)
        {
            // TODO: Complete member initialization
            Variable = sparqlExpressionAsVariable.variableNode;
            Constrained = (result=>sparqlExpressionAsVariable.RunExpressionCreateBind(result).Value);
            IsDistinct = sparqlExpressionAsVariable.sparqlExpression.IsDistinct;
        }


    }
}
