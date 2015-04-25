using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RDFCommon;

using RDFTripleStore;
using RDFTripleStore.OVns;


namespace SparqlTesting
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
   public class Graph :IGraph
   {
       IGraph<Triple<string, string, ObjectVariants>> realGraph;

       public Graph(string path)
       {
                               realGraph=new GoGraphStringBased(path);
       }

       public IGraphNode Name { get; private set; }
       public INodeGenerator NodeGenerator { get; private set; }
       public void Clear()
       {
           throw new NotImplementedException();
       }

       public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithObject(IObjectNode n)
       {
           throw new NotImplementedException();
       }

       public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithPredicate(IPredicateNode n)
       {
           throw new NotImplementedException();
       }

       public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriplesWithSubject(ISubjectNode n)
       {
           throw new NotImplementedException();
       }

       public IEnumerable<IObjectNode> GetTriplesWithSubjectPredicate(ISubjectNode subj, IPredicateNode pred)
       {
           throw new NotImplementedException();
       }

       public IEnumerable<IPredicateNode> GetTriplesWithSubjectObject(ISubjectNode subj, IObjectNode obj)
       {
           throw new NotImplementedException();
       }

       public IEnumerable<ISubjectNode> GetTriplesWithPredicateObject(IPredicateNode pred, IObjectNode obj)
       {
           throw new NotImplementedException();
       }

       public IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> GetTriples()
       {
           throw new NotImplementedException();
       }

       public void Add(ISubjectNode s, IPredicateNode p, IObjectNode o)
       {
           throw new NotImplementedException();
       }

       public void Insert(IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples)
       {
           throw new NotImplementedException();
       }

       public void Add(Triple<ISubjectNode, IPredicateNode, IObjectNode> t)
       {
           throw new NotImplementedException();
       }

       public bool Contains(ISubjectNode subject, IPredicateNode predicate, IObjectNode node)
       {
           throw new NotImplementedException();
       }

       public void Delete(IEnumerable<Triple<ISubjectNode, IPredicateNode, IObjectNode>> triples)
       {
           throw new NotImplementedException();
       }

       public IEnumerable<ISubjectNode> GetAllSubjects()
       {
           throw new NotImplementedException();
       }

       public long GetTriplesCount()
       {
           throw new NotImplementedException();
       }

       public bool Any()
       {
           throw new NotImplementedException();
       }

       public void FromTurtle(string gString)
       {
           throw new NotImplementedException();
       }
   }
}
