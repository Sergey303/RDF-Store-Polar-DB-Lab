using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using RDFCommon;
using RDFTripleStore.OVns;

namespace RDFTripleStore
{
    /// <summary>
    /// читает RDF Turtle с помощью coco/r
    /// запускает <see cref="TripleGeneratorBuffered"/> в отдельной нити,
    /// непрерывно читает входной файл и записывает получаемые порции в Queue<List<Triple<string, string, ObjectVariants>>>
    /// в основном исполняемом потоке отслеживается Queue: если есть элементы, то они "возвращаются" механизмом выполнения указанного делегата.
    /// </summary>
    public class TripleGeneratorBufferedParallel : IGenerator<List<Triple<string, string, ObjectVariants>>>
    {
        private TripleGeneratorBuffered tg;

        public TripleGeneratorBufferedParallel(string path, string graphName, int maxBuffer = 1000)
        {
            tg = new TripleGeneratorBuffered(path, graphName, maxBuffer);
        }

        public TripleGeneratorBufferedParallel(Stream baseStream, string graphName, int maxBuffer = 1000)
        {
            tg = new TripleGeneratorBuffered(baseStream, graphName, maxBuffer);

        }

        /// <summary>
        ///  запускаеи чтение с помощью TripleGeneratorBuffered в отдельном потоке.
        /// синхронизация буферов с помощью очереди.  
       /// </summary>
        /// <param name="onGenerate"> в основном потоке вынимает из очереди порции и выполняет onGenerate</param>
        public void Start(Action<List<Triple<string, string, ObjectVariants>>> onGenerate)
        {
            var queue=new Queue<List<Triple<string, string, ObjectVariants>>>();

            var thread = new Thread(() =>
                tg.Start(b =>
                {
                    lock (queue)
                    {
                        queue.Enqueue(b);
                    }

                }));
            thread.Start();
            while (true)
            {
                int count;
                lock (queue)
                {
                    count = queue.Count;
                }
                if (count == 0)
                {
                    if (!thread.IsAlive) break;
                    Thread.Sleep(1);
                }
                else
                {
                    List<Triple<string, string, ObjectVariants>> buffer;
                    lock (queue)
                    {
                        buffer = queue.Dequeue();
                    }
                    onGenerate(buffer);
                }

            }
        }
    }
}