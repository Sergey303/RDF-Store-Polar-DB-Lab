using System;
using System.Linq;
using RDFCommon;
using RDFCommon.Interfaces;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.Query;
using SparqlParseRun.SparqlClasses.Query.Result;
using SparqlParseRun.SparqlClasses.SolutionModifier;


namespace SparqlParseRun
{
    public abstract class SparqlQuery 
    {
        internal SparqlGraphPattern sparqlWhere;
        internal readonly SparqlResultSet ResultSet;
        protected RdfQuery11Translator q;
        protected SparqlSolutionModifier sparqlSolutionModifier;

        public SparqlQuery(RdfQuery11Translator q)
        {
            this.q = q;
            ResultSet=new SparqlResultSet(q);
        }


        public virtual SparqlResultSet Run()
        {
            
            ResultSet.Variables = q.Variables;
            ResultSet.Results = Enumerable.Repeat(new SparqlResult(q), 1);

            ResultSet.Results = sparqlWhere.Run(ResultSet.Results);
         
            if (sparqlSolutionModifier != null)
                ResultSet.Results = sparqlSolutionModifier.Run(ResultSet.Results);   
            return ResultSet;
        }
        public SparqlQueryTypeEnum Type { get; set; }
        public abstract SparqlQueryTypeEnum QueryType { get; }


        internal void SetValues(ISparqlGraphPattern valueDataBlock)
        {
            if(valueDataBlock!=null)
          ResultSet.Results=   valueDataBlock.Run(ResultSet.Results);
           
        }

      
    }
}
