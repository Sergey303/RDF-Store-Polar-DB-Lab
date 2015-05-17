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
            SparqlExpression arg = this.sparqlArgs[0];
            if (Equals(sparqlUriNode, SpecialTypesClass.Bool))
            {
                Func<object, bool> f = o =>
                {
                    if (o is string)
                        return bool.Parse((string)o);
                    if (o is double || o is int || o is float || o is decimal)
                        return Math.Abs(Convert.ToDouble(o)) > 0;
                    if (o is bool)
                        return (bool)o;
                    throw new ArgumentException();
                };
                if (arg.Const != null)
                    Const = new OV_bool(f(arg.Const.Content));
                else
                {
                    Operator = result => f(arg.Operator(result));
                    //todo
                }
                   return;
            }  else
                if (Equals(sparqlUriNode, SpecialTypesClass.Double))
                {

                    Func<object, double> f = o =>
                    {
                            if (o is string)
                                return double.Parse(((string)o).Replace(".", ","));
                        if (o is double || o is int || o is float || o is decimal)
                            return (Convert.ToDouble(o));
                        throw new ArgumentException();
                    };
                    if (arg.Const != null)
                        Const = new OV_double(f(arg.Const.Content));
                    else
                    {
                        Operator = result => f(arg.Operator(result));
                        //todo
                    }
                    return;
                }
                else
                    if (Equals(sparqlUriNode, SpecialTypesClass.Float))
                    {
                        TypedOperator = result =>
                        {
                            dynamic o = (arg.TypedOperator(result)).Content;
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
                            TypedOperator = result =>
                            {
                                dynamic o = (arg.TypedOperator(result)).Content;
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
                                TypedOperator = result =>
                                {
                                    dynamic o = (arg.TypedOperator(result)).Content;
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
                                    TypedOperator = result =>
                                    {
                                        ObjectVariants ov = (arg.TypedOperator(result));
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
                                    TypedOperator = result => new OV_string(sparqlArgs[0].TypedOperator(result).Content.ToString());
                                    return;
                                }
                                else
            throw new NotImplementedException("mathod call " + sparqlUriNode);
        }

      //  internal readonly Func<SparqlResult, dynamic> Func;
    }
}
