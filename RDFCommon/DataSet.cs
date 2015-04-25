using System.Collections.Generic;

namespace RDFCommon
{
    public class DataSet : List<IGraphNode>
    {
        public DataSet(IEnumerable<IGraphNode> gs)
            :base(gs)
        {
            

        }

        public DataSet()                 
        {
            
        }
    }
}