using System;
using System.Collections.Generic;
using System.IO;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun;
using SparqlParseRun.SparqlClasses.Query.Result;
using TestingNs;

namespace RDFTripleStore
{
    public class StoreCascadingInt : GraphCascadingInt, IStore //CacheMeasure  GraphCached   InterpretMeasure
    {

        public StoreCascadingInt(string path)
                   //        : base(new SecondStringGraph(path)) 
         :   base(path)
        {
        }

        public void ReloadFrom(string fileName)
        {
          //  ClearAll();
            FromTurtle(fileName);    
        }

     

        public void ReloadFrom(Stream baseStream)
        {
          //  ClearAll();
          //base.FromTurtle(baseStream);    
        }

       
        private SparqlResultSet Run(SparqlQuery queryContext)
        {
             return queryContext.Run();
        }


        public IStoreNamedGraphs NamedGraphs { get; private set; }
        public void ClearAll()
        {
           base.Clear();
        }

        public IGraph CreateTempGraph()
        {
           return new RamListOfTriplesGraph("temp");
        }


   
    }
}
