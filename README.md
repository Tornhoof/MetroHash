# MetroHash for .NET
This is a direct port of J. Andrew Rogers' Metro Hash 128 algorithm.
Compare https://github.com/jandrewrogers/MetroHash

## Implementation
This implements the non-incremental and incremental version of the Metro Hash 128 algorithm.
Due to code-sharing between both versions the non-incremental is a bit slower than previously.

## Usage
Non-Incremental: 
```csharp
    var hash = MetroHash128.Hash(0, _data, 0, _data.Length);
```
Incremental: 
```csharp
    var metroHash = new MetroHash128(0);
    metroHash.Update(_data, 0, _data.Length);
    var hash = metroHash.FinalizeHash();
```

## Nuget
https://www.nuget.org/packages/MetroHash

## Performance
``` ini

BenchmarkDotNet=v0.10.3.0, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Xeon(R) CPU E5-1620 0 3.60GHz, ProcessorCount=8
Frequency=3507174 Hz, Resolution=285.1299 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1586.0
  DefaultJob : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1586.0


```
|                     Method |        Mean |    StdDev |  Gen 0 | Allocated |
|--------------------------- |------------ |---------- |------- |---------- |
| MetroHash128NonIncremental |  75.1922 ns | 0.1074 ns | 0.0041 |      40 B |
|    MetroHash128Incremental | 110.2251 ns | 0.4219 ns | 0.0243 |     160 B |


## Copyright
C++ Implementation: Copyright (c) 2015 J. Andrew Rogers

C# Implementation: Copyright © MetroHash for .NET Contributors 2017
