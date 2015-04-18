using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFTripleStore
{
   public class RamGraph   :IGraph<string, string, ObjectVariants>
    {
        private List<Triple<string, string, ObjectVariants>> triples;

        public virtual void Build(IEnumerable<Triple<string, string, ObjectVariants>> triples)
        {
            this.triples = triples.ToList();
        }

        public virtual IEnumerable<Triple<string, string, ObjectVariants>> Search(string subject = null, string predicate = null, ObjectVariants obj = null)
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
