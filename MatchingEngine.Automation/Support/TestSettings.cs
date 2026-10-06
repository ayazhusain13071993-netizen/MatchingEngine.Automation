using System.Text.Json;

namespace MatchingEngine.Automation.Support;

/// <summary>
/// Run settings. Defaults come from appsettings.json and can be overridden by
/// environment variables (BASE_URL, HEADLESS) - handy for CI.
/// </summary>
public sealed class TestSettings
{
    public string BaseUrl { get; set; } = "https://www.matchingengine.com/";
    public bool Headless { get; set; }
    public int TimeoutSeconds { get; set; } = 20;

    public static TestSettings Load()
    {
        var settings = new TestSettings();

        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (File.Exists(path))
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            settings = JsonSerializer.Deserialize<TestSettings>(File.ReadAllText(path), options) ?? settings;
        }

        var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
        if (!string.IsNullOrWhiteSpace(baseUrl))
        {
            settings.BaseUrl = baseUrl;
        }

        if (bool.TryParse(Environment.GetEnvironmentVariable("HEADLESS"), out var headless))
        {
            settings.Headless = headless;
        }

        return settings;
    }
}
