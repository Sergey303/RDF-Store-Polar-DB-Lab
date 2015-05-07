﻿using System;
using System.Diagnostics;
using System.IO;

namespace TestingNs
{
    public static class Perfomance
    {
        private static Stopwatch timer = new Stopwatch();

        /// <summary>
        /// Выводит в консоль время исполнения
        /// </summary>
        /// <param name="action">тестируемый метод</param>
        /// <param name="mesage"></param>
        /// <param name="outputFile">if true, write result at file</param>
        public static void ComputeTime(this Action action, string mesage, bool outputFile = false, string pathOutputFile=@"..\..\Perfomance.txt")
        {
            timer.Restart();
            action.Invoke();
            timer.Stop();
            if (!outputFile)
                Console.WriteLine("{0} {1}ms", mesage, timer.Elapsed.TotalMilliseconds);
            else
                using (StreamWriter file = new StreamWriter(pathOutputFile, true))
                    file.WriteLine("{0} {1}ms", mesage, timer.Elapsed.Ticks*1.0 /10000);  //=100 * 10^-9 sec.
        }
    }
}