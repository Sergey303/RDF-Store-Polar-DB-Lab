using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;
using SparqlParseRun.SparqlClasses.SolutionModifier;

namespace SparqlParseRun.SparqlClasses.Query
{
    public class SparqlDescribeQuery : SparqlQuery
    {
        private readonly List<ObjectVariants> nodeList = new List<ObjectVariants>();
        private bool isAll;


        public SparqlDescribeQuery(RdfQuery11Translator q) : base(q)
        {
            
        }

        internal void Add(ObjectVariants sparqlNode)
        {
            nodeList.Add(sparqlNode);
        }

        internal void IsAll()
        {
            isAll = true;
        }

        internal void Create(SparqlGraphPattern sparqlWhere)
        {
            this.sparqlWhere = sparqlWhere;
        }

        internal void Create(SparqlSolutionModifier sparqlSolutionModifier)
        {
            this.sparqlSolutionModifier = sparqlSolutionModifier;
        }

        public override SparqlResultSet Run(IStore store)
         {
            base.Run(store);
            var rdfInMemoryGraph = store.CreateTempGraph();
            if (isAll)
                foreach (ObjectVariants node in ResultSet.Results.SelectMany(result => result
                    .GetAll   ((var, value) => value)
                    //.Where(node => node is ObjectVariants).Cast<ObjectVariants>()
                    ))
                {
                    ObjectVariants node1 = node;
                    foreach (var temp in store.GetTriplesWithSubject(node, (p, o) =>
                    {
                        rdfInMemoryGraph.Add(node1, p, o);
                        return true;
                    })) ;
                    foreach (var temp in store.GetTriplesWithObject(node, (s, p) =>
                    {
                        rdfInMemoryGraph.Add(s, p, node1);
                        return true;
                    })) ;
                }
            else
            {
                foreach (ObjectVariants node in nodeList.Where(node => node is VariableNode)
                    .SelectMany(uriNode => ResultSet.Results.SelectMany(result => result
                                            .GetAll((var, value) => value)
                        //.Where(node => node is ObjectVariants).Cast<ObjectVariants>()
                        )))
                {
                    ObjectVariants node1 = node;
                    foreach (var temp in store.GetTriplesWithSubject(node, (p, o) =>
                    {
                        rdfInMemoryGraph.Add(node1, p, o);
                        return true;
                    })) ;
                    foreach (var temp in store.GetTriplesWithObject(node, (s, p) =>
                    {
                        rdfInMemoryGraph.Add(s, p, node1);
                        return true;
                    })) ;
                }
                foreach (ObjectVariants node in nodeList.Where(node => !(node is VariableNode)))

                    {
                        ObjectVariants node1 = node;
                        foreach (var temp in store.GetTriplesWithSubject(node, (p, o) =>
                        {
                            rdfInMemoryGraph.Add(node1, p, o);
                            return true;
                        })) ;
                        foreach (var temp in store.GetTriplesWithObject(node, (s, p) =>
                        {
                            rdfInMemoryGraph.Add(s, p, node1);
                            return true;
                        })) ;
                    }
            }
            ResultSet.ResultType = ResultType.Describe;
            ResultSet.GraphResult = rdfInMemoryGraph;
            return ResultSet;
        }

        public override SparqlQueryTypeEnum QueryType
        {
            get { return SparqlQueryTypeEnum.Describe; }
        }
    }
}
