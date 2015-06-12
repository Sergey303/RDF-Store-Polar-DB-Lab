using System;
using System.Collections;
using System.Collections.Generic;
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

        public SparqlExpressionAsVariable(VariableNode variableNode, SparqlExpression sparqlExpression)
        {
            // TODO: Complete member initialization
            this.variableNode = variableNode;
            this.sparqlExpression = sparqlExpression;
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            switch (sparqlExpression.AggregateLevel)
            {
                case SparqlExpression.VariableDependenceGroupLevel.Const:
                    return variableBindings.Select(
                        variableBinding =>
                        {
                            variableBinding.Add(variableNode, sparqlExpression.Const);
                            return variableBinding;
                        });
                    break;
                case SparqlExpression.VariableDependenceGroupLevel.UndependableFunc:
                case SparqlExpression.VariableDependenceGroupLevel.SimpleVariable:
                    return variableBindings.Select(
                        variableBinding =>
                        {
                            variableBinding.Add(variableNode, sparqlExpression.Operator(variableBinding));
                            return variableBinding;
                        });
                case SparqlExpression.VariableDependenceGroupLevel.Group:
                    if (variableBindings is SparqlGroupsCollection)

                else
                    break;
                case SparqlExpression.VariableDependenceGroupLevel.GroupOfGroups:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

    public class SparqlGroupsCollection: IEnumerable<SparqlGroupOfResults>
    {
        public IEnumerable<SparqlGroupOfResults> groups;

        public SparqlGroupsCollection(IEnumerable<SparqlGroupOfResults> groups)
        {
            this.groups = groups;
        }

        public IEnumerator<SparqlGroupOfResults> GetEnumerator()
        {
            return groups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
