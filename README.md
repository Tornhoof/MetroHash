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
	MetroHash128.Hash(0, inputSpan, outputSpan);
```
Incremental: 
```csharp
    var metroHash = new MetroHash128(0);
    metroHash.Update(_data, 0, _data.Length);
    var hash = metroHash.FinalizeHash();
	
	var metroHash = new MetroHash128(0);
    metroHash.Update(dataSpan);
    var hash = metroHash.FinalizeHash();
	
	var metroHash = new MetroHash128(0);
    metroHash.Update(dataSpan);
    metroHash.FinalizeHash(outputSpan); // this version is a bit slower than the one above
```

Note: The Span versions are considerably slower on netfx as it does not yet support the fast span.

## Nuget
https://www.nuget.org/packages/MetroHash

## Performance
``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3906246 Hz, Resolution=256.0003 ns, Timer=TSC
.NET Core SDK=2.1.300-rtm-008701
  [Host]     : .NET Core 2.1.0-rtm-26502-02 (CoreCLR 4.6.26502.03, CoreFX 4.6.26502.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.0-rtm-26502-02 (CoreCLR 4.6.26502.03, CoreFX 4.6.26502.02), 64bit RyuJIT


```
|                                  Method |     Mean |     Error |    StdDev |  Gen 0 | Allocated |
|---------------------------------------- |---------:|----------:|----------:|-------:|----------:|
|              MetroHash128NonIncremental | 42.90 ns | 0.6105 ns | 0.5711 ns | 0.0095 |      40 B |
|                 MetroHash128Incremental | 65.56 ns | 0.1082 ns | 0.0904 ns | 0.0380 |     160 B |
|          MetroHash128NonIncrementalSpan | 45.46 ns | 0.1894 ns | 0.1772 ns |      - |       0 B |
|             MetroHash128IncrementalSpan | 85.60 ns | 0.0716 ns | 0.0598 ns | 0.0380 |     160 B |
| MetroHash128IncrementalSpanDirectReturn | 70.01 ns | 0.6243 ns | 0.5534 ns | 0.0380 |     160 B |


## Copyright
C++ Implementation: Copyright (c) 2015 J. Andrew Rogers

C# Implementation: Copyright © MetroHash for .NET Contributors
