using System;
using System.Collections.Generic;
using System.Linq;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.Query
{
    public class SparqlSelect : List<IVariableNode>
    {

        private bool isAll;
        internal bool IsReduced;


        internal bool IsDistinct { get; set; }
    

        internal void IsAll()
        {
            isAll = true;
        }


        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings, SparqlResultSet resultSet)
        {
            List<VariableNode> selected=null;

            if (isAll)
            {
                selected = resultSet.Variables.Values.Where(v => !(v is SparqlBlankNode)).ToList();

            }
            else
            {
                selected = new List<VariableNode>();
                foreach (IVariableNode variable in this)
                {
                    var expr = variable as SparqlExpressionAsVariable;
                    if (expr != null)
                    {
                        variableBindings = expr.Run(variableBindings);
                        selected.Add(expr.variableNode);
                    }
                    else selected.Add((VariableNode) variable);
                }
            }
            variableBindings = variableBindings.Select(result =>
                {
                    result.SeletSelection(selected);
                    return result;
                });
            if (IsDistinct)
                variableBindings = variableBindings.Distinct(new BindingsComparer());
            if (IsReduced)
                variableBindings = Reduce(variableBindings);

            return variableBindings;
        }

        private static IEnumerable<SparqlResult> Reduce(IEnumerable<SparqlResult> results)
        {
            var duplicated=new Dictionary<SparqlResult, bool>();
            foreach (var res in results)
            {
                if (duplicated.ContainsKey(res))
                {
                    if(duplicated[res]) continue;
                    duplicated[res] = true;
                }
                else duplicated.Add(res, false);
                yield return res;
            }
        }

        class BindingsComparer :  IEqualityComparer<SparqlResult>
        {
            public bool Equals(SparqlResult x, SparqlResult y)
            {
                //if (x.Count != y.Count) return false;
                SparqlVariableBinding v2;
                return x.TestAll((var, value) => y[var].Equals(x[var]));
            }

            public int GetHashCode(SparqlResult obj)
            {
                unchecked
                {
                    return obj.GetHashCode();
                }
            }
        }

        

    }
}
