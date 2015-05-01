using System;
using RDFCommon.OVns;

namespace RDFTripleStore.Comparer
{
    public class PO_Pair : IComparable
    {
        int p; ObjectVariants ov;
        public PO_Pair(int predicate, ObjectVariants ov) { this.p = predicate; this.ov = ov; }
        public int CompareTo(object another)
        {
            PO_Pair ano = (PO_Pair)another;
            int cmp = this.GetHashCode().CompareTo(ano.GetHashCode());
            if (cmp == 0)
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
            //return p.GetHashCode() + 7777 * ov.GetHashCode();
            return unchecked((2 ^ p.GetHashCode()) * (3 ^ ov.GetHashCode()));
        }
    }
}