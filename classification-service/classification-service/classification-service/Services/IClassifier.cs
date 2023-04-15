using classification_service.Models;

namespace classification_service.Services;

public interface IClassifier
{
    Task<PredictionAssigned> ClassifyAssigned(IEnumerable<string> labels);
    Task<PredictionPriority> ClassifyPriority(IEnumerable<string> labels);
}