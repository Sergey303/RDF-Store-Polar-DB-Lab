using System.Collections.Generic;
using RDFCommon.OVns;

namespace SparqlParseRun
{
    public class DataSet : List<ObjectVariants>
    {
        public DataSet(IEnumerable<ObjectVariants> gs)
            :base(gs)
        {
            

        }

        public DataSet()                 
        {
            
        }
    }
}