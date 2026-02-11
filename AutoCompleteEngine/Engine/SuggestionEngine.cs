namespace AutoCompleteEngine.Engine;

public class SuggestionEngine : ISuggestionEngine
{
    private readonly TreeNode _root = new TreeNode();

    public void Ingest(string word)
    {
        var currentNode = _root;

        var lowerCaseWord = word.ToLower();

        foreach (var symbol in lowerCaseWord)
        {
            if (!currentNode.Children.ContainsKey(symbol))
            {
                currentNode.Children.Add(symbol, new TreeNode());
            }

            currentNode = currentNode.Children[symbol];
        }

        currentNode.IsWordEnd = true;
    }

    public List<string> Query(string prefix)
    {
        var prefixNode = _root;

        var suggestions = new List<string>();

        var lowerCasePrefix = prefix.ToLower();

        foreach (var symbol in lowerCasePrefix)
        {
            if (!prefixNode.Children.TryGetValue(symbol, out var node))
            {
                return new List<string>();
            }

            prefixNode = node;
        }

        DFS(prefixNode, suggestions, lowerCasePrefix);

        return suggestions;
    }

    private void DFS(TreeNode node, List<string> suggestions, string currentWord)
    {
        if (suggestions.Count >= 5)
        {
            return;
        }

        if (node.IsWordEnd)
        {
            suggestions.Add(currentWord);
        }

        foreach (var item in node.Children)
        {
            DFS(item.Value, suggestions, currentWord + item.Key);

            if (suggestions.Count >= 5)
            {
                return;
            }
        }
    }
}
