# MetroHash for .NET
This is a direct port of J. Andrew Rogers' Metro Hash 128 algorithm.
Compare https://github.com/jandrewrogers/MetroHash

## Implementation
This implements the non-incremental version of the Metro Hash 128 algorithm.

Todo: Incremental

## Nuget
https://www.nuget.org/packages/MetroHash

## Performance
``` ini

BenchmarkDotNet=v0.10.3.0, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Xeon(R) CPU E5-1620 0 3.60GHz, ProcessorCount=8
Frequency=3507226 ticks, Resolution=285.1256 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1586.0
  DefaultJob : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1586.0


```
| Method                     |       Mean |    StdDev |  Gen 0 | Allocated |
|--------------------------- |----------- |---------- |------- |---------- |
| MetroHash128NonIncremental | 54.7950 ns | 0.0998 ns | 0.0079 |      40 B |


## Copyright
C++ Implementation: Copyright (c) 2015 J. Andrew Rogers

C# Implementation: Copyright © MetroHash for .NET Contributors 2017
