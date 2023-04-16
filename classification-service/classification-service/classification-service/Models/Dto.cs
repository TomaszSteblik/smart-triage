namespace classification_service.Models;

public class Dto
{
    public Dto(int id, string assigned, string priority)
    {
        Id = id;
        Assigned = assigned;
        Priority = priority;
    }

    public int Id { get; set; }
    public string Assigned { get; set; }
    public string Priority { get; set; }
}