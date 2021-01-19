using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace PA1
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");
            NQueenTest();
            Console.WriteLine("===========================================");
            PageRankTest();
            Console.WriteLine("===========================================");
            MeanShiftTest();
        }

        static void NQueenTest()
        {
            Console.WriteLine("N-Queen");
            Stopwatch stopwatch = new Stopwatch();
            NQueen nQueen = new NQueen(13);
            stopwatch.Start();
            var result = nQueen.Solve();
            stopwatch.Stop();
            Console.WriteLine($"Multi-thread time elapsed: {stopwatch.Elapsed}");
            Console.WriteLine($"Result: {result.Count}");

            stopwatch.Restart();
            var result2 = nQueen.SolveSingleThread();
            stopwatch.Stop();
            Console.WriteLine($"Single-thread time elapsed: {stopwatch.Elapsed}");
            Console.WriteLine($"Result: {result2.Count}");
            Console.WriteLine();
        }

        static void PageRankTest()
        {
            Console.WriteLine("PageRank");
            Stopwatch stopwatch = new Stopwatch();
            PageRank pageRank = new PageRank(@"..\..\..\assets\polblogs.csv");
            stopwatch.Start();
            var result = pageRank.Solve(4);
            stopwatch.Stop();
            Console.WriteLine($"Multi-thread time elapsed: {stopwatch.Elapsed}");
            foreach (var entry in result.OrderBy(entry => entry.Key).Take(5))
            {
                Console.WriteLine($"{entry.Key}: {entry.Value}");
            }
            Console.WriteLine(".....");

            stopwatch.Restart();
            var result2 = pageRank.SolveSingleThread();
            stopwatch.Stop();
            Console.WriteLine($"Single-thread time elapsed: {stopwatch.Elapsed}");
            Console.WriteLine($"Result:");
            foreach(var entry in result2.OrderBy(entry => entry.Key).Take(5))
            {
                Console.WriteLine($"{entry.Key}: {entry.Value}");
            }
            Console.WriteLine(".....");
            Console.WriteLine();
        }

        static void MeanShiftTest()
        {
            Console.WriteLine("Mean-Shift");
            Stopwatch stopwatch = new Stopwatch();
            MeanShift meanShift = new MeanShift(@"..\..\..\assets\mean-shift-data.csv");

            stopwatch.Start();
            IList<double[]> result = meanShift.Solve();
            foreach (var point in result)
            {
                Console.WriteLine(string.Join(", ", point));
            }
            stopwatch.Stop();
            Console.WriteLine($"Multi-thread time elapsed: {stopwatch.Elapsed}");

            Console.WriteLine();
            stopwatch.Restart();
            IList<double[]> result2 = meanShift.SolveSingleThread();
            foreach (var point in result2)
            {
                Console.WriteLine(string.Join(", ", point));
            }
            stopwatch.Stop();
            Console.WriteLine($"Single-thread time elapsed: {stopwatch.Elapsed}");
            Console.WriteLine();
        }
    }
}
