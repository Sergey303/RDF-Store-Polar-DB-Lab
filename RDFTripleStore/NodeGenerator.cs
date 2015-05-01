using System;
using RDFCommon;
using RDFCommon.OVns;

namespace RDFTripleStore
{
    public class NodeGenerator:INodeGenerator
    {
        public NodeGenerator()
        {
            SpecialTypes=new SpecialTypesClass(this);
        }
        public IIriNode CreateUriNode(string uri)
        {
            return new OV_iri(uri.ToLowerInvariant()); 
        }

        public IIriNode CreateUriNode(UriPrefixed uri)
        {
            return new OV_iri(uri.FullName.ToLowerInvariant());
        }

        public ILiteralNode CreateLiteralNode(string p)
        {
            return new OV_string(p);
        }

        public ILiteralNode CreateLiteralWithLang(string s, string lang)
        {
            s = s.Trim('"', '\'');
            return new OV_langstring(s, lang); 
        }


        public ILiteralNode CreateLiteralNode(int parse)
        {
            return new OV_int(parse);//SimpleLiteralNode

        }

        public ILiteralNode CreateLiteralNode(decimal p)
        {
            return new OV_decimal(p);//SimpleLiteralNode
        }

        public ILiteralNode CreateLiteralNode(double p)
        {
            return new OV_double(p);//SimpleLiteralNode
        }

        public ILiteralNode CreateLiteralNode(bool p)
        {
            return new OV_bool(p);// ? BoolLiteralNode.TrueNode((SpecialTypes.Bool)) : BoolLiteralNode.FalseNode((SpecialTypes.Bool));
        }

    

        public IBlankNode CreateBlankNode(IGraphNode graphName, string blankNodeString = null)
        {
            return new OV_iri(CreateBlankNode(((IIriNode) graphName).UriString, blankNodeString));
        }

        public IIriNode GetUri(string uri)
        {
          return new OV_iri(uri);
        }

        public SpecialTypesClass SpecialTypes { get; private set; }
        public IIriNode GetUriNode(UriPrefixed uriPrefixed)
        {
            return GetUri(uriPrefixed.FullName);
        }

        public ILiteralNode CreateLiteralNode(string p, IIriNode typeUriNode)
        {
            p = p.Trim('"','\'');
            
            if (typeUriNode == this.SpecialTypes.@string)
                return new OV_string(p);
            else if (typeUriNode.Equals(this.SpecialTypes.date))
            {
                DateTime date;
                if(!DateTime.TryParse(p, out date)) throw new ArgumentException(p);
                return new OV_date(date);
            }
            else if (typeUriNode.Equals(this.SpecialTypes.dateTime))
            {
                DateTimeOffset date;
                if (!DateTimeOffset.TryParse(p, out date)) throw new ArgumentException(p);
                return new OV_dateTimeZone(date);
            }
            else if (typeUriNode .Equals(this.SpecialTypes.@bool))
            {
                bool b;
                if (!bool.TryParse(p, out b)) throw new ArgumentException(p);
                return new OV_bool(b);
            }
            else if (typeUriNode.Equals(SpecialTypes.@decimal))
            {
                decimal d;
                if (!decimal.TryParse(p.Replace(".", ","), out d)) throw new ArgumentException(p);
             return new OV_decimal(d);
            }                                                    
            else if (typeUriNode .Equals(SpecialTypes.@double))
            {
                double d;
                if (!double.TryParse(p.Replace(".", ","), out d)) throw new ArgumentException(p);
                return new OV_double(d);
            }
            else if (typeUriNode .Equals(SpecialTypes.@float) )
            {
                float f;
                if (!float.TryParse(p.Replace(".",","), out f)) throw new ArgumentException(p);
            return new OV_float(f);
            }
            else if (typeUriNode .Equals( SpecialTypes.integer)    )
            {
                int i;
                if (!int.TryParse(p, out i)) throw new ArgumentException(p);
                return new OV_int(i);
            }
            //else if (typeUriNode .Equals( SpecialTypes.DayTimeDuration.FullName)
            //{
            //    TimeSpan i;
            //    if (!TimeSpan.TryParse(p, out i)) throw new ArgumentException(p);
            //    return new ObjectVariant(11,i);
            //}
            else 
            return new OV_typed(p, typeUriNode.UriString );   
        }

        public string CreateBlankNode(string graph, string blankNodeString = null)
        {
            if (graph != null) blankNodeString = graph + "/" + blankNodeString;
            if(blankNodeString==null)
                blankNodeString = "blank" + (long)(random.NextDouble() * 1000 * 1000 * 1000 * 1000);

          
            return blankNodeString;
        }

        private Random random = new Random();


    
    }
    
}
