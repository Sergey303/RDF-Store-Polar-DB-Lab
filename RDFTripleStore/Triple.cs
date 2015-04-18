using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFTripleStore
{
    public struct Triple<Ts, Tp, To>
    {
        public Ts Subject;
        public Tp Predicate;
        public To Object;

        public Triple(Ts subject, Tp predicate, To o) 
        {
            Subject = subject;
            Predicate = predicate;
            Object = o;
        }
    }
}
