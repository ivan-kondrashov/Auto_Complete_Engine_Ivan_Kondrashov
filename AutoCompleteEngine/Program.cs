using AutoCompleteEngine.Data;
using AutoCompleteEngine.Engine;

var engine = new SuggestionEngine();

await DataLoader.LoadJsonData("Data/data.json", engine);

Console.Write("Prefix: ");
var prefix = Console.ReadLine();

var suggestions = engine.Query(prefix);

if (suggestions == null || suggestions.Count == 0)
{
    Console.WriteLine("There are no suggested words!");

    return;
}

foreach (var suggestion in suggestions)
{
    Console.WriteLine(suggestion);
}
