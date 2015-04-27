using System;
using System.Collections.Generic;

namespace RDFTripleStore.Comparer
{
    public class Comparer : IComparer<Comparer>, IComparable<Comparer>, IComparable
    {
        protected readonly IComparable k1;

        public Comparer(IComparable k1)
        {
            this.k1 = k1;
        }

        public int Compare(Comparer x, Comparer y)
        {
            return x.CompareTo(y);
        }

        public int CompareTo(Comparer other)
        {
            var c1 = k1.CompareTo(other.k1);
            return c1 == 0 ? 0 : c1 > 0 ? 1 : -1;
        }

        public virtual int CompareTo(object obj)
        {
            if (obj is Comparer) return (this).CompareTo((Comparer)obj);
            throw new ArgumentException();
        }
    }
}