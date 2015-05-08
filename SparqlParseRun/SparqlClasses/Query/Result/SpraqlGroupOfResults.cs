using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlParseRun.SparqlClasses.Query.Result
{
    public class SpraqlGroupOfResults : SparqlResult
    {
        public IEnumerable<SparqlResult> Group;

        public SpraqlGroupOfResults(VariableNode variable, ObjectVariants value)
        {
            Add(variable, value);
        }

        public SpraqlGroupOfResults()
        {
          
        }

        public SpraqlGroupOfResults(IEnumerable<VariableNode> variables, List<ObjectVariants> values)
        {
            int i = 0;
            var valuesArray = values.ToArray();
            foreach (var variable in variables)
            {
                i++;
                if(variable==null) continue;
                Add(variable, valuesArray[i]);
            }   
        }
    }
}