using BenchmarkDotNet.Running;

namespace AutoCompleteEngine.Benchmark;

public class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<SuggestionEngineBenchmark>();
    }
}
