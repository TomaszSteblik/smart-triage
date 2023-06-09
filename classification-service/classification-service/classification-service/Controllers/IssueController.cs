using System.Text;
using System.Text.Json;
using classification_service.Models;
using classification_service.Services;
using Microsoft.AspNetCore.Mvc;

namespace classification_service.Controllers;

[ApiController]
[Route("[controller]")]
public class IssueController : ControllerBase
{
    private readonly IClassifier _classifier;
    private readonly ITrainer _trainer;
    private readonly IHttpClientFactory _httpClientFactory;

    public IssueController(IClassifier classifier, ITrainer trainer, IHttpClientFactory httpClientFactory)
    {
        _classifier = classifier;
        _trainer = trainer;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    [Route("train")]
    public async Task<IActionResult> Train([FromBody] IEnumerable<Issue> issues)
    {
        await _trainer.Train(issues.ToList());
        return Ok();
    }
    
    [HttpPost]
    [Route("classify/{id}")]
    public async Task<IActionResult> Train([FromBody] IEnumerable<string> labels, [FromRoute] int id)
    {
        var labelsArray = labels.ToList();
        var assigned = await _classifier.ClassifyAssigned(labelsArray);
        var priority = await _classifier.ClassifyPriority(labelsArray);
        var client = _httpClientFactory.CreateClient();
        using StringContent jsonContent = new(
            JsonSerializer.Serialize(new
            {
                id = id,
                assigned = assigned.Assigned,
                priority = priority.Priority
            }),
            Encoding.UTF8,
            "application/json");
        await client.PostAsync("http://localhost:4567/classified",jsonContent);
        return Ok();
    }
}