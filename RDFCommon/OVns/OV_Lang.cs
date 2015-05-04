using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFCommon.OVns
{
    class OV_Lang     :ObjectVariants
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
