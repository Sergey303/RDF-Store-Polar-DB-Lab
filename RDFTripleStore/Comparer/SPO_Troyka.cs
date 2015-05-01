using System;
using RDFCommon.OVns;

namespace RDFTripleStore.Comparer
{
    public class SPO_Troyka : IComparable
    {
        int s, p; ObjectVariants ov;
        public SPO_Troyka(int subject, int predicate, ObjectVariants ov) { this.s = subject; this.p = predicate; this.ov = ov; }
        public int CompareTo(object another)
        {
            SPO_Troyka ano = (SPO_Troyka)another;
            int cmp = this.GetHashCode().CompareTo(ano.GetHashCode());
            if (cmp == 0)
            {
                cmp = this.s.CompareTo(ano.s);
            }
            if (cmp == 0 )
            {
                cmp = this.p.CompareTo(ano.p);
            }
            if (cmp == 0 )
            {
                cmp = this.ov.CompareTo(ano.ov);
            }
            return cmp;
        }
        public override int GetHashCode()
        {
            return (2 ^ s.GetHashCode()) * (3 ^ p.GetHashCode()) * (7 ^ ov.GetHashCode());
        }
    }
}