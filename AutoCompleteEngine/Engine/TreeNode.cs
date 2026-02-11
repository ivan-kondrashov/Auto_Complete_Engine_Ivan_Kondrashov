namespace AutoCompleteEngine.Engine;

public class TreeNode
{
    public Dictionary<char, TreeNode> Children { get; set; } = new Dictionary<char, TreeNode>();
    public bool IsWordEnd { get; set; }
}
