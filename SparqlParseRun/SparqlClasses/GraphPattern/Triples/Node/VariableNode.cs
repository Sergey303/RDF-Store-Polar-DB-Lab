using System;
using RDFCommon;
using RDFCommon.OVns;


namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node
{
    public class VariableNode : ObjectVariants, IVariableNode
    {
        public readonly string VariableName;
        private readonly int index;
        //public NodeType NodeType { get { return NodeType.Variable; } }
        //public INode Value;
        //  public int index;

        public VariableNode(string variableName, int index)
        {
            VariableName = variableName;
            this.index = index;
        }

      

        public override dynamic Content
        {
            get { throw new NotImplementedException(); }
        }

        public override ObjectVariants Change(Func<dynamic, dynamic> changing)
        {
            throw new NotImplementedException();
        }

        public override ObjectVariantEnum Variant
        {
            get { throw new NotImplementedException(); }
        }

        public override object WritableValue
        {
            get { throw new NotImplementedException(); }
        }

        public int Index
        {
            get { return index; }
        }
    }
}