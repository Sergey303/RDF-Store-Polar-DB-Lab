﻿using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlSameTerm : SparqlExpression
    {
        public SparqlSameTerm(SparqlExpression str, SparqlExpression pattern)
        {
            IsAggragate = pattern.IsAggragate || str.IsAggragate;
            IsDistinct = pattern.IsDistinct || str.IsDistinct;
            Func = result => new OV_bool(str.Func(result).Equals(pattern.Func(result))); 
        }
    }
}
