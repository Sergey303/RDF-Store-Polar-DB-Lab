using System;

namespace RDFTripleStore.Comparer
{
    public class SP_Pair : IComparable
    {
        int s, p;
        public SP_Pair(int subject, int predicate) { this.s = subject; this.p = predicate; }
        //int S { get; set; }
        //int P { get; set; }
        public int CompareTo(object another)
        {
            SP_Pair ano = (SP_Pair)another;
            int cmp = this.GetHashCode().CompareTo(ano.GetHashCode());
            if (cmp == 0)
            {
                cmp = this.s.CompareTo(ano.s);
            }
            if (cmp == 0)
            {
                cmp = this.p.CompareTo(ano.p);
            }
            return cmp;
        }
        public override int GetHashCode()
        {
            //return s.GetHashCode() ^ p.GetHashCode();
            return (3001 ^ s.GetHashCode()) * (1409 ^ p.GetHashCode());
        }
    }
}