using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace MatchingEngine.Automation.Support;

/// <summary>
/// Owns the Chrome driver for one scenario. Reqnroll creates one instance per scenario
/// (context injection) and disposes it afterwards, so scenarios never share a browser.
/// </summary>
public sealed class BrowserSession : IDisposable
{
    private IWebDriver _driver;

    public TestSettings Settings { get; } = TestSettings.Load();

    public IWebDriver Driver => _driver ??= CreateDriver();

    public void Start() => _ = Driver;

    public string TakeScreenshot(string scenarioTitle)
    {
        var directory = Path.Combine(AppContext.BaseDirectory, "screenshots");
        Directory.CreateDirectory(directory);

        var safeName = string.Concat(scenarioTitle.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        var path = Path.Combine(directory, $"{safeName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

        ((ITakesScreenshot)Driver).GetScreenshot().SaveAsFile(path);
        return path;
    }

    private IWebDriver CreateDriver()
    {
        var options = new ChromeOptions();

        if (Settings.Headless)
        {
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
        }
        else
        {
            options.AddArgument("--start-maximized");
        }

        // A fixed desktop viewport keeps the full desktop navigation (no hamburger menu).
        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--disable-notifications");

        var driver = new ChromeDriver(options);
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
        return driver;
    }

    public void Dispose()
    {
        if (_driver == null)
        {
            return;
        }

        try
        {
            _driver.Quit();
        }
        finally
        {
            _driver.Dispose();
            _driver = null;
        }
    }
}
