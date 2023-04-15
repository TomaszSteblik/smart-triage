namespace classification_service.Models;

public class IssuePriority
{
    public IssuePriority(IEnumerable<string> labels, string priority)
    {
        Labels = labels.Aggregate((partialPhrase, word) =>$"{partialPhrase} {word}");
        Priority = priority;
    }

    public IssuePriority(IEnumerable<string> labels)
    {
        Labels = labels.Aggregate((partialPhrase, word) =>$"{partialPhrase} {word}");
    }

    public string Labels { get; set; }
    public string Priority { get; set; }
}