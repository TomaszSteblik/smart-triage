using classification_service.Models;

namespace classification_service.Services;

public interface ITrainer
{
    public Task Train(ICollection<Issue> issues);
}