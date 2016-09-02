﻿using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;

namespace MetroHash.Benchmark
{
    public class MyConfig : ManualConfig
    {
        public MyConfig()
        {
            Add(new MemoryDiagnoser());
        }
    }

    [Config(typeof(MyConfig))]
    public class MetroBenchmark
    {
        private static readonly byte[] _data =
            Encoding.ASCII.GetBytes("012345678901234567890123456789012345678901234567890123456789012");

        [Benchmark]
        public byte[] MetroHash128NonIncremental()
        {
            return MetroHash128.Hash(0, _data, 0, _data.Length);
        }
    }
}