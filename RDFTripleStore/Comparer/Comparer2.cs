using System;
using System.Collections.Generic;

namespace RDFTripleStore.Comparer
{
    public class Comparer2 : Comparer, IComparer<Comparer2>, IComparable<Comparer2>
    {
        protected readonly IComparable k2;

        public Comparer2(IComparable k1, IComparable k2) : base(k1)
        {
         
            this.k2 = k2;
        }

        public int Compare(Comparer2 x, Comparer2 y)
        {
            return x.CompareTo(y);
        }

        public int CompareTo(Comparer2 other)
        {
            if (this is Comparer3 && other is Comparer3) return ((Comparer3)this).CompareTo((Comparer3)other);
            int compareK1 = k1.CompareTo(other.k1);
            if (compareK1 != 0) return compareK1 > 0 ? 1 : -1;
            else
            {
                int compareK2 = k2.CompareTo(other.k2);
                return compareK2 == 0 ? 0 : compareK2 > 0 ? 1 : -1;
            }
        }

        public override int CompareTo(object obj)
        {
            // if (obj is Comparer3) return this.CompareTo((Comparer3)obj);
            if (obj is Comparer2) return this.CompareTo((Comparer2)obj);
            if (obj is Comparer) return this.CompareTo((Comparer)obj);
            throw new ArgumentException();
        }
    }
}