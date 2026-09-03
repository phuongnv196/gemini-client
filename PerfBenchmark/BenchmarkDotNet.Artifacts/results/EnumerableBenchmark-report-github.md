```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Processor 2.30GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.103
  [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3


```
| Method          | Size | Mean      | Error    | StdDev   | Ratio |
|---------------- |----- |----------:|---------:|---------:|------:|
| **AnyThenToList**   | **1**    |  **57.81 ns** | **0.374 ns** | **0.350 ns** |  **1.00** |
| ToListThenCount | 1    |  49.28 ns | 0.178 ns | 0.158 ns |  0.85 |
|                 |      |           |          |          |       |
| **AnyThenToList**   | **10**   | **159.14 ns** | **0.959 ns** | **0.801 ns** |  **1.00** |
| ToListThenCount | 10   | 146.38 ns | 0.508 ns | 0.475 ns |  0.92 |
|                 |      |           |          |          |       |
| **AnyThenToList**   | **100**  | **676.40 ns** | **4.629 ns** | **4.330 ns** |  **1.00** |
| ToListThenCount | 100  | 671.15 ns | 4.203 ns | 3.510 ns |  0.99 |
