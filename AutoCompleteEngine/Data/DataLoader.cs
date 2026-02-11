using AutoCompleteEngine.Engine;
using System.Text.Json;

namespace AutoCompleteEngine.Data;

public class DataLoader
{
    public static async Task LoadWords(string filePath, ISuggestionEngine suggestionEngine, int? count = null)
    {
        using var fileStream = File.OpenRead(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var words = await JsonSerializer.DeserializeAsync<List<string>>(fileStream, options);

        if (words != null)
        {
            if (count.HasValue)
            {
                words = words.Take(count.Value).ToList();
            }

            foreach (var word in words)
            {
                suggestionEngine.Ingest(word);

                suggestionEngine.IngestWithoutTreeList(word);

                suggestionEngine.IngestWithoutTreeDictionaryHashSet(word);
            }
        }
    }

    public static async Task<List<QueryInfo>> LoadPrefixes(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var queries = await JsonSerializer.DeserializeAsync<List<QueryInfo>>(fileStream, options);

        return queries;
    }
}
