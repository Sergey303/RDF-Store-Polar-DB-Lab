using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;

namespace GoTripleStore
{
    public class TripleStore
    {
        private GaGraphStringBased g;
        public TripleStore(GaGraphStringBased g)
        {
            this.g = g;
        }
        public IEnumerable<string> GetSubjectByObjPred(string obj, string pred) 
        {
            var qu = g.GetTriplesWithPredicateObject(pred, new OV_iri(obj)).ToArray();
            //return qu.Select(ent => (string)g.Dereference(ent)[0]);
            var qu2 = qu.Select(ent => (string)g.Dereference(ent)[0]).ToArray();
            return qu2;
        }
        public IEnumerable<string> GetObjBySubjPred(string subj, string pred) 
        {
            var qu = g.GetTriplesWithSubjectPredicate(subj, pred);
            return qu.Select(ent => ((OV_iri)g.Dereference(ent)[2].ToOVariant()).Name);
        }
        public IEnumerable<ObjectVariants> GetDataBySubjPred(string subj, string pred) 
        {
            var qu = g.GetTriplesWithSubjectPredicate(subj, pred).ToArray();
            return qu.Select(ent =>
            {
                var v = g.Dereference(ent)[2].ToOVariant();
                return v;
            });
        }
        public bool ChkOSubjPredObj(string subj, string pred, string obj) { throw new NotImplementedException(); }

    }
}
