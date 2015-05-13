using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RDFCommon;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Update;

namespace SparqlParseRun.SparqlClasses.Query.Result
{
    public class SparqlResultSet 
    {
        private readonly RdfQuery11Translator q;
        public IEnumerable<SparqlResult> Results ;
        public IGraph GraphResult;
        internal ResultType ResultType;

      

        public bool AnyResult
        {
            get { return Results.Any(); }
        }

        public string UpdateMessage;

        public SparqlUpdateStatus UpdateStatus;

        internal Dictionary<string, VariableNode> Variables = new Dictionary<string, VariableNode>();

        public SparqlResultSet(RdfQuery11Translator q)
        {
            this.q = q;
        }

        public XElement ToXml()
        {
            XNamespace xn = "http://www.w3.org/2005/sparql-results#";
            switch (ResultType)
            {
                case ResultType.Select:
                    return new XElement(xn + "sparql", new XAttribute(XNamespace.Xmlns + "ns", xn),
                        new XElement(xn + "head",
                            Variables.Select(v => new XElement(xn + "variable", new XAttribute(xn + "name", v.Key)))),
                        new XElement(xn + "results",
                            Results.Select(result =>
                                new XElement(xn + "result",
                                    result.GetSelected((var, value)=> 
                                        new XElement(xn + "binding",    
                                            new XAttribute(xn + "name", var.VariableName),
                                            BindingToXml(xn, value)))))));
                case ResultType.Describe:
                case ResultType.Construct:
                    return GraphResult.ToXml(q.prolog);
                case ResultType.Ask:
                    return new XElement(xn + "sparql", //new XAttribute(XNamespace.Xmlns , xn),
                        new XElement(xn + "head",
                            Variables.Select(v => new XElement(xn + "variable", new XAttribute(xn + "name", v.Key)))),
                        new XElement(xn + "boolean", AnyResult));
                case ResultType.Update:
                    return new XElement("update", new XAttribute("status", UpdateStatus.ToString()));
                default:                                       
                    throw new ArgumentOutOfRangeException();
            }
        }

        //public JsonConvert ToJson()

        private XElement BindingToXml(XNamespace xn, ObjectVariants b)
        {
            if (b is IIriNode)
            {
                return new XElement(xn + "uri", ((ObjectVariants)b).Content);
            }     
            else if (b is IBlankNode)
            {
                return new XElement(xn + "bnode", ((IBlankNode)b).Name);
            }
            else if (b is ILiteralNode)
            {
                var literalNode = ((ILiteralNode) b);
                if (literalNode is ILanguageLiteral)
                {
                    return new XElement(xn + "literal",
                        new XAttribute(xn + "lang", ((ILanguageLiteral) literalNode).Lang), literalNode.Content);
                }
                else if (literalNode is IStringLiteralNode)
                {
                    return new XElement(xn + "literal", literalNode.Content);
                }
                else //if (literalNode == LiteralType.TypedObject)
                {
                    return new XElement(xn + "literal", new XAttribute(xn + "type", (literalNode).DataType),
                        literalNode.Content);
                }
            }

            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString()
        {
            switch (ResultType)
            {
                case ResultType.Describe:
                case ResultType.Construct:
                    return GraphResult.ToString();
                case ResultType.Select:
                    //  return Results.ag.ToString();
                case ResultType.Ask:
                    return AnyResult.ToString();

                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public string ToJson()
        {
            string headVars;
            switch (ResultType)
            {
                case ResultType.Select:
                        headVars = string.Format(@"""head"": {{ ""vars"": [ {0} ] }}",
                            string.Join("," + Environment.NewLine,
                                Variables.Keys.Select(v => string.Format("\"{0}\"", v))));
                    return
                     string.Format(@"{{ {0}, ""results"": {{ ""bindings"" : [{1}] }} }}", headVars,
                         string.Join("," + Environment.NewLine, Results.Select(result => string.Format("{{{0}}}",
                             string.Join("," + Environment.NewLine,
                                 result.GetSelected((var, value) =>
                                     string.Format("\"{0}\" : {1}", var.VariableName,
                                         value.ToJson())))))) );
                case ResultType.Describe:
                case ResultType.Construct:
                    return GraphResult.ToJson();
                case ResultType.Ask:
                    headVars = string.Format(@"""head"": {{ ""vars"": [ {0} ] }}",
                        string.Join("," + Environment.NewLine,
                            Variables.Keys.Select(v => string.Format("\"{0}\"", v))));
                    return string.Format("{{ {0}, \"boolean\" : {1}}}", headVars, AnyResult);
                case ResultType.Update:
                    headVars = string.Format(@"""head"": {{ ""vars"": [ {0} ] }}",
                        string.Join("," + Environment.NewLine,
                            Variables.Keys.Select(v => string.Format("\"{0}\"", v))));
                    return string.Format("{{ {0}, \"status\" : {1}}}", headVars, UpdateStatus);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    
    }


    internal enum ResultType
    {
        Describe, Select, Construct, Ask,
        Update
    }

   
}