# MetroHash for .NET
This is a direct port of J. Andrew Rogers' Metro Hash 128 algorithm.
Compare https://github.com/jandrewrogers/MetroHash

## Implementation
This implements the non-incremental version of the Metro Hash 128 algorithm.
Note: An incremental implementation deriving from HashAlgorithm is several times slower, due to memcpy overhead for incremental and the overall overhead from .NET's HashAlgorithm system.

## Nuget
https://www.nuget.org/packages/MetroHash

## Performance
```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU E5-1620 0 3.60GHz, ProcessorCount=8
Frequency=3507226 ticks, Resolution=285.1256 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=64-bit RELEASE [RyuJIT]
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1590.0

Type=MetroBenchmark  Mode=Throughput  

```
                     Method |      Median |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
--------------------------- |------------ |---------- |------- |------ |------ |------------------- |
 MetroHash128NonIncremental | 110.4863 ns | 0.4324 ns | 757.00 |     - |     - |              41,96 |

## Copyright
C++ Implementation: Copyright (c) 2015 J. Andrew Rogers

C# Implementation: Copyright © MetroHash for .NET Contributors 2016
