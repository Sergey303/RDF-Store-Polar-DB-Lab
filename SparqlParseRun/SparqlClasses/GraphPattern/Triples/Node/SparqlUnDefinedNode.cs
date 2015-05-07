using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node
{
    public class SparqlUnDefinedNode : ObjectVariants

    {
        public override ObjectVariantEnum Variant
        {
            get { throw new NotImplementedException(); }
        }

        public override object WritableValue
        {
            get { throw new NotImplementedException(); }
        }

        public override dynamic Content
        {
            get { throw new NotImplementedException(); }
        }

        public override ObjectVariants Change(Func<dynamic, dynamic> changing)
        {
            throw new NotImplementedException();
        }
    }
}