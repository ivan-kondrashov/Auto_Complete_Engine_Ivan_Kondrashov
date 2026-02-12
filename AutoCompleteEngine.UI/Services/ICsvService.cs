using AutoCompleteEngine.UI.Models;

namespace AutoCompleteEngine.UI.Services;

public interface ICsvService
{
    Task<List<BenchmarkInfo>> LoadBenchmarkDataAsync(string filePath);
}
