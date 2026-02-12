using AutoCompleteEngine.Core.Data;
using AutoCompleteEngine.Core.Engine;
using Benchly;
using BenchmarkDotNet.Attributes;

namespace AutoCompleteEngine.Benchmark;

[MemoryDiagnoser]
[ColumnChart(Title = "Executing time")]
[BoxPlot(Title = "Auto complete methods comparing")]
public class SuggestionEngineBenchmark
{
    [Params(1000, 10000)]
    public int WordsCount { get; set; }
    private readonly SuggestionEngine _engine = new SuggestionEngine();
    private List<QueryInfo> _prefixes = new List<QueryInfo>();

    [GlobalSetup]
    public async Task Setup()
    {
        await DataLoader.LoadWords("Data/words.json", _engine, WordsCount);

        _prefixes = await DataLoader.LoadPrefixes("Data/prefixes.json");
    }

    [Benchmark]
    public void Query_Recursive_DFS_String()
    {
        foreach (var prefix in _prefixes)
        {
            _engine.QueryRecursiveDFSString(prefix.Prefix);
        }
    }

    [Benchmark]
    public void Query_Recursive_DFS_StringBuilder()
    {
        foreach (var prefix in _prefixes)
        {
            _engine.QueryRecursiveDFSStringBuilder(prefix.Prefix);
        }
    }

    [Benchmark]
    public void Query_Loop_DFS_String()
    {
        foreach (var prefix in _prefixes)
        {
            _engine.QueryLoopDFSString(prefix.Prefix);
        }
    }

    [Benchmark]
    public void Query_Loop_DFS_StringBuilder()
    {
        foreach (var prefix in _prefixes)
        {
            _engine.QueryLoopDFSStringBuilder(prefix.Prefix);
        }
    }

    [Benchmark]
    public void Query_Loop_BFS_String()
    {
        foreach (var prefix in _prefixes)
        {
            _engine.QueryLoopBFSString(prefix.Prefix);
        }
    }

    [Benchmark]
    public void Query_Loop_BFS_StringBuilder()
    {
        foreach (var prefix in _prefixes)
        {
            _engine.QueryLoopBFSStringBuilder(prefix.Prefix);
        }
    }

    [Benchmark]
    public void Query_Recursive_BFS_String()
    {
        foreach (var prefix in _prefixes)
        {
            _engine.QueryRecursiveBFSString(prefix.Prefix);
        }
    }

    [Benchmark]
    public void Query_Recursive_BFS_StringBuilder()
    {
        foreach (var prefix in _prefixes)
        {
            _engine.QueryRecursiveBFSStringBuilder(prefix.Prefix);
        }
    }

    [Benchmark]
    public void Query_Without_Tree_List()
    {
        foreach (var prefix in _prefixes)
        {
            _engine.QueryWithoutTreeList(prefix.Prefix);
        }
    }

    [Benchmark]
    public void Query_Without_Tree_Dictionary_HashSet()
    {
        foreach (var prefix in _prefixes)
        {
            _engine.QueryWithoutTreeDictionaryHashSet(prefix.Prefix);
        }
    }
}
