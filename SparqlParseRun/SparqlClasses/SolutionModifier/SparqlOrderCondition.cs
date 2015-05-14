using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Expressions;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SolutionModifier
{
    public class SparqlOrderCondition 
    {
        private readonly Func<dynamic, dynamic> orderCondition = node =>
        {
            if (node is SparqlUnDefinedNode) return string.Empty;
            if (node is IBlankNode) return node.ToString();
            if (node is ILiteralNode) return node.ToString();
            if (node is ObjectVariants) return node.ToString();  
            return node;
        };
        private readonly Func<SparqlResult, dynamic> getNode;

        private readonly Func<dynamic, int> orderByTypeCondition = node =>
        {
            if (node is SparqlUnDefinedNode)
                return 0;
            if (node is IBlankNode)
                return 1;
            if (node is ObjectVariants)
                return 2;
            if (node is IStringLiteralNode)
                return 3;
            return 4;
        };
        private SparqlOrderDirection direction=SparqlOrderDirection.Asc;

        public SparqlOrderCondition(SparqlExpression sparqlExpression, string dir)
        {
            // TODO: Complete member initialization
            switch (dir.ToLower())
            {
                case "desc":
                    direction = SparqlOrderDirection.Desc;
                    break;
                case "asc":
                default:
                    direction = SparqlOrderDirection.Asc;
                    break;
            }
            getNode = sparqlExpression.FunkClone;
          
        }

        private enum SparqlOrderDirection
        {
            Desc                                                 ,
            Asc
        }

        public SparqlOrderCondition(SparqlExpression sparqlExpression)
        {
            // TODO: Complete member initialization
            getNode = sparqlExpression.Func;

        }

        public SparqlOrderCondition(SparqlFunctionCall sparqlFunctionCall)
        {
            // TODO: Complete member initialization
            getNode = sparqlFunctionCall.Func;
        }

        public SparqlOrderCondition(VariableNode variableNode)
        {
            // TODO: Complete member initialization
            getNode = result =>
            {
                ObjectVariants sparqlVariableBinding = result[variableNode];
                if (sparqlVariableBinding !=null)
                    return sparqlVariableBinding is ILiteralNode &&  !(sparqlVariableBinding is ILanguageLiteral)
                        ? ((ILiteralNode)sparqlVariableBinding ).Content
                        : sparqlVariableBinding;
                else return new SparqlUnDefinedNode(); 
            };

        }

       public IEnumerable<SparqlResult> Order(IEnumerable<SparqlResult> resultSet)
        {
           switch (direction)
           {
               case SparqlOrderDirection.Desc:
                   return from r in resultSet
                              let node=getNode(r)
                              orderby orderByTypeCondition(node) descending, orderCondition(node) descending 
                              select r;
                   break;
               case SparqlOrderDirection.Asc:
               default:
                   return from r in resultSet
                          let node = getNode(r)
                          orderby orderByTypeCondition(node), orderCondition(node)
                          select r;
                   break;
           }
        }

     

     
    }
}
