using System.Text;
using BenchmarkDotNet.Attributes;

namespace MetroHash.Benchmark
{
    [MemoryDiagnoser]
    public class MetroBenchmark
    {
        private static readonly byte[] Data =
            Encoding.ASCII.GetBytes("012345678901234567890123456789012345678901234567890123456789012");

        [Benchmark]
        public byte[] MetroHash128NonIncremental()
        {
            return MetroHash128.Hash(0, Data, 0, Data.Length);
        }

        [Benchmark]
        public byte[] MetroHash128Incremental()
        {
            var metroHash = new MetroHash128(0);
            metroHash.Update(Data, 0, Data.Length);
            return metroHash.FinalizeHash();
        }
    }
}