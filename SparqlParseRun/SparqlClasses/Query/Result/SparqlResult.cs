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
     //   private readonly Dictionary<VariableNode, ObjectVariants> row;
        

        public SparqlResult(SparqlResult old, ObjectVariants newObj, VariableNode variable)
        {
           
     
            
            rowArray = old.rowArray;
            rowArray[variable.Index] = newObj;
        }
        public SparqlResult(SparqlResult old, ObjectVariants newObj1, VariableNode variable1, ObjectVariants newObj2, VariableNode variable2)
        {
       
            rowArray = old.rowArray;
            rowArray[variable1.Index] = newObj1;
            rowArray[variable2.Index] = newObj2; 

        }

        public SparqlResult(RdfQuery11Translator q)
        {                                        
            rowArray = new ObjectVariants[q.Variables.Count];
        }



        public ObjectVariants this[VariableNode var]
        {
            get
            {
                return rowArray[var.Index];
            }
            set
            {
                rowArray[var.Index] = value;

            }
        }

     
        public bool ContainsKey(VariableNode var)
        {
            return rowArray[var.Index]!=null;
        }                       

        public bool Equals(SparqlResult other)
        {
           // return ((IStructuralComparable) row).CompareTo(other.row, Comparer<INode>.Default)==0;

            return TestAll((var, value) => value.Equals(other[var]));
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
                return rowArray.Sum(value=>(int)Math.Pow(value.GetHashCode(), 2));
            }
        }
        private readonly ObjectVariants[] rowArray;

        public SparqlResult(SparqlResult old, ObjectVariants newObj1, VariableNode variable1, ObjectVariants newObj2, VariableNode variable2, ObjectVariants newObj3, VariableNode variable3)
        {
            
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
         rowArray[variable.Index]= value;
        }

        public IEnumerable<T> GetAll<T>(Func<VariableNode,ObjectVariants, T> selector)
        {
            return q.Variables.Values.Select(v => selector(v, rowArray[v.Index]));
        }

        public bool TestAll(Func<VariableNode, ObjectVariants, bool> selector)
        {
            return q.Variables.Values.All(v => selector(v, rowArray[v.Index]));
        }

     
        public void SeletSelection(List<VariableNode> selected)
        {
            this.selected = selected;
        }

        public IEnumerable<T> GetSelected<T>(Func<VariableNode, ObjectVariants, T> selector)
        {
            return selected.Select(v => selector(v, rowArray[v.Index]));
        }

       // private readonly Dictionary<string, VariableNode> Variables;

        private List<VariableNode> selected;
        private RdfQuery11Translator q;

        private SparqlResult(ObjectVariants[] copy)
        {
            rowArray = copy;
        }

        //public IEnumerator<SparqlResult> Branching() 
        public bool[] Backup()
        {
            return rowArray.Select(v => v != null).ToArray();
        }

        public void Restore(bool[] backup)
        {
            for (int i = 0; i < backup.Length; i++)
            {
                if(backup[i]) continue;
                rowArray[i] = null;
            }
        }


        public SparqlResult Clone()
        {
            ObjectVariants[] copy=new ObjectVariants[rowArray.Length];
            rowArray.CopyTo(copy,0);
            return new SparqlResult(copy);
        }
    }
}