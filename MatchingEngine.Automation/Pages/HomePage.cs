using MatchingEngine.Automation.Support;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MatchingEngine.Automation.Pages;

public sealed class HomePage : BasePage
{
    // Best-effort: only used if a cookie banner appears.
    private static readonly By CookieAcceptButton = By.XPath(
        "//button[normalize-space()='Accept all' or normalize-space()='Accept All' or normalize-space()='Accept' " +
        "or normalize-space()='Allow all' or normalize-space()='I agree']");

    public HomePage(BrowserSession session) : base(session)
    {
    }

    public string Title => Driver.Title;

    public void Open()
    {
        Driver.Navigate().GoToUrl(Settings.BaseUrl);
        WaitForPageReady();
        DismissCookieBannerIfPresent();
    }

    private void DismissCookieBannerIfPresent()
    {
        try
        {
            var button = new WebDriverWait(Driver, TimeSpan.FromSeconds(3))
                .Until(d => d.FindElements(CookieAcceptButton).FirstOrDefault(e => e.Displayed));
            button.Click();
        }
        catch (WebDriverTimeoutException)
        {
            // No banner - nothing to do.
        }
    }
}
