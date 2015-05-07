using System;
using System.IO;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;

namespace RDFTurtleParser
{
    public class TriplesGenerator : IGenerator<TripleStrOV>
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

        public TriplesGenerator(Stream baseStream, string graphName)
        {
            parser = new Parser(
                 new Scanner(baseStream))
            {
                graphName = graphName
            };
        }


        public void Start(Action<TripleStrOV> onGenerate)
        {
            parser.ft = (s, s1, arg3) => onGenerate(new TripleStrOV(s, s1, arg3));
            parser.Parse();
        }
    }
}
