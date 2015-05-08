using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlParseRun.SparqlClasses.Query.Result
{
    public class SparqlResult
    {
        //public bool result;
        private readonly Dictionary<VariableNode, ObjectVariants> row;
        

        public SparqlResult(SparqlResult old, ObjectVariants newObj, VariableNode variable)
        {
            row=new Dictionary<VariableNode, ObjectVariants>(old.row)
            {
                {variable, newObj}
            };
            rowArray = new[] {newObj};

        }
        public SparqlResult(SparqlResult old, ObjectVariants newObj1, VariableNode variable1, ObjectVariants newObj2, VariableNode variable2)
        {
            if(variable2==null)
                row = new Dictionary<VariableNode, ObjectVariants>(old.row)
            {
                {variable1, newObj1},
            };
            else
                row = new Dictionary<VariableNode, ObjectVariants>(old.row)
            {
                {variable1 ,newObj1},
                {variable2, newObj2}
            };
        }
    

        public SparqlResult()
        {
            row = new Dictionary<VariableNode, ObjectVariants>();
            rowArray = new ObjectVariants[1];
        }



        public ObjectVariants this[VariableNode var]
        {
            get
            {

                ObjectVariants value;
            if (row.TryGetValue(var, out value))
                return value;
            else return null;// new SparqlUnDefinedNode();
            }
            set
            {
                row[var] = value;
                rowArray = new ObjectVariants[] { value };

            }
        }

     
        public bool ContainsKey(VariableNode var)
        {
            return row.ContainsKey(var);
        }

  
       

        

        public bool Equals(SparqlResult other)
        {
           // return ((IStructuralComparable) row).CompareTo(other.row, Comparer<INode>.Default)==0;
            if (row.Count != other.row.Count) return false;
            ObjectVariants b;
            return row.All(sparqlVariableBinding => other.row.TryGetValue(sparqlVariableBinding.Key, out b) && b.Equals(sparqlVariableBinding.Value));
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
                foreach (KeyValuePair<VariableNode, ObjectVariants> pair in row)
                    sum += pair.Key.VariableName.GetHashCode() ^ pair.Value.GetHashCode();
                return sum;
            }
        }
        public ObjectVariants[] rowArray;

        public SparqlResult(SparqlResult old, ObjectVariants newObj1, VariableNode variable1, ObjectVariants newObj2, VariableNode variable2, ObjectVariants newObj3, VariableNode variable3)
        {
            throw new NotImplementedException();
        }

        public SparqlResult(SparqlResult old, ObjectVariants newObj1, VariableNode variable1, ObjectVariants newObj2, VariableNode variable2, ObjectVariants newObj3, VariableNode variable3, ObjectVariants arg4, VariableNode variable4)
        {
            throw new NotImplementedException();
        }

        public SparqlResult(IEnumerable<SparqlVariableBinding> old)
        {
            throw new NotImplementedException();
        }

        public void Add(VariableNode variable, ObjectVariants value)
        {
         row.Add(variable, value);
        }

        public IEnumerable<T> GetAll<T>(Func<VariableNode,ObjectVariants, T> selector)
        {
            return row.Select(binding => selector(binding.Key, binding.Value));
        }
        public bool TestAll(Func<VariableNode, ObjectVariants, bool> selector)
        {
            return row.All(binding => selector(binding.Key, binding.Value));
        }

        public void RemoveBlanks()
        {
            var listForRemove = new List<VariableNode>();
            listForRemove.AddRange(GetAll((var, node) => var).Where(v => v is SparqlBlankNode));
            foreach (var variableNode in listForRemove)
                row.Remove(variableNode);
        }
    }
}