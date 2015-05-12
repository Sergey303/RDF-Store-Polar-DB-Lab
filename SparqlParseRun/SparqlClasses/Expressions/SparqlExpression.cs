using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public abstract class SparqlExpression
    {
        public Func<SparqlResult, ObjectVariants> TypedOperator;
        public Func<SparqlResult, dynamic> Operator;
        public bool IsAggragate;
        public bool IsDistinct;
        private ExpressionTypeEnum typeEnum;
        private ObjectVariantEnum? realType;
        private ObjectVariants @constconst;


        internal static SparqlExpression EqualsExpression(SparqlExpression l, SparqlExpression r, RdfQuery11Translator q)
        {
            var sparqlBinaryExpression = new SparqlBinaryExpression(l, r, (o, o1) => o.Equals(o1));
            l.SyncTypes(r);
            sparqlBinaryExpression.SetExprType(ObjectVariantEnum.Bool);
            return sparqlBinaryExpression;
        }


        public static SparqlExpression NotEquals(SparqlExpression l, SparqlExpression r)
        {
            var sparqlBinaryExpression = new SparqlBinaryExpression(l, r, (o, o1) => ! o.Equals(o1));
            l.SyncTypes(r);
            sparqlBinaryExpression.SetExprType(ObjectVariantEnum.Bool);
            return sparqlBinaryExpression;
        }

        public void SyncTypes(SparqlExpression other)
        {
            
        }

        public static SparqlExpression Smaller(SparqlExpression l, SparqlExpression r)
        {
            var sparqlBinaryExpression = new SparqlBinaryExpression(l, r, (o, o1) => ((IComparable)o).CompareTo(o1) < 0 );
            l.SyncTypes(r);
            sparqlBinaryExpression.SetExprType(ObjectVariantEnum.Bool);
            return sparqlBinaryExpression;
        }
        


        public static SparqlExpression Greather(SparqlExpression l, SparqlExpression r)
        {
            Func<object, object, object> @operator;
            //if (l.RealType == ObjectVariantEnum.Int || r.RealType == ObjectVariantEnum.Int)
            //    @operator = (o, o1) => (int)o > (int)o1;
            //else 
            //    if (l.RealType == ObjectVariantEnum.Date|| r.RealType == ObjectVariantEnum.Date)
            //    @operator = (o, o1) => (DateTimeOffset)o > (DateTimeOffset)o1;
            //else                                                          throw new NotImplementedException();
            var sparqlBinaryExpression = new SparqlBinaryExpression(l, r, (o, o1) => ((IComparable)o).CompareTo(o1) > 0);
            sparqlBinaryExpression.SetExprType(ObjectVariantEnum.Bool);
            return sparqlBinaryExpression;
        }

        internal static SparqlExpression SmallerOrEquals(SparqlExpression l, SparqlExpression r)
        {
            var sparqlBinaryExpression = new SparqlBinaryExpression(l, r, (o, o1) => ((IComparable)o).CompareTo(o1) <= 0);
            sparqlBinaryExpression.SetExprType(ObjectVariantEnum.Bool);
            return sparqlBinaryExpression;
        }

        public static SparqlExpression GreatherOrEquals(SparqlExpression l, SparqlExpression r)
        {
            var sparqlBinaryExpression = new SparqlBinaryExpression(l, r, (o, o1) => ((IComparable)o).CompareTo(o1) >=0 );
            sparqlBinaryExpression.SetExprType(ObjectVariantEnum.Bool);
            return sparqlBinaryExpression;
        }

        internal SparqlExpression InCollection(List<SparqlExpression> collection)
        {
            var inCollection = new SparqlBoolExpression(
                result =>
                {
                    var o = Operator(result);
                    return collection.Any(element => element.Operator(result).Equals(o));
                })
            {
                IsAggragate = IsAggragate || collection.Any(element => element.IsAggragate),
                IsDistinct = IsDistinct || collection.Any(element => element.IsDistinct),
            };
            if (Const != null)
            {
                var cConsts = collection.Select(expression => expression.Const).ToArray();
                if (cConsts.Contains(Const))
                    inCollection.Const = new OV_bool(true);
                else if (cConsts.All(c => c != null)) inCollection.Const = new OV_bool(false);
                else inCollection.Const = null;
            }
            return inCollection;
        }

        internal SparqlExpression NotInCollection(List<SparqlExpression> collection)
        {
            var notInCollection = new SparqlBoolExpression(
                result =>
                {
                    var o = Operator(result);
                    return collection.All(element => !element.Operator(result).Equals(o));
                })
            {
                IsAggragate = IsAggragate || collection.Any(element => element.IsAggragate),
                IsDistinct = IsDistinct || collection.Any(element => element.IsDistinct),
            };
            if (Const != null)
            {
                var cConsts = collection.Select(expression => expression.Const).ToArray();
                if (cConsts.Contains(Const))
                    notInCollection.Const = new OV_bool(false);
                else if (cConsts.All(c => c != null)) notInCollection.Const = new OV_bool(true);
                else notInCollection.Const = null;
            }
            return notInCollection;
        }

        public static SparqlExpression operator +(SparqlExpression l, SparqlExpression r)
        {
            l.SetExprType(ExpressionTypeEnum.numeric);
            var sparqlBinaryExpression = new SparqlBinaryExpression(l, r, (o, o1) =>
            {
                if(o is int)
                return (int) o + (int) o1;
                if(o is decimal)
                    return (decimal)o + (decimal)o1;
                throw new Exception();
            });
            sparqlBinaryExpression.SetExprType(l);            
            return sparqlBinaryExpression;
        }

        public static SparqlExpression operator -(SparqlExpression l, SparqlExpression r)
        {
            l.SetExprType(ExpressionTypeEnum.numeric);
            var sparqlBinaryExpression = new SparqlBinaryExpression(l, r, (o, o1) =>
            {
                if (o is int)
                    return (int)o - (int)o1;
                if (o is decimal)
                    return (decimal)o - (decimal)o1;
                throw new Exception();
            });
            sparqlBinaryExpression.SetExprType(l);
            return sparqlBinaryExpression;
        }

        public static SparqlExpression operator *(SparqlExpression l, SparqlExpression r)
        {
            l.SetExprType(ExpressionTypeEnum.numeric);
            var sparqlBinaryExpression = new SparqlBinaryExpression(l, r, (o, o1) => (int)o * (int)o1);
            sparqlBinaryExpression.SetExprType(l);
            return sparqlBinaryExpression;   
        }

        public static SparqlExpression operator /(SparqlExpression l, SparqlExpression r)
        {
            l.SetExprType(ExpressionTypeEnum.numeric);
            //but xsd:decimal if both operands are xsd:integeк

            var sparqlBinaryExpression = new SparqlBinaryExpression(l, r, (o, o1) => (int)o / (int)o1);
            sparqlBinaryExpression.SetExprType(l);
            return sparqlBinaryExpression;
        }

        public static SparqlExpression operator !(SparqlExpression e)
        {
            e.SetExprType(ObjectVariantEnum.@Bool);
            var opLogicalNot = new SparqlUnaryExpression(o => !o, e);
            opLogicalNot.SetExprType(ObjectVariantEnum.Bool);

            return opLogicalNot;
        }

        public static SparqlExpression operator -(SparqlExpression e)
        {
            e.SetExprType(ExpressionTypeEnum.numeric);
            var uminus = new SparqlUnaryExpression(o => -o, e);
            uminus.SetExprType(e);
            return uminus;
        }

        internal bool Test(SparqlResult result)
        {
            return (bool)Operator(result);
        }

        public Func<SparqlResult, ObjectVariants> FunkClone
        {
            get
            {
                return (Func<SparqlResult, ObjectVariants>)TypedOperator.Clone();
            }
        }

        public ObjectVariants Const
        {
            get { return @constconst; }
            set
            {
                @constconst = value;
                realType = constconst.Variant;
            }
        }

        public ObjectVariantEnum? RealType
        {
            get
            {   
                return realType;
            }
            set
            {
                realType = value;
            }
        }

        public void SetExprType(ObjectVariantEnum variant)
        {
            RealType = variant;
        }
        public void SetExprType(ExpressionTypeEnum typeEnum)
        {
            this.typeEnum = typeEnum;
        }
        private void SetExprType(SparqlExpression variant)
        {

        }                                       

        private ExpressionTypeClass exprTypeObj;

       public class ExpressionTypeClass
       {
           private ExpressionTypeEnum generalType;
           public ObjectVariantEnum listType;
       }        
        public enum ExpressionTypeEnum
        {
             numeric, 
            stringOrWithLang,
            literal,
            Date,
        }

    }
}
