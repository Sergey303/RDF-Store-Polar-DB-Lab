using System.Collections.Generic;
using System.Linq;
using RDFCommon;

namespace RDFTripleStore
{
   public class RamGraph   :IGraph<Triple<string, string, ObjectVariants.ObjectVariants>>
    {
        private List<Triple<string, string, ObjectVariants.ObjectVariants>> triples;

        public virtual void Build(IEnumerable<Triple<string, string, ObjectVariants.ObjectVariants>> triples)
        {
            this.triples = triples.ToList();
        }

       public void Build(IGenerator<List<Triple<string, string, ObjectVariants.ObjectVariants>>> generator)
       {
           triples=new List<Triple<string, string, ObjectVariants.ObjectVariants>>();
           generator.Start(list =>
               triples.AddRange(list));
       }

     
       public virtual IEnumerable<Triple<string, string, ObjectVariants.ObjectVariants>> Search(object subject = null, object predicate = null, ObjectVariants.ObjectVariants obj = null)
        {
            if (subject == null && predicate == null && obj == null) return triples;
            if (predicate == null && obj == null) return triples.Where(t => t.Subject == subject);
            if (subject == null && obj == null) return triples.Where(t => t.Predicate == predicate);
            if (subject == null && predicate == null) return triples.Where(t => t.Object.Equals(obj));
            if (subject == null) return triples.Where(t => t.Object.Equals(obj) && predicate == t.Predicate);
            if (predicate == null) return triples.Where(t => t.Object.Equals(obj) && t.Subject == subject);
            if (obj == null) return triples.Where(t => t.Subject==subject && t.Predicate==predicate);
            return triples.Where(t => t.Subject == subject && t.Predicate == predicate && t.Object.Equals(obj));
        }
    }
}
