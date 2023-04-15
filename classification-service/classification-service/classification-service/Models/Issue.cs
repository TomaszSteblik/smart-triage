namespace classification_service.Models;

public class Issue
{
    public Issue(IEnumerable<string> labels, string priority, string assigned)
    {
        Labels = labels;
        Priority = priority;
        Assigned = assigned;
    }

    public IEnumerable<string> Labels { get; set; }
    public string Priority { get; set; }
    public string Assigned { get; set; }
}