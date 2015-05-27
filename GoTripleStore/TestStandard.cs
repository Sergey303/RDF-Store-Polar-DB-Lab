using System;
using System.Collections.Generic;
using System.Linq;
using Polar.Data;

namespace GoTripleStore
{
    public class TestStandard
    {
        public static void Main6() // Main6()
        {
            string path = "../../../Databases/";
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            Random rnd = new Random();
            int cnt = -1;
            Standard3TabsInt tabs = new Standard3TabsInt(path);
            int npersons = 400000;
            bool toload = true;
            if (toload)
            {
                sw.Restart();
                TestDataGenerator generator = new TestDataGenerator(npersons, 2378459);
                tabs.Build(generator.Generate());
                sw.Stop();
                Console.WriteLine("Load ok. duration={0}", sw.ElapsedMilliseconds);
            }

            sw.Restart();
            for (int i = 0; i < 1000; i++)
            {
                int code = rnd.Next(npersons - 1);
                object[] v = tabs.GetPersonByCode(code);
            }
            sw.Stop();
            Console.WriteLine("1000 persons ok. duration={0}", sw.ElapsedMilliseconds);

            sw.Restart();
            for (int i = 0; i < 1000; i++)
            {
                int code = rnd.Next(2*npersons - 1);
                object[] v = tabs.GetPhoto_docByCode(code);
                if (i == 200)
                {
                    Console.WriteLine("photo_doc record: {0} {1}", v[0], v[1]);
                }
            }
            sw.Stop();
            Console.WriteLine("1000 photo_docs ok. duration={0}", sw.ElapsedMilliseconds);

            sw.Restart();
            for (int i = 0; i < 100; i++)
            {
                int code = rnd.Next(2 * npersons - 1);
                cnt = tabs.GetReflectionsByReflected(code).Count();
            }
            sw.Stop();
            Console.WriteLine("100 portraits ok. duration={0}", sw.ElapsedMilliseconds);
        }
    }
}
