using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFTripleStore
{
    class Program
    {
        static void Main(string[] args)
        {
            RamGraph graph=new RamGraph();
            graph.TestBuild(1);
            graph.TestSearch();
        }
    }
}
