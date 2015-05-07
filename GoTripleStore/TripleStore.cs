using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;

namespace GoTripleStore
{
    public class TripleStore
    {
        protected GaGraphStringBased g;
        public TripleStore(GaGraphStringBased g)
        {
            this.g = g;
        }
        public virtual IEnumerable<string> GetSubjectByObjPred(string obj, string pred) 
        {
            var qu = g.GetTriplesWithPredicateObject(pred, new OV_iri(obj));
            var qu2 = qu.Select(ent => (string)g.Dereference(ent)[0]);
            return qu2;
        }
        public virtual IEnumerable<string> GetObjBySubjPred(string subj, string pred) 
        {
            var qu = g.GetTriplesWithSubjectPredicate(subj, pred);
            return qu.Select(ent => ((OV_iri)g.Dereference(ent)[2].ToOVariant()).Name);
        }
        public virtual IEnumerable<ObjectVariants> GetDataBySubjPred(string subj, string pred) 
        {
            var qu = g.GetTriplesWithSubjectPredicate(subj, pred);
            return qu.Select(ent =>
            {
                var v = g.Dereference(ent)[2].ToOVariant();
                return v;
            });
        }
        public virtual bool ChkOSubjPredObj(string subj, string pred, string obj) { throw new NotImplementedException(); }

    }
    public class TripleStore_Diag : TripleStore
    {
        private List<object> solutions = new List<object>();
        private int position = 0;
        private bool accumulate = false;
        private bool use = false;
        public void StartAccumulate()
        {
            use = false;
            accumulate = true;
            solutions = new List<object>();
        }
        public void StartUse()
        {
            accumulate = false;
            use = true;
            position = 0;
        }

        //private GaGraphStringBased g;
        public TripleStore_Diag(GaGraphStringBased g) : base(g)
        {
            
        }
        public override IEnumerable<string> GetSubjectByObjPred(string obj, string pred)
        {
            string[] solution = null;
            if (use)
            {
                solution = (string[])solutions[position];
                position++;
            }
            else
            {
                var qu = g.GetTriplesWithPredicateObject(pred, new OV_iri(obj));
                var qu2 = qu.Select(ent => (string)g.Dereference(ent)[0]);
                solution = qu2.ToArray();
                if (accumulate) solutions.Add(solution);
            }
            return solution;
        }
        public override IEnumerable<string> GetObjBySubjPred(string subj, string pred)
        {
            string[] solution = null;
            if (use)
            {
                solution = (string[])solutions[position];
                position++;
            }
            else
            {
                var qu = g.GetTriplesWithSubjectPredicate(subj, pred);
                var qu2 = qu.Select(ent => ((OV_iri)g.Dereference(ent)[2].ToOVariant()).Name);
                solution = qu2.ToArray();
                if (accumulate) solutions.Add(solution);
            }
            return solution;
        }
        public override IEnumerable<ObjectVariants> GetDataBySubjPred(string subj, string pred)
        {
            ObjectVariants[] solution = null;
            if (use)
            {
                solution = (ObjectVariants[])solutions[position];
                position++;
            }
            else
            {
                var qu = g.GetTriplesWithSubjectPredicate(subj, pred);
                var qu2 = qu.Select(ent =>
                {
                    var v = g.Dereference(ent)[2].ToOVariant();
                    return v;
                });
                solution = qu2.ToArray();
                if (accumulate) solutions.Add(solution);
            }
            return solution;
        }
        public override bool ChkOSubjPredObj(string subj, string pred, string obj) { throw new NotImplementedException(); }

    }
}
