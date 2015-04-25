using System;
using RDFCommon;
using RDFTripleStore.OVns;
using RDFTripleStore.parsers.RDFTurtle;

namespace RDFTripleStore
{
    public class TriplesGenerator : IGenerator<Triple<string, string, ObjectVariants>>
    {
        private readonly Parser parser;

        public TriplesGenerator(string path, string graphName)
        {
            parser = new Parser(
                new Scanner(path))
            {
                graphName = graphName
            };
            
        }


        public void Start(Action<Triple<string, string, ObjectVariants>> onGenerate)
        {
            parser.ft = (s, s1, arg3) => onGenerate(new Triple<string, string, ObjectVariants>(s, s1, arg3));
            parser.Parse();
        }
    }
}
