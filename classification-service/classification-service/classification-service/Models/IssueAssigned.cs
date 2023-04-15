namespace classification_service.Models;

public class IssueAssigned
{
    public IssueAssigned(IEnumerable<string> labels, string assigned)
    {
        Labels = labels.Aggregate((partialPhrase, word) =>$"{partialPhrase} {word}");
        Assigned = assigned;
    }

    public IssueAssigned(IEnumerable<string> labels)
    {
        Labels = labels.Aggregate((partialPhrase, word) =>$"{partialPhrase} {word}");
    }

    public string Labels { get; set; }
    public string Assigned { get; set; }
}