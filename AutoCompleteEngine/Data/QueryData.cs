namespace AutoCompleteEngine.Data;

public class QueryData
{
    public required List<string> Words { get; set; }
    public required List<QueryInfo> Queries { get; set; }
}
