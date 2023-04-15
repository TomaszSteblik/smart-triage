using Microsoft.ML.Data;

namespace classification_service.Models;

public class PredictionPriority
{
    [ColumnName("PredictedLabel")]
    public string Priority { get; set; }
}