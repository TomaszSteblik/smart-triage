using Microsoft.ML.Data;

namespace classification_service.Models;

public class PredictionAssigned
{
    [ColumnName("PredictedLabel")]
    public string Assigned { get; set; }
}