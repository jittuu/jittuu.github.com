using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MultiThreadingAndBlockingIO {
    class Program {
        static void Main(string[] args) {
            var stopwatch = new Stopwatch();

            Console.WriteLine("");
            stopwatch.Start();

            Parallel.ForEach(Enumerable.Range(1, 10000), num => {
                // simulate some CPU operations
                var total = 0;
                for (int i = 0; i < 1000000; i++) {
                    total += i;
                }

                // comment this for without Console.WriteLine operation
                //Console.Write("....");
            });

            stopwatch.Stop();
            Console.WriteLine();
            //Console.WriteLine("With Console.WriteLine: {0:c}", stopwatch.Elapsed);
            Console.WriteLine("Without Console.WriteLine: {0:c}", stopwatch.Elapsed);
        }
    }
}
