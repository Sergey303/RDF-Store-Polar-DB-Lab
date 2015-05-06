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
  
        
        public virtual ObjectVariants GetUri(string uri)
        {
          return new OV_iri(uri);
        }

        public SpecialTypesClass SpecialTypes { get; protected set; }
   
        public ObjectVariants CreateBlankNode()
        {
            return
                new OV_iri("Http://iis.nsk.su/.well-known/genid/blank"
                              + BlankNodeGenerateNums()
                              + BlankNodeGenerateNums());
        }

        public virtual ObjectVariants AddIri(string iri)
        {
            return new OV_iri(iri);
        }

        private long BlankNodeGenerateNums()
        {
            return (long)(random.NextDouble() * 1000 * 1000 * 1000 * 1000);
        }

        public ObjectVariants CreateLiteralNode(string p, string typeUriNode)
        {
            p = p.Trim('"','\'');

            switch (typeUriNode)
            {
                case SpecialTypesClass.String:
                    return new OV_string(p);
                case (SpecialTypesClass.@Bool):
                    return new OV_bool(p);
                case (SpecialTypesClass.@Decimal):
                    return new OV_decimal(p);
                case (SpecialTypesClass.Integer):
                    return new OV_int(p);
                case (SpecialTypesClass.@Float):
                    return new OV_float(p);
                case (SpecialTypesClass.@Double):
                    return new OV_double(p);
                
                case (SpecialTypesClass.Date):
                    return new OV_date(p);
                case (SpecialTypesClass.Time):
                    return new OV_time(p);
                case (SpecialTypesClass.DateTime):
                    return new OV_dateTimeStamp(p);
                case (SpecialTypesClass.DateTimeStamp):
                    return new OV_dateTimeStamp(p);

                //case (SpecialTypesClass.GYear):
                //    return new OV_double(p);
                //case (SpecialTypesClass.GMonth):
                //    return new OV_double(p);
                //case (SpecialTypesClass.GDay):
                //    return new OV_double(p);
                //case (SpecialTypesClass.GYearMonth):
                //    return new OV_double(p);
                //case (SpecialTypesClass.GMonthDay):
                //    return new OV_double(p);
                //case (SpecialTypesClass.Duration):
                //    return new OV_double(p);
                //case (SpecialTypesClass.YearMonthDuration):
                //    return new OV_double(p);
                //case (SpecialTypesClass.DayTimeDuration):
                //    return new OV_double(p);
                
                //todo
                default:
                    return CreateLiteralOtherType(p, typeUriNode);
            }
        }

        public ObjectVariants CreateBlankNode(string graph, string blankNodeString = null)
        {
            if (graph != null) blankNodeString = graph + "/" + blankNodeString;

            return new OV_iri(blankNodeString); 
        }
        public string CreateBlank(string graph, string blankNodeString)
        {
            if (graph != null) blankNodeString = graph + "/" + blankNodeString;

            return blankNodeString;
        }
        public string CreateBlank()
        {
            return "";
        }
        private Random random = new Random();


        public virtual ObjectVariants CreateLiteralOtherType(string p, string typeUriNode)
        {
            return new OV_typed(p, typeUriNode);   
        }
    }
    
}
