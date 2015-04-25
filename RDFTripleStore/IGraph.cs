using System.Collections.Generic;

namespace RDFTripleStore
{
    //public interface IGraph<Ts,Tp,To>
    //{
    //    void Build(IEnumerable<Triple<Ts,Tp,To>> triples);
    //    IEnumerable<Triple<Ts,Tp,To>> Search(Ts subject=default(Ts), Tp predicate =default(Tp), To obj=default(To));
    //}
    public interface IGraph<Tri>
    {
        //void Build(IEnumerable<Tri> triples);
        void Build(IEnumerable<Triple<string, string, ObjectVariants>> triples);
        void Build(IGenerator<List<Triple<string, string, ObjectVariants>>> generator);
        IEnumerable<Tri> Search(object subject = null, object predicate = null, ObjectVariants obj = null);
        
    }
}