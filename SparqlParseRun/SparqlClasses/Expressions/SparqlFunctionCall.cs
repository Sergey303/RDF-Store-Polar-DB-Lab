using System;
using RDFCommon;
using RDFCommon.OVns;


namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlFunctionCall  :SparqlExpression
    {
   //     private SparqlUriNode sparqlUriNode;
       public SparqlArgs sparqlArgs;

        public SparqlFunctionCall(string sparqlUriNode, SparqlArgs sparqlArgs, NodeGenerator q)
        {
            // TODO: Complete member initialization
           // this.sparqlUriNode = sparqlUriNode;
            this.sparqlArgs = sparqlArgs;
            IsDistinct = sparqlArgs.isDistinct;   //todo
            if (Equals(sparqlUriNode, SpecialTypesClass.Bool))
            {
              Func= result =>
                {
                    ObjectVariants ov = (this.sparqlArgs[0].Func(result));
                    dynamic o = ov.Content;
                    if (o is string)
                        return new OV_bool(bool.Parse(o)); ;
                    if (o is double || o is int || o is float || o is decimal)
                        return new OV_bool(((double)o) != 0.0d); 
                    if (o is bool)
                        return ov;
                    throw new ArgumentException();
                };
                   return;
            }  else
                if (Equals(sparqlUriNode, SpecialTypesClass.Double))
                {
                    Func = result =>
                    {
                        dynamic o = (this.sparqlArgs[0].Func(result)).Content;
                        if (o is string)
                            return new OV_double(double.Parse(o.Replace(".", ",")));
                        if (o is double || o is int || o is float || o is decimal)
                            return new OV_double(Convert.ToDouble(o));
                        throw new ArgumentException();
                    };
                    return;
                }
                else
                    if (Equals(sparqlUriNode, SpecialTypesClass.Float))
                    {
                        Func = result =>
                        {
                            dynamic o = (this.sparqlArgs[0].Func(result)).Content;
                            if (o is string)
                                 return new OV_float(float.Parse(o.Replace(".", ",")));
                            if (o is double || o is int || o is float || o is decimal)
                                return new OV_float(Convert.ToDouble(o));
                            throw new ArgumentException();
                        };
                        return;
                    }
                    else
                        if (Equals(sparqlUriNode, SpecialTypesClass.Decimal))
                        {
                            Func = result =>
                            {
                                dynamic o = (this.sparqlArgs[0].Func(result)).Content;
                                if (o is string)
                                    return new OV_decimal((decimal.Parse(o.Replace(".", ","))));
                                if (o is double || o is int || o is float || o is decimal)
                                    return new OV_decimal(Convert.ToDecimal(o));
                                throw new ArgumentException();
                            };
                            return;
                        }
                        else
                            if (Equals(sparqlUriNode, SpecialTypesClass.Int))
                            {
                                Func = result =>
                                {
                                    dynamic o = (this.sparqlArgs[0].Func(result)).Content;
                                    if (o is string)
                                        return new OV_int((int.Parse(o)));
                                    if (o is double || o is int || o is float || o is decimal)
                                        return new OV_int(Convert.ToInt32(o));
                                    throw new ArgumentException();
                                };
                                return;
                            }
                            else
                                if (Equals(sparqlUriNode, SpecialTypesClass.DateTime))
                                {
                                    Func = result =>
                                    {
                                        ObjectVariants ov = (this.sparqlArgs[0].Func(result));
                                        dynamic o = ov.Content;
                                        if (o is string)
                                            return new OV_dateTime(DateTime.Parse(o));
                                        if (o is DateTime)
                                            return ov;
                                        throw new ArgumentException();
                                    };
                                    return;
                                }
                                else if (Equals(sparqlUriNode, SpecialTypesClass.String))
                                {
                                    Func = result => new OV_string(sparqlArgs[0].Func(result).Content.ToString());
                                    return;
                                }
                                else
            throw new NotImplementedException("mathod call " + sparqlUriNode);
        }

      //  internal readonly Func<SparqlResult, dynamic> Func;
    }
}
