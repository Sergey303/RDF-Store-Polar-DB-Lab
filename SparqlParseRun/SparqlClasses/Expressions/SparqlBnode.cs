using System;
using RDFCommon;
using RDFCommon.OVns;


namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlBnode : SparqlExpression
    {

        public SparqlBnode(SparqlExpression value, RdfQuery11Translator q)       :base(value.AggregateLevel)
        {
            //IsDistinct = value.IsDistinct;
            //value.SetExprType(ObjectVariantEnum.Str);
            //SetExprType(ObjectVariantEnum.Iri);
            var litConst = value.Const;
            if (litConst != null)
                Const = q.Store.NodeGenerator.CreateBlankNode((string) litConst.Content);
            else
            {
                TypedOperator = result => q.Store.NodeGenerator.CreateBlankNode(value.Operator(result));
                //OvConstruction = o => q.Store.NodeGenerator.CreateBlankNode((string) o);
            }
        }

        public SparqlBnode(RdfQuery11Translator q):base(VariableDependenceGroupLevel.UndependableFunc)
        {
          //  SetExprType(ObjectVariantEnum.Iri);
            //OvConstruction = o => q.Store.NodeGenerator.CreateBlankNode(); 
           TypedOperator = result => q.Store.NodeGenerator.CreateBlankNode();
        }                      
    }
}
