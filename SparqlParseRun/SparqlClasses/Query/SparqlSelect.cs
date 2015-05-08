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


        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings, SparqlResultSet resultSet = null)
        {
            var bindings = variableBindings as SparqlResult[] ?? variableBindings.ToArray();
            if (bindings.Length == 0) return bindings;
            if (isAll)
            {
              
                foreach (var sparqlResult in bindings)
                {
                    sparqlResult.RemoveBlanks();
                }
            }
            else
            {
                if (resultSet != null)
                    resultSet.Variables.Clear();
                SparqlResult[] newList = bindings.Select(result => new SparqlResult()).ToArray();
                foreach (var v in this)
                {
                    var expressionAsVariable = v as SparqlExpressionAsVariable;
                    var variableNode = (v as VariableNode);
                    if (expressionAsVariable != null)
                    {
                        if (resultSet != null)
                            resultSet.Variables.Add(expressionAsVariable.variableNode.VariableName,
                                expressionAsVariable.variableNode);
                        if (!(bindings.First() is SpraqlGroupOfResults) &&
                            expressionAsVariable.sparqlExpression.IsAggragate)
                        {
                            bindings = new SparqlResult[] {new SpraqlGroupOfResults() {Group = bindings}};
                            newList = new SparqlResult[] {new SpraqlGroupOfResults() {Group = newList}};
                        }

                        for (int i = 0; i < newList.Length; i++)
                        {
                            var newVariable = expressionAsVariable.RunExpressionCreateBind(bindings[i]);
                            newList[i].Add(expressionAsVariable.variableNode, newVariable);
                            bindings[i].Add(expressionAsVariable.variableNode, newVariable);
                        }
                    }
                    else if (variableNode != null)
                    {
                        if (resultSet != null)
                            resultSet.Variables.Add(variableNode.VariableName, variableNode);
                        for (int i = 0; i < newList.Length; i++)
                            if (bindings[i].ContainsKey(variableNode))
                                newList[i].Add(variableNode, bindings[i][variableNode]);
                    }
                    else
                        throw new ArgumentNullException("variableNode");
                }

                bindings = newList;
            }
            if (IsDistinct)
                bindings = bindings.Distinct(new BindingsComparer()).ToArray();
            if (IsReduced)
            {
                var counts = new Dictionary<SparqlResult, int>();
                foreach (var variableBinding in bindings)
                    if (counts.ContainsKey(variableBinding)) counts[variableBinding]++;
                    else counts.Add(variableBinding, 1);
                bindings = Reduce(counts).ToArray();
            }

            return bindings;
        }

        private static IEnumerable<SparqlResult> Reduce(Dictionary<SparqlResult, int> counts)
        {
            foreach (var count in counts)
            {
                yield return count.Key;
                if (count.Value > 1)
                    yield return count.Key;
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
                    int sum = 0;
                    obj.GetAll((var, value) => 
                        sum +=(int) Math.Pow(value.GetHashCode(), 2));
                    return sum;
                }
            }
        }

        

    }
}
