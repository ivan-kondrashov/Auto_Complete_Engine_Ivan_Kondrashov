using System.Text;

namespace AutoCompleteEngine.Engine;

public class SuggestionEngine : ISuggestionEngine
{
    private readonly TreeNode _root = new TreeNode();

    private readonly List<string> _wordsList = new List<string>();

    private readonly Dictionary<string, List<string>> _prefixMap = new Dictionary<string, List<string>>();

    private readonly HashSet<string> _fullWords = new HashSet<string>();

    public void IngestWithoutTreeDictionaryHashSet(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return;
        }

        string lowerWord = word.ToLower();

        if (!_fullWords.Add(lowerWord))
        {
            return;
        }

        for (int i = 1; i <= lowerWord.Length; i++)
        {
            string prefix = lowerWord.Substring(0, i);

            if (!_prefixMap.TryGetValue(prefix, out var suggestions))
            {
                suggestions = new List<string>();
                _prefixMap[prefix] = suggestions;
            }

            if (suggestions.Count < 5)
            {
                suggestions.Add(lowerWord);
            }
        }
    }

    public List<string> QueryWithoutTreeDictionaryHashSet(string prefix)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            return new List<string>();
        }

        string lowerPrefix = prefix.ToLower();

        if (_prefixMap.TryGetValue(lowerPrefix, out var result))
        {
            return result;
        }

        return new List<string>();
    }

    public void IngestWithoutTreeList(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return;
        }

        string lowerWord = word.ToLower();

        int index = _wordsList.BinarySearch(lowerWord);

        if (index < 0)
        {
            _wordsList.Insert(~index, lowerWord);
        }
    }

    public List<string> QueryWithoutTreeList(string prefix)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            return new List<string>();
        }

        string lowerPrefix = prefix.ToLower();

        var suggestions = new List<string>();

        int index = _wordsList.BinarySearch(lowerPrefix);

        if (index < 0)
        {
            index = ~index;
        }

        for (int i = index; i < _wordsList.Count && suggestions.Count < 5; i++)
        {
            if (_wordsList[i].StartsWith(lowerPrefix))
            {
                suggestions.Add(_wordsList[i]);
            }
            else
            {
                break;
            }
        }

        return suggestions;
    }

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

    public List<string> QueryRecursiveDFSString(string prefix)
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

        RecursiveDFSString(prefixNode, suggestions, lowerCasePrefix);

        return suggestions;
    }

    public List<string> QueryRecursiveDFSStringBuilder(string prefix)
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

        var currentWord = new StringBuilder(lowerCasePrefix);

        RecursiveDFSStringBuilder(prefixNode, suggestions, currentWord);

        return suggestions;
    }

    private void RecursiveDFSString(TreeNode node, List<string> suggestions, string currentWord)
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
            RecursiveDFSString(item.Value, suggestions, currentWord + item.Key);

            if (suggestions.Count >= 5)
            {
                return;
            }
        }
    }

    private void RecursiveDFSStringBuilder(TreeNode node, List<string> suggestions, StringBuilder currentWord)
    {
        if (suggestions.Count >= 5)
        {
            return;
        }

        if (node.IsWordEnd)
        {
            suggestions.Add(currentWord.ToString());
        }

        foreach (var item in node.Children)
        {
            currentWord.Append(item.Key);

            RecursiveDFSStringBuilder(item.Value, suggestions, currentWord);

            currentWord.Length--;

            if (suggestions.Count >= 5)
            {
                return;
            }
        }
    }

    public List<string> QueryLoopDFSString(string prefix)
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

        var stack = new Stack<(TreeNode Node, string CurrentWord)>();

        stack.Push((prefixNode, lowerCasePrefix));

        while (stack.Count > 0)
        {
            if (suggestions.Count >= 5)
            {
                break;
            }

            var (currentNode, currentWord) = stack.Pop();

            if (currentNode.IsWordEnd)
            {
                suggestions.Add(currentWord);
            }

            foreach (var child in currentNode.Children.OrderByDescending(x => x.Key))
            {
                stack.Push((child.Value, currentWord + child.Key));
            }
        }

        return suggestions;
    }

    public List<string> QueryLoopDFSStringBuilder(string prefix)
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

        var stack = new Stack<(TreeNode Node, StringBuilder CurrentWord)>();

        var rootBuilder = new StringBuilder(lowerCasePrefix);

        stack.Push((prefixNode, rootBuilder));

        while (stack.Count > 0)
        {
            if (suggestions.Count >= 5)
            {
                break;
            }

            var (currentNode, currentWord) = stack.Pop();

            if (currentNode.IsWordEnd)
            {
                suggestions.Add(currentWord.ToString());
            }

            var currentLength = currentWord.Length;

            foreach (var child in currentNode.Children.OrderByDescending(x => x.Key))
            {
                currentWord.Append(child.Key);

                stack.Push((child.Value, new StringBuilder(currentWord.ToString())));

                currentWord.Length = currentLength;
            }
        }

        return suggestions;
    }

    public List<string> QueryLoopBFSString(string prefix)
    {
        var prefixNode = _root;

        var lowerCasePrefix = prefix.ToLower();

        foreach (var symbol in lowerCasePrefix)
        {
            if (!prefixNode.Children.TryGetValue(symbol, out var node))
            {
                return new List<string>();
            }

            prefixNode = node;
        }

        var suggestions = new List<string>();

        var queue = new Queue<(TreeNode Node, string Word)>();

        queue.Enqueue((prefixNode, lowerCasePrefix));

        while (queue.Count > 0 && suggestions.Count < 5)
        {
            var (currentNode, currentWord) = queue.Dequeue();

            if (currentNode.IsWordEnd)
            {
                suggestions.Add(currentWord);

                if (suggestions.Count >= 5)
                {
                    break;
                }
            }

            foreach (var child in currentNode.Children)
            {
                queue.Enqueue((child.Value, currentWord + child.Key));
            }
        }

        return suggestions;
    }

    public List<string> QueryLoopBFSStringBuilder(string prefix)
    {
        var prefixNode = _root;

        var lowerCasePrefix = prefix.ToLower();

        foreach (var symbol in lowerCasePrefix)
        {
            if (!prefixNode.Children.TryGetValue(symbol, out var node))
            {
                return new List<string>();
            }

            prefixNode = node;
        }

        var suggestions = new List<string>();

        var queue = new Queue<(TreeNode Node, string Word)>();

        queue.Enqueue((prefixNode, lowerCasePrefix));

        var sb = new StringBuilder();

        while (queue.Count > 0 && suggestions.Count < 5)
        {
            var (currentNode, currentWord) = queue.Dequeue();

            if (currentNode.IsWordEnd)
            {
                suggestions.Add(currentWord);

                if (suggestions.Count >= 5)
                {
                    break;
                }
            }

            foreach (var child in currentNode.Children)
            {
                sb.Clear();

                sb.Append(currentWord);

                sb.Append(child.Key);

                queue.Enqueue((child.Value, sb.ToString()));
            }
        }

        return suggestions;
    }

    public List<string> QueryRecursiveBFSString(string prefix)
    {
        var prefixNode = _root;

        var lowerCasePrefix = prefix.ToLower();

        int maxWordsCount = 5;

        foreach (var symbol in lowerCasePrefix)
        {
            if (!prefixNode.Children.TryGetValue(symbol, out var node))
            {
                return new List<string>();
            }
            prefixNode = node;
        }

        var suggestions = new List<string>();

        var currentLevel = new List<(TreeNode Node, string Word)>
        {
            (prefixNode, lowerCasePrefix)
        };

        RecursiveBFSString(currentLevel, suggestions, maxWordsCount);

        return suggestions;
    }

    private void RecursiveBFSString(
        List<(TreeNode Node, string Word)> currentLevel,
        List<string> suggestions,
        int maxWordsCount)
    {
        if (currentLevel.Count == 0 || suggestions.Count >= maxWordsCount)
        {
            return;
        }

        var nextLevel = new List<(TreeNode Node, string Word)>();

        foreach (var (node, word) in currentLevel)
        {
            if (suggestions.Count >= maxWordsCount)
            {
                break;
            }

            if (node.IsWordEnd)
            {
                suggestions.Add(word);

                if (suggestions.Count >= maxWordsCount)
                {
                    break;
                }
            }

            foreach (var child in node.Children)
            {
                nextLevel.Add((child.Value, word + child.Key));
            }
        }

        if (suggestions.Count < maxWordsCount && nextLevel.Count > 0)
        {
            RecursiveBFSString(nextLevel, suggestions, maxWordsCount);
        }
    }

    public List<string> QueryRecursiveBFSStringBuilder(string prefix)
    {
        var prefixNode = _root;

        var lowerCasePrefix = prefix.ToLower();

        int maxWordsCount = 5;

        foreach (var symbol in lowerCasePrefix)
        {
            if (!prefixNode.Children.TryGetValue(symbol, out var node))
            {
                return new List<string>();
            }
            prefixNode = node;
        }

        var suggestions = new List<string>();

        var currentLevel = new List<(TreeNode Node, StringBuilder Sb)>
        {
            (prefixNode, new StringBuilder(lowerCasePrefix))
        };

        RecursiveBFSStringBuilder(currentLevel, suggestions, maxWordsCount);

        return suggestions;
    }

    private void RecursiveBFSStringBuilder(
        List<(TreeNode Node, StringBuilder Sb)> currentLevel,
        List<string> suggestions,
        int maxWordsCount)
    {
        if (currentLevel.Count == 0 || suggestions.Count >= maxWordsCount)
        {
            return;
        }

        var nextLevel = new List<(TreeNode Node, StringBuilder Sb)>();

        foreach (var (node, sb) in currentLevel)
        {
            if (suggestions.Count >= maxWordsCount)
            {
                break;
            }

            if (node.IsWordEnd)
            {
                suggestions.Add(sb.ToString());

                if (suggestions.Count >= maxWordsCount)
                {
                    break;
                }
            }

            foreach (var child in node.Children)
            {
                var newSb = new StringBuilder(sb.ToString());

                newSb.Append(child.Key);

                nextLevel.Add((child.Value, newSb));
            }
        }

        if (suggestions.Count < maxWordsCount && nextLevel.Count > 0)
        {
            RecursiveBFSStringBuilder(nextLevel, suggestions, maxWordsCount);
        }
    }
}
