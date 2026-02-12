using CsvHelper.Configuration.Attributes;

namespace AutoCompleteEngine.UI.Models;

public class BenchmarkInfo
{
    [Name("Method")]
    public string Method { get; set; } = string.Empty;

    [Name("Mean")]
    public string MeanRaw { get; set; } = string.Empty;

    [Name("Allocated")]
    public string AllocatedRaw { get; set; } = string.Empty;

    [Name("WordsCount")]
    public int WordsCount { get; set; }

    [Ignore]
    public double TimeNs { get; set; }

    [Ignore]
    public double MemoryBytes { get; set; }
}
