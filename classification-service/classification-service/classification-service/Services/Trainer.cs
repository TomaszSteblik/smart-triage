using classification_service.Models;
using Microsoft.ML;

namespace classification_service.Services;

public class Trainer : ITrainer
{
    public Task TrainAssigned(IEnumerable<IssueAssigned> issues)
    {   
        var mlContext = new MLContext(seed: 1);
        
        var trainingDataView = mlContext.Data.LoadFromEnumerable(issues);
        
        var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "Assigned",inputColumnName:nameof(IssueAssigned.Assigned))
            .Append(mlContext.Transforms.Text.TokenizeIntoWords("LabelsTokenized", inputColumnName:nameof(IssueAssigned.Labels)))
            .Append(mlContext.Transforms.Text.FeaturizeText("LabelsFeaturized","LabelsTokenized"))
            .Append(mlContext.Transforms.Concatenate(outputColumnName:"Features", "LabelsFeaturized"))
            .AppendCacheCheckpoint(mlContext);  
        
        var trainer = mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Assigned", "Features");
        
        var trainingPipeline = dataProcessPipeline.Append(trainer)
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
        
        Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");
        var crossValidationResults= mlContext.MulticlassClassification.CrossValidate(data:trainingDataView, estimator:trainingPipeline, numberOfFolds: 6, labelColumnName:"Assigned");

        Console.WriteLine("=============== Training the model ===============");
        var trainedModel = trainingPipeline.Fit(trainingDataView);
        
        var predEngine = mlContext.Model.CreatePredictionEngine<IssueAssigned, PredictionAssigned>(trainedModel);
        mlContext.Model.Save(trainedModel, trainingDataView.Schema, "assigned.zip");
        
        return Task.CompletedTask;
    }
    
    public Task TrainPriority(IEnumerable<IssuePriority> issues)
    {   
        var mlContext = new MLContext(seed: 1);
        
        var trainingDataView = mlContext.Data.LoadFromEnumerable(issues);
        
        var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "Priority",inputColumnName:nameof(IssuePriority.Priority))
            .Append(mlContext.Transforms.Text.TokenizeIntoWords("LabelsTokenized", inputColumnName:nameof(IssueAssigned.Labels)))
            .Append(mlContext.Transforms.Text.FeaturizeText("LabelsFeaturized","LabelsTokenized"))
            .Append(mlContext.Transforms.Concatenate(outputColumnName:"Features", "LabelsFeaturized"))
            .AppendCacheCheckpoint(mlContext);  
        
        var trainer = mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Priority", "Features");
        
        var trainingPipeline = dataProcessPipeline.Append(trainer)
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
        
        Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");
        var crossValidationResults= mlContext.MulticlassClassification.CrossValidate(data:trainingDataView, estimator:trainingPipeline, numberOfFolds: 6, labelColumnName:"Priority");

        Console.WriteLine("=============== Training the model ===============");
        var trainedModel = trainingPipeline.Fit(trainingDataView);
        
        var predEngine = mlContext.Model.CreatePredictionEngine<IssueAssigned, PredictionAssigned>(trainedModel);
        mlContext.Model.Save(trainedModel, trainingDataView.Schema, "priority.zip");
        
        return Task.CompletedTask;
    }

    public async Task Train(ICollection<Issue> issues)
    {
        await Task.WhenAll(
            TrainAssigned(issues.Select(x => new IssueAssigned(x.Labels, x.Assigned))),
            TrainPriority(issues.Select(x => new IssuePriority(x.Labels, x.Priority)))
            );
    }
}