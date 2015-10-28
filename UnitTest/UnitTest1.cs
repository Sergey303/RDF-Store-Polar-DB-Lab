using System;
using System.Collections.Generic;
using System.Data.HashFunction;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFCommon;
using RDFCommon.OVns;
using RDFTripleStore;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            double five = 5.0;
            Assert.AreEqual(five,5);      
        }

        [TestMethod]
        public void HashTest(StoreCascadingInt store)    {
              
          
            var triples = store.GetTriples((s, p, o) => o.ToString()).Distinct().ToArray();
            Console.WriteLine(triples.Length);
            //var objectsDistinct = objects.Distinct().ToArray();
            
            //Console.WriteLine(objectsDistinct.Count());
           
            var hash = new DefaultBuzHash();
            var hashes = triples.Select(t =>
            {

                var h = hash.ComputeHash(t);
                return BitConverter.ToInt32(h,0);
            }).ToArray();

            var distinct = hashes.Distinct();
          //  Console.WriteLine(objects.Count(variants => variants is OV_integer));
            var count = distinct.Count();
            Console.WriteLine(count);
            Console.WriteLine(100 * count / hashes.Length);


        }

        [TestMethod]
        public void MPHTripleTest(StoreCascadingInt store)
        {
            var keyGenerator = new Triple2Key(store);
            Console.WriteLine("Generating minimum perfect hash function for {0} keys", keyGenerator.NbKeys);
            var start = DateTime.Now;
            var hashFunction = MPHTest.MPH.MinPerfectHash.Create(keyGenerator, 1);

            Console.WriteLine("Completed in {0:0.000000} s", DateTime.Now.Subtract(start).TotalMilliseconds / 1000.0);

            // Show the extra hash space necessary
            Console.WriteLine("Hash function map {0} keys to {1} hashes (load factor: {2:0.000000}%)",
                keyGenerator.NbKeys, hashFunction.N,
                ((keyGenerator.NbKeys * 100) / (double)hashFunction.N));

            // Check for any collision
            var used = new System.Collections.BitArray((int)hashFunction.N);
            keyGenerator.Rewind();
            start = DateTime.Now;
            for (var test = 0U; test < keyGenerator.NbKeys; test++)
            {
                var hash = (int)hashFunction.Search(keyGenerator.Read());
                if (used[hash])
                {
                    Console.WriteLine("FAILED - Collision detected at {0}", test);
                    return;
                }
                used[hash] = true;
            }
            var end = DateTime.Now.Subtract(start).TotalMilliseconds;
            Console.WriteLine("PASS - No collision detected");

            Console.WriteLine("Total scan time : {0:0.000000} s", end / 1000.0);
            Console.WriteLine("Average key hash time : {0} ms", end / (double)keyGenerator.NbKeys);


        }

        private class Triple2Key : MPHTest.MPH.IKeySource
    {
            private IEnumerator<byte[]> getTriples;
            private IGraph graph;

            public Triple2Key(IGraph graph)
            {
                this.graph = graph;
                getTriples = graph.GetTriples((s, p, o) => s.ToString() + p + o).Select(Encoding.UTF8.GetBytes).GetEnumerator();
            }

            public byte[] Read()
            {
                if (getTriples.MoveNext())
                    return getTriples.Current;
                else
                {
                    return null;
                }
            }

            public void Rewind()
            {
                getTriples = graph.GetTriples((s, p, o) => s.ToString() + p + o).Select(Encoding.UTF8.GetBytes).GetEnumerator();
            }

            public uint NbKeys { get { return (uint)graph.GetTriplesCount(); } }
    }

    }
}
