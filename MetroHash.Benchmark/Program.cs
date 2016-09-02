using BenchmarkDotNet.Running;

namespace MetroHash.Benchmark
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<MetroBenchmark>();
        }
    }
}