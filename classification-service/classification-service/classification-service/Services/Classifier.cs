using classification_service.Models;
using Microsoft.ML;

namespace classification_service.Services;

public class Classifier : IClassifier
{
    private readonly MLContext _mlContextAssigned;
    private readonly ITransformer _trainedModelAssigned;
    private readonly PredictionEngine<IssueAssigned, PredictionAssigned> _predEngineAssigned;
    private readonly MLContext _mlContextPriority;
    private readonly ITransformer _trainedModelPriority;
    private readonly PredictionEngine<IssuePriority, PredictionPriority> _predEnginePriority;

    public Classifier()
    {
        _mlContextAssigned = new MLContext();
        _trainedModelAssigned = _mlContextAssigned.Model.Load("assigned.zip", out var modelInputSchemaAssigned);
        _predEngineAssigned = _mlContextAssigned.Model.CreatePredictionEngine<IssueAssigned, PredictionAssigned>(_trainedModelAssigned);

        
        _mlContextPriority = new MLContext();
        _trainedModelPriority = _mlContextPriority.Model.Load("priority.zip", out var modelInputSchemaPriority);
        _predEnginePriority = _mlContextPriority.Model.CreatePredictionEngine<IssuePriority, PredictionPriority>(_trainedModelPriority);
    }
    
    public Task<PredictionAssigned> ClassifyAssigned(IEnumerable<string> labels)
    {
        var prediction = _predEngineAssigned.Predict(new IssueAssigned(labels));
        return Task.FromResult(prediction);
    }
    
    public Task<PredictionPriority> ClassifyPriority(IEnumerable<string> labels)
    {
        var prediction = _predEnginePriority.Predict(new IssuePriority(labels));
        return Task.FromResult(prediction);
    }
    
    

}