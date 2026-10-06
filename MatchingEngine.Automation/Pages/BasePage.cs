using MatchingEngine.Automation.Support;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MatchingEngine.Automation.Pages;

/// <summary>Shared helpers for every page / component object. Explicit waits only - no Thread.Sleep.</summary>
public abstract class BasePage
{
    protected IWebDriver Driver { get; }
    protected TestSettings Settings { get; }
    protected WebDriverWait Wait { get; }

    protected BasePage(BrowserSession session)
    {
        Driver = session.Driver;
        Settings = session.Settings;
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(Settings.TimeoutSeconds))
        {
            PollingInterval = TimeSpan.FromMilliseconds(250)
        };
        Wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
    }

    protected void WaitForPageReady()
    {
        Wait.Message = "Timed out waiting for document.readyState to be 'complete'";
        Wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").ToString() == "complete");
    }

    protected IWebElement WaitForDisplayed(By locator, string description)
    {
        Wait.Message = $"Timed out waiting for {description} ({locator}) to be displayed";
        return Wait.Until(d => d.FindElements(locator).FirstOrDefault(e => e.Displayed));
    }

    /// <summary>Finds a displayed h1-h4 whose text matches (case-insensitive, whitespace-normalised).</summary>
    protected IWebElement FindHeading(string text) =>
        Driver.FindElements(By.XPath("//h1|//h2|//h3|//h4"))
              .FirstOrDefault(h => h.Displayed && TextNormaliser.Equal(TextOf(h), text));

    protected IWebElement WaitForHeading(string text)
    {
        Wait.Message = $"Timed out waiting for heading '{text}'";
        return Wait.Until(_ => FindHeading(text));
    }

    /// <summary>
    /// Raw DOM text (textContent): not affected by CSS text-transform, so assertions compare the
    /// real copy rather than e.g. an upper-cased rendering.
    /// </summary>
    protected static string TextOf(IWebElement element) =>
        TextNormaliser.Normalise(element.GetDomProperty("textContent"));

    protected void ScrollIntoView(IWebElement element) =>
        ((IJavaScriptExecutor)Driver).ExecuteScript(
            "arguments[0].scrollIntoView({block: 'center', inline: 'nearest', behavior: 'instant'});", element);

    protected bool IsInViewport(IWebElement element) =>
        (bool)((IJavaScriptExecutor)Driver).ExecuteScript(@"
            const r = arguments[0].getBoundingClientRect();
            const h = window.innerHeight || document.documentElement.clientHeight;
            const w = window.innerWidth || document.documentElement.clientWidth;
            return r.bottom > 0 && r.top < h && r.right > 0 && r.left < w;", element);
}
