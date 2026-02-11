namespace AutoCompleteEngine.Engine;

public interface ISuggestionEngine
{
    void Ingest(string word);
    List<string> Query(string prefix);
}
