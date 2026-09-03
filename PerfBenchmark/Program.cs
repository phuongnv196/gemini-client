using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public class EnumerableBenchmark
{
    private IEnumerable<int> _contents;

    [Params(1, 10, 100)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // Using a generator to ensure multiple enumeration actually evaluates the sequence again
        _contents = GenerateData(Size);
    }

    private IEnumerable<int> GenerateData(int size)
    {
        for(int i = 0; i < size; i++)
        {
            yield return i;
        }
    }

    [Benchmark(Baseline = true)]
    public List<int> AnyThenToList()
    {
        if (_contents == null || !_contents.Any())
            throw new ArgumentException();

        return _contents.ToList();
    }

    [Benchmark]
    public List<int> ToListThenCount()
    {
        if (_contents == null)
            throw new ArgumentException();

        var list = _contents.ToList();
        if (list.Count == 0)
            throw new ArgumentException();

        return list;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<EnumerableBenchmark>();
    }
}
