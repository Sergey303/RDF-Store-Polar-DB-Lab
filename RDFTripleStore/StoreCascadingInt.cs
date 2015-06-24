using System;
using System.Collections.Generic;
using System.IO;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun;
using SparqlParseRun.SparqlClasses.Query.Result;


namespace RDFTripleStore
{
    public class StoreCascadingInt : GraphCascadingInt, IStore //CacheMeasure  GraphCached   InterpretMeasure
    {

        public StoreCascadingInt(string path)
                   //        : base(new SecondStringGraph(path)) 
         :   base(path)
        {
            NamedGraphs = new NamedGraphsByFolders(new DirectoryInfo(path), NodeGenerator, d=> new GraphCascadingInt(d.FullName));
        }

        public void ReloadFrom(string fileName)
        {
          //  ClearAll();
            FromTurtle(fileName);    
        }

     

        public void ReloadFrom(Stream baseStream)
        {
            ClearAll();
          base.FromTurtle(baseStream);    
        }

       
        private SparqlResultSet Run(SparqlQuery queryContext)
        {
             return queryContext.Run();
        }


        public IStoreNamedGraphs NamedGraphs { get; set; }
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
