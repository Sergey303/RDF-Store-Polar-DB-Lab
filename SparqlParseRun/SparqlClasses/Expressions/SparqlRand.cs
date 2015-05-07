using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlRand : SparqlExpression
    {
        public SparqlRand()
        {
            Random r=new Random();
            Func = result => new OV_double(r.NextDouble());
        }
    }
}