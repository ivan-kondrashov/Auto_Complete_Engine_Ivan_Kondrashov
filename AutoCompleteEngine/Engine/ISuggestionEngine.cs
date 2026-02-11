namespace AutoCompleteEngine.Engine;

public interface ISuggestionEngine
{
    void Ingest(string word);
    void IngestWithoutTreeDictionaryHashSet(string word);
    void IngestWithoutTreeList(string word);
    List<string> QueryLoopBFSString(string prefix);
    List<string> QueryLoopBFSStringBuilder(string prefix);
    List<string> QueryLoopDFSString(string prefix);
    List<string> QueryLoopDFSStringBuilder(string prefix);
    List<string> QueryRecursiveBFSString(string prefix);
    List<string> QueryRecursiveBFSStringBuilder(string prefix);
    List<string> QueryRecursiveDFSString(string prefix);
    List<string> QueryRecursiveDFSStringBuilder(string prefix);
    List<string> QueryWithoutTreeDictionaryHashSet(string prefix);
    List<string> QueryWithoutTreeList(string prefix);
}
