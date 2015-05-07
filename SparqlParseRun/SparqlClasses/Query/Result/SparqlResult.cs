using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlParseRun.SparqlClasses.Query.Result
{
    public class SparqlResult : IEnumerable<ObjectVariants>
    {
        //public bool result;
        public readonly Dictionary<VariableNode, SparqlVariableBinding> row;
        

        public SparqlResult(SparqlResult old, ObjectVariants newObj, VariableNode variable)
        {
            row=new Dictionary<VariableNode, SparqlVariableBinding>(old.row)
            {
                {variable, new SparqlVariableBinding(variable,newObj)}
            };
        }
        public SparqlResult(SparqlResult old, ObjectVariants newObj1, VariableNode variable1, ObjectVariants newObj2, VariableNode variable2)
        {
            if(variable2==null)
                row = new Dictionary<VariableNode, SparqlVariableBinding>(old.row)
            {
                {variable1, new SparqlVariableBinding(variable1,newObj1)},
            };
            else
            row = new Dictionary<VariableNode, SparqlVariableBinding>(old.row)
            {
                {variable1, new SparqlVariableBinding(variable1,newObj1)},
                {variable2, new SparqlVariableBinding(variable2,newObj2)}
            };
        }
        internal SparqlResult(Dictionary<VariableNode, SparqlVariableBinding> sparqlResult)
        {
            row = sparqlResult;
        }

        public SparqlResult()
        {
          row=new Dictionary<VariableNode, SparqlVariableBinding>();
            rowArray = new ObjectVariants[1];
        }

       

        public SparqlVariableBinding this[VariableNode index]
        {
            get { return row[index]; }
            set
            {
                row[index]= value;
            }
        }




        IEnumerator<ObjectVariants> IEnumerable<ObjectVariants>.GetEnumerator()
        {
            return row.Values.Select(b => b.Value).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
       

        

        public bool Equals(SparqlResult other)
        {
           // return ((IStructuralComparable) row).CompareTo(other.row, Comparer<INode>.Default)==0;
            if (row.Count != other.row.Count) return false;
            SparqlVariableBinding b;
            return row.All(sparqlVariableBinding => other.row.TryGetValue(sparqlVariableBinding.Key, out b) && b.Value.Equals(sparqlVariableBinding.Value.Value));
        }

        public override bool Equals(object obj)
        {
            var sparqlResult = obj as SparqlResult;
            return sparqlResult != null && Equals(sparqlResult);
        }

      

        public override int GetHashCode()
        {
            unchecked
            {
                int sum = 0;
                foreach (KeyValuePair<VariableNode, SparqlVariableBinding> pair in row)
                    sum += pair.Key.VariableName.GetHashCode() ^ pair.Value.Value.GetHashCode();
                return sum;
            }
        }
        ObjectVariants[] rowArray=new ObjectVariants[];
    }
}