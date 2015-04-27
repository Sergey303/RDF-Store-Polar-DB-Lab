using System;
using System.Collections.Generic;

namespace RDFTripleStore
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

        public int CompareTo(object obj)
        {
            if (obj is Comparer) return (this).CompareTo((Comparer)obj);
            throw new ArgumentException();
        }
    }

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

        public new int CompareTo(object obj)
        {
           // if (obj is Comparer3) return this.CompareTo((Comparer3)obj);
            if (obj is Comparer2) return ((Comparer2)this).CompareTo((Comparer2)obj);
            if (obj is Comparer) return ((Comparer)this).CompareTo((Comparer)obj);
            throw new ArgumentException();
        }
    }

    public class Comparer3 : Comparer2, IComparer<Comparer3>, IComparable<Comparer3>, IComparable, IComparer<IComparable>
    {

        private readonly IComparable k3;

        public Comparer3(IComparable k1, IComparable k2, IComparable k3)
            : base(k1, k2)
        {
            this.k3 = k3;
        }

  

        public int CompareTo(Comparer3 other)
        {
            int compareK1 = k1.CompareTo(other.k1);
            if (compareK1 != 0) return compareK1 > 0 ? 1 : -1;
            else
            {
                int compareK2 = k2.CompareTo(other.k2);
                if (compareK2 != 0) return compareK2 > 0 ? 1 : -1;
                else
                {
                    int compareK3 = k3.CompareTo(other.k3);
                    return compareK3 == 0 ? 0 : compareK3 > 0 ? 1 : -1;
                }
            }
        }

        public new int CompareTo(object obj)
        {
            if (obj is Comparer3) return this.CompareTo((Comparer3) obj);
            if (obj is Comparer2) return ((Comparer2) this).CompareTo((Comparer2) obj);
            if (obj is Comparer) return ((Comparer)this).CompareTo((Comparer)obj);
            throw new ArgumentException();
        }
    //________________________________Comparer____________________________________________________
        public static Comparer3 Comparer = new Comparer3(null, null, null);
        public int Compare(Comparer3 x, Comparer3 y)
        {
            return x.CompareTo(y);
        }
        public int Compare(IComparable x, IComparable y)
        {
            if (x is Comparer3) return ((Comparer3) x).CompareTo(y);
            if (x is Comparer2) return ((Comparer2)x).CompareTo(y);
            return x.CompareTo(y);
        }
    }
}
