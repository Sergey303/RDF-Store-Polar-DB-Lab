using System.Collections.Generic;

namespace RDFTripleStore
{
    public interface IGraph<Ts,Tp,To>
    {
        void Build(IEnumerable<Triple<Ts,Tp,To>> triples);
        IEnumerable<Triple<Ts,Tp,To>> Search(Ts subject=default(Ts), Tp predicate =default(Tp), To obj=default(To));

    }
}