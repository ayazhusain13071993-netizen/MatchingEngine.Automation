using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using MatchingEngine.Automation.Support;
using OpenQA.Selenium;
using Reqnroll;

namespace MatchingEngine.Automation.Hooks;

[Binding]
public sealed class ExtentReportHooks
{
    private static ExtentReports _extent;
    private static ExtentTest _feature;

    private readonly ScenarioContext _scenarioContext;
    private readonly BrowserSession _session;
    private ExtentTest _scenario;
    private bool _failureLogged;
    private static string _reportPath;

    public ExtentReportHooks(ScenarioContext scenarioContext, BrowserSession session)
    {
        _scenarioContext = scenarioContext;
        _session = session;
    }

    [BeforeTestRun]
    public static void InitialiseReport()
    {
        _reportPath = Path.Combine(AppContext.BaseDirectory, "Reports", $"ExtentReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");
        Directory.CreateDirectory(Path.GetDirectoryName(_reportPath)!);

        var spark = new ExtentSparkReporter(_reportPath);
        spark.Config.DocumentTitle = "Matching Engine - UI Automation";
        spark.Config.ReportName = "Matching Engine - Solutions navigation";

        _extent = new ExtentReports();
        _extent.AttachReporter(spark);
        _extent.AddSystemInfo("Browser", "Google Chrome");
        _extent.AddSystemInfo("Framework", "Selenium + C# + Reqnroll + NUnit");
    }

    [AfterTestRun]
    public static void FlushReport()
    {
        _extent.Flush();
        Console.WriteLine($"Extent report: {_reportPath}");
    }

    [BeforeFeature]
    public static void CreateFeatureNode(FeatureContext featureContext) =>
        _feature = _extent.CreateTest(featureContext.FeatureInfo.Title);

    [BeforeScenario]
    public void CreateScenarioNode() =>
        _scenario = _feature.CreateNode(_scenarioContext.ScenarioInfo.Title);

    [AfterStep]
    public void LogStep()
    {
        var info = _scenarioContext.StepContext.StepInfo;
        var stepText = $"{info.StepDefinitionType} {info.Text}";

        if (_scenarioContext.TestError == null)
        {
            _scenario.Pass(stepText);
            return;
        }

        if (_failureLogged)
        {
            return; // later steps are skipped after a failure
        }

        _failureLogged = true;
        try
        {
            var base64 = ((ITakesScreenshot)_session.Driver).GetScreenshot().AsBase64EncodedString;
            _scenario.Fail($"{stepText}<br>{_scenarioContext.TestError.Message}",
                MediaEntityBuilder.CreateScreenCaptureFromBase64String(base64, "Failure screenshot").Build());
        }
        catch (Exception)
        {
            _scenario.Fail($"{stepText}<br>{_scenarioContext.TestError.Message}");
        }
    }
}