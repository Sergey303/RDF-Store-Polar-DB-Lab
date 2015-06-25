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
            
            NodeGenerator=
                ng = NodeGeneratorInt.Create(path, table.TableCell.IsEmpty);
            NamedGraphs = new NamedGraphsByFolders(new DirectoryInfo(path), ng, d => new GraphCascadingInt(d.FullName + "/"){NodeGenerator=NodeGenerator});
        }

        private readonly NodeGeneratorInt ng;

        public void ReloadFrom(string fileName)
        {
            ng.Clear();  //  ClearAll();
            FromTurtle(fileName);
            ng.Build();
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
