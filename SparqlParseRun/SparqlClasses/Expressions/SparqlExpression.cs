using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public abstract class SparqlExpression
    {
        public Func<SparqlResult, ObjectVariants> Func;
        public bool IsAggragate;
        public bool IsDistinct;
        private ExpressionType type;

        
        internal virtual void EqualsExpression(SparqlExpression value, RdfQuery11Translator q)
        {
            IsDistinct = IsDistinct || value.IsDistinct;
            IsAggragate = IsAggragate || value.IsAggragate;  
            var funkClone = FunkClone;   
            Func = result =>
            {
                var firstVar = this as SparqlVarExpression;
                var secondVar = value as SparqlVarExpression;
                ObjectVariants firstVarValue;
                SparqlVariableBinding secondVarValue;
                var firstsKnowns = firstVar == null || result.ContainsKey(firstVar.Variable);
                var secondsKnowns = secondVar == null || result.ContainsKey(secondVar.Variable);
                if (firstsKnowns)
                {               
                    if (secondsKnowns) return new OV_bool(funkClone(result).Equals(value.Func(result)));
                    else throw new Exception();
                    result.Add(secondVar.Variable,
                         funkClone(result));
                    return new OV_bool(true);
                }
                else throw new Exception();
                if (secondsKnowns)
                    result.Add(firstVar.Variable,
                         value.Func(result));
                else throw new Exception();

                return new OV_bool(true);
            };
        }

        public void SetVariablesTypes(ExpressionType type)
        {
            this.type = type;
        }
        public enum ExpressionType
        {
            iri, numeric, @string, stringWithLang,
            @bool,
            BlankNode,
            stringOrWithLang,
            literal,
            withDate,
            @int
        }
        public SparqlExpression NotEquals(SparqlExpression value)
        {

            IsAggragate = IsAggragate || value.IsAggragate;
            IsDistinct = IsDistinct || value.IsDistinct;
                      var funkClone = FunkClone;    
           return new SparqlBoolExpression(result => new OV_bool(!funkClone(result).Equals(value.Func(result))));
        }

        public SparqlExpression Smaller(SparqlExpression value)
        {   
            return new SparqlBoolExpression(result =>
            {
                var l = Func(result).Content;
                var r = value.Func(result).Content;
                //todo if(l is INumLiteral && r is INumLiteral)
                return new OV_bool(l < r);
                
            })
            {                                                                  
                      IsAggragate = IsAggragate || value.IsAggragate,
                      IsDistinct = IsDistinct || value.IsDistinct
            };
        }

        public SparqlExpression Greather(SparqlExpression value)
        {
            return new SparqlBoolExpression(result =>
                new OV_bool(Func(result).Content > (value.Func(result)).Content))
            {
                IsAggragate = IsAggragate || value.IsAggragate,
                IsDistinct = IsDistinct || value.IsDistinct
            };
        }

        internal SparqlExpression SmallerOrEquals(SparqlExpression value)
        {                             
            IsAggragate = IsAggragate || value.IsAggragate;
            IsDistinct = IsDistinct || value.IsDistinct;
            SetVariablesTypes(ExpressionType.literal);
            value.SetVariablesTypes(ExpressionType.literal);
            return new SparqlBoolExpression(result => new OV_bool(Func(result).Content <= (value.Func(result).Content) ));
        }

        public SparqlExpression GreatherOrEquals(SparqlExpression value)
        {
           
            
            SetVariablesTypes(ExpressionType.literal);
            value.SetVariablesTypes(ExpressionType.literal);

            return new SparqlBoolExpression(result => new OV_bool(Func(result).Content>=(value.Func(result).Content) ))
            {
                IsAggragate = IsAggragate || value.IsAggragate,
                IsDistinct = IsDistinct || value.IsDistinct
            };
        }

        internal SparqlExpression InCollection(List<SparqlExpression> collection)
        {

            return new SparqlBoolExpression(
                Func = result => new OV_bool(collection.Any(element => element.Func(result).Content.Equals(Func(result).Content))))
            {
                IsAggragate = IsAggragate || collection.Any(element => element.IsAggragate),
                    IsDistinct = IsDistinct || collection.Any(element => element.IsDistinct)
            };
        }

        internal SparqlExpression NotInCollection(List<SparqlExpression> collection)
        {
            return new SparqlBoolExpression(
                  Func = result => new OV_bool(collection.Any(element => element.Func(result).Content.Equals(Func(result).Content))))
            {
                IsAggragate = IsAggragate || collection.Any(element => element.IsAggragate),
                IsDistinct = IsDistinct || collection.Any(element => element.IsDistinct)
            };
        }

        public static SparqlExpression operator +(SparqlExpression l, SparqlExpression r)
        {
            var funkClone =  l.FunkClone;
            l.SetVariablesTypes(ExpressionType.numeric);
            r.SetVariablesTypes(ExpressionType.numeric);
            l.Func = result => funkClone(result).Change(o =>o + r.Func(result).Content);
            l.IsAggragate = l.IsAggragate || r.IsAggragate;
            l.IsDistinct = l.IsDistinct || r.IsDistinct;
            return l;
        }

        public static SparqlExpression operator -(SparqlExpression l, SparqlExpression r)
        {
            var funkClone = l.FunkClone;
            l.SetVariablesTypes(ExpressionType.numeric); 
            r.SetVariablesTypes(ExpressionType.numeric);

            l.Func = result => funkClone(result).Change(o => o - r.Func(result).Content);
            l.IsAggragate = l.IsAggragate || r.IsAggragate;
            l.IsDistinct = l.IsDistinct || r.IsDistinct;
            return l;
        }

        public static SparqlExpression operator *(SparqlExpression l, SparqlExpression r)
        {
            var funkClone = l.FunkClone;
            l.SetVariablesTypes(ExpressionType.numeric);
            r.SetVariablesTypes(ExpressionType.numeric);
            l.Func = result => funkClone(result).Change(o => o * r.Func(result).Content);

            l.IsAggragate = l.IsAggragate || r.IsAggragate;
            l.IsDistinct = l.IsDistinct || r.IsDistinct;
            return l;          
        }

        public static SparqlExpression operator /(SparqlExpression l, SparqlExpression r)
        {
            var funkClone = l.FunkClone;
            l.SetVariablesTypes(ExpressionType.numeric);
            r.SetVariablesTypes(ExpressionType.numeric);
            l.Func = result => funkClone(result).Change(o => o / r.Func(result).Content);
            l.IsAggragate = l.IsAggragate || r.IsAggragate;
            l.IsDistinct = l.IsDistinct || r.IsDistinct;
            return l;
        }

        public static SparqlExpression operator !(SparqlExpression e)
        {
            var funkClone = e.FunkClone;
            e.Func = result => funkClone(result).Change(o=>!o);
            return e;
        }

        public static SparqlExpression operator -(SparqlExpression e)
        {
            e.SetVariablesTypes(ExpressionType.numeric);

            var funkClone = e.FunkClone;

            e.Func = result => funkClone(result).Change(o=>-o);
            return e;
        }

        internal bool Test(SparqlResult result)
        {
            return ((OV_bool)Func(result)).value;
        }

        public Func<SparqlResult, ObjectVariants> FunkClone
        {
            get
            {
                return (Func<SparqlResult, ObjectVariants>)Func.Clone();
            }
        }
}

    public class SparqlBoolExpression : SparqlExpression
    {
        public SparqlBoolExpression(Func<SparqlResult, ObjectVariants> func)
        {
            Func = func;
               SetVariablesTypes(ExpressionType.@bool);
        }
    }
}
