using System;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace MetroHash.Benchmark
{
    [MemoryDiagnoser]
    public class MetroBenchmark
    {
        private static readonly byte[] Data =
            Encoding.ASCII.GetBytes("012345678901234567890123456789012345678901234567890123456789012");

        private static readonly byte[] LargeData =
            Encoding.ASCII.GetBytes(
                "012345678901234567890123456789012345678901234567890123456789012012345678901234567890123456789012345678901234567890123456789012012345678901234567890123456789012345678901234567890123456789012012345678901234567890123456789012345678901234567890123456789012");

        private static readonly byte[] Buffer = new byte[16];

        [Benchmark]
        public byte[] MetroHash128NonIncremental()
        {
            return MetroHash128.Hash(0, Data, 0, Data.Length);
        }

        [Benchmark]
        public byte[] MetroHash128NonIncrementalLargeData()
        {
            return MetroHash128.Hash(0, LargeData, 0, LargeData.Length);
        }

        [Benchmark]
        public byte[] MetroHash128Incremental()
        {
            var metroHash = new MetroHash128(0);
            metroHash.Update(Data, 0, Data.Length);
            return metroHash.FinalizeHash();
        }


        [Benchmark]
        public byte[] MetroHash128NonIncrementalSpan()
        {
            MetroHash128.Hash(0, Data, Buffer);
            return Buffer;
        }

        [Benchmark]
        public byte[] MetroHash128IncrementalSpan()
        {
            var metroHash = new MetroHash128(0);
            metroHash.Update(Data);
            metroHash.FinalizeHash(Buffer);
            return Buffer;
        }

        [Benchmark]
        public byte[] MetroHash128IncrementalSpanDirectReturn()
        {
            var metroHash = new MetroHash128(0);
            metroHash.Update(Data);
            return metroHash.FinalizeHash();
        }
    }
}