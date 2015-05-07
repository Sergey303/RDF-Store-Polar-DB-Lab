using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RDFCommon;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Query.Result
{
    public static class RdfGraphSerialization
    {
        public static XElement ToXml(this IGraph g)
        {
            XNamespace rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
            var prefixes = new Dictionary<string, string>();                    
                
            var RDF = new XElement(rdf + "RDF", new XAttribute(XNamespace.Xmlns + "rdf", rdf));
            int i = 0;
            foreach (var s in g.GetAllSubjects())
            {
                XAttribute id = null;
                if (s is IIriNode)
                {
                    id = new XAttribute(rdf + "about", ((ObjectVariants)s).Content);
                }
                else if (s is IBlankNode)
                {
                    id = new XAttribute(rdf + "nodeID", ((IBlankNode)s).Name);
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
                RDF.Add(new XElement(rdf + "Description", id,
                    g.GetTriplesWithSubject(s, (p,o) =>
                    {
                        string prf;
                        string localName;
                        var ns = GetNsAndLocalName(p.Content, out localName);
                        if (!prefixes.TryGetValue(ns, out prf))
                        {
                            prefixes.Add(ns, prf = "ns" + i++);
                            RDF.Add(new XAttribute(XNamespace.Xmlns + prf, ns));
                        }
                        var x = new XElement(XName.Get(localName, prf));

                        if (o is IIriNode)
                        {
                            x.Add(new XAttribute(rdf + "resource", o.ToString()));
                        }   
                        else if (o is IBlankNode) //todo
                            {
                                x.Add(new XAttribute(rdf + "nodeID", ((IBlankNode)o).Name));
                            }
                        else
                        {
                            ILiteralNode literal = o as ILiteralNode;
                            if (literal != null)
                            {
                                if (literal is ILanguageLiteral)
                                {
                                    x.Add(new XAttribute(XNamespace.Xml + "lang", ((ILanguageLiteral) o).Lang));
                                }
                                else if (literal is IStringLiteralNode)
                                {
                                }
                                else
                                    x.Add(new XAttribute(rdf + "datatype", literal.DataType));
                                x.Add(literal.Content);
                            }

                            else
                            {
                                throw new ArgumentOutOfRangeException();
                            }
                        }
                        return x;
                    })));
            };
       
            return RDF;
        }

        private static string GetNsAndLocalName(string uri, out string localName)
        {
            var lastIndexOf1 = uri.LastIndexOf('\\');
            var lastIndexOf2 = uri.LastIndexOf('/');
            var lastIndexOf3 = uri.LastIndexOf('#');
            var lastIndex = Math.Max(lastIndexOf1, Math.Max(lastIndexOf2, lastIndexOf3));
            if(lastIndex==-1) throw new Exception();

            localName = uri.Substring(lastIndex+1);

            return uri.Substring(0, lastIndex);
        }

        public static string ToJson(this IGraph g)
        {       
            return string.Format("{{ {0} }}",
                string.Join(","+Environment.NewLine,
                    g.GetAllSubjects().Select(s => string.Format("\"{0}\" : {{ {1} }}",
                        s,
                        string.Join(" , ",
                        g.GetTriplesWithSubject(s, Tuple.Create).GroupBy(po=> po.Item1).Select(pGroup =>
                            pGroup.Count() > 1
                                ? string.Format("\"{0}\" : [{1}]", pGroup.Key,
                                    string.Join("," + Environment.NewLine, pGroup.Select(t => t.Item2).Select(ToJson)))
                                : string.Format("\"{0}\" : {1}", pGroup.Key, ToJson(pGroup.First().Item2))))))));
        }

        public static string ToJson(this ObjectVariants b)
        {
            if (b is IIriNode)
            {
                return "{ \"type\" : \"uri\", \"value\" : \"" + b +  "\" }";
            }
            else if (b is ILiteralNode)
            {
                var literalNode = ((ILiteralNode) b);
                if (literalNode is ILanguageLiteral)
                {
                    return "{ \"type\" : \"literal\", \"value\" : \"" + literalNode.Content +
                         "\", \"xml:lang\": \"" + ((ILanguageLiteral)literalNode).Lang + "\" }";
                }
                else if (literalNode is IStringLiteralNode)
                {
                    return "{ \"type\" : \"literal\", \"value\" : \"" + literalNode.Content + "\" }";
                }
                else 
                {
                    return "{ \"type\" : \"literal\", \"value\" : \"" + literalNode.Content +
                           "\", \"datatype\": \"" + literalNode.DataType + "\" }";
                }
            }
            else if (b is IBlankNode)
            {
                return "{ \"type\" : \"bnode\", \"value\" : \"" + b + "\" }";
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public static string ToTurtle(this IGraph g)
        {
            return
                string.Join("." + Environment.NewLine,
                    g.GetAllSubjects().Select(s =>
                        string.Format("{0} {1}", s,
                        string.Join(";" + Environment.NewLine,
                            g.GetTriplesWithSubject(s, Tuple.Create).GroupBy(po => po.Item1).Select(pGroup =>
                                string.Format("{0} {1}", pGroup.Key,
                                    string.Join("," + Environment.NewLine, pGroup.Select(t => t.Item2))))))));
        }

        public static void FromXml(this IGraph g, XElement x)
        {
            throw new NotImplementedException();
        }

        public static XElement ToXml(this IGraph g, Prologue prolog)
        {
            XNamespace rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
            var prefixes = new Dictionary<string, string>();

            var RDF = new XElement(rdf + "RDF");
            int i = 0;
            foreach (var s in g.GetAllSubjects())
            {
                XAttribute id = null;
                if (s is IIriNode)
                {
                    id = new XAttribute(rdf + "about", ((ObjectVariants) s).Content);
                }
                else if (s is IBlankNode)
                {
                    id = new XAttribute(rdf + "nodeID", s.ToString());
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
                RDF.Add(new XElement(rdf + "Description", id,
                    g.GetTriplesWithSubject(s, (predicate, @object) =>
                    {
                        string p;
                        NamespaceLocalName ns = Prologue.SplitUri(predicate.Content);
                        //if (!prefixes.TryGetValue(ns.@namespace, out p))
                        //{ 
                        //    RDF.Add);
                        //}
                        var x = new XElement(XName.Get("ns", ns.localname), new XAttribute(XNamespace.Xmlns + "ns", ns.@namespace));
                        if (@object is IIriNode)
                        {
                            x.Add(new XAttribute(rdf + "resource", predicate.Content));
                        }
                        else if (@object is ILiteralNode)
                        {
                            ILiteralNode ol = ((ILiteralNode) @object);
                            if (ol is ILanguageLiteral)
                                x.Add(new XAttribute(XNamespace.Xml + "lang",
                                    ((ILanguageLiteral) @object).Lang));
                            else if (!(ol is IStringLiteralNode))
                                x.Add(new XAttribute(rdf + "datatype", ol.DataType));

                            x.Add(ol.Content);
                        }
                        else if (@object is IBlankNode)
                        {
                            x.Add(new XAttribute(rdf + "nodeID", @object.ToString()));
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        return x;
                    })));
            };

            return RDF; 
        }
    }
}