﻿using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlMonth : SparqlExpression
    {
        private SparqlExpression sparqlExpression;

        public SparqlMonth(SparqlExpression value)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
            Func = result =>
            {
                var f = value.Func(result).Content;
                if (f is DateTime)
                    return new OV_int(((DateTime)f).Month);
                throw new ArgumentException();
            };
        }
    }
}