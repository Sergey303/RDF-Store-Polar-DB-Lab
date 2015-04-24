using System;
using System.Collections.Generic;

namespace RDFTripleStore
{
    public class TripleGeneratorBuffered : IGenerator<List<Triple<string, string, ObjectVariants>>>
    {
        private TriplesGenerator tg;
        private List<Triple<string, string, ObjectVariants>> buffer;
        private int maxBuffer;

        public TripleGeneratorBuffered(string path, string graphName, int maxBuffer=1000)
        {
            this.maxBuffer = maxBuffer;
            buffer = new List<Triple<string, string, ObjectVariants>>();
            tg = new TriplesGenerator(path, graphName);
        }

        public void Start(Action<List<Triple<string, string, ObjectVariants>>> onGenerate)
        {
            tg.Start(
                triple =>
                {
                    buffer.Add(triple);
                    if (buffer.Count == maxBuffer)
                    {
                        onGenerate(buffer);
                        buffer=new List<Triple<string, string, ObjectVariants>>();
                    }
                })
                ;
        }
    }
}