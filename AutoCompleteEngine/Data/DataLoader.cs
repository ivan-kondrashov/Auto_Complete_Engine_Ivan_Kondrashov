using AutoCompleteEngine.Engine;
using System.Text.Json;

namespace AutoCompleteEngine.Data;

public class DataLoader
{
    public static async Task LoadJsonData(string filePath, ISuggestionEngine suggestionEngine)
    {
        using var fileStream = File.OpenRead(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var data = await JsonSerializer.DeserializeAsync<QueryData>(fileStream, options);

        foreach (var word in data.Words)
        {
            suggestionEngine.Ingest(word);
        }
    }
}
