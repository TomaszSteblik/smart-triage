using classification_service.Models;
using classification_service.Services;

namespace classification_service_tests;

public class UnitTest1
{
    [Fact]
    public async Task PredictionTest()
    {
        var issues = new[]
        {
            new Issue(new[] {"login", "authorization", "web"}, "tsteblik@gmail.com","High"),
            new Issue(new[] {"desktop", "browse", "explorer", "crash"}, "rkulka@gmail.com", "Low"),
            new Issue(new[] {"api","users", "edit"}, "rkulka@gmail.com", "High"),
            new Issue(new[] {"books", "adding", "windows"}, "tsteblik@gmail.com","Medium")
        };

        var labelsSteblik = new[] {"books", "adding", "windows"};
        var labelsKulka = new[] {"api", "browse", "windows"};

        var trainer = new Trainer();
        await trainer.Train(issues);
        var classifier = new Classifier();

        var predictionSteblik = await classifier.ClassifyAssigned(labelsSteblik);
        var predictionKulka = await classifier.ClassifyAssigned(labelsKulka);

        Assert.Equal("tsteblik@gmail.com", predictionSteblik.Assigned);
        Assert.Equal("rkulka@gmail.com", predictionKulka.Assigned);

    }
}