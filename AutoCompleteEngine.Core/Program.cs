using AutoCompleteEngine.Core.Data;
using AutoCompleteEngine.Core.Engine;

var engine = new SuggestionEngine();

await DataLoader.LoadWords("Data/words_2.json", engine);

var prefixes = await DataLoader.LoadPrefixes("Data/prefixes.json");

Console.WriteLine("\nТестирование запросов:");
foreach (var test in prefixes)
{
    var results = engine.QueryWithoutTreeDictionaryHashSet(test.Prefix);

    Console.WriteLine($"\nПрефикс: '{test.Prefix}'");
    Console.WriteLine($"Результаты: {string.Join(",", results)}");

    if (results.Count > 5)
    {
        Console.WriteLine("! ОШИБКА: Вернулось больше 5 результатов");
    }
}
