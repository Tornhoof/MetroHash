using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

namespace MetroHash.Benchmark
{
    [MemoryDiagnoser]
    [InliningDiagnoser]
    public class MetroBenchmark
    {
        private static readonly byte[] _data =
            Encoding.ASCII.GetBytes("012345678901234567890123456789012345678901234567890123456789012");

        [Benchmark]
        public byte[] MetroHash128NonIncremental()
        {
            return MetroHash128.Hash(0, _data, 0, _data.Length);
        }

        [Benchmark]
        public byte[] MetroHash128Incremental()
        {
            var metroHash = new MetroHash128(0);
            metroHash.Update(_data, 0, _data.Length);
            return metroHash.FinalizeHash();
        }
    }
}