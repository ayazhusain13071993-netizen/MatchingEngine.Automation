using MatchingEngine.Automation.Support;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace MatchingEngine.Automation.Pages;

/// <summary>The site header, shared by every page (component object).</summary>
public sealed class HeaderComponent : BasePage
{
    // Solution links all live under /Music-and-copyright-solutions/<solution>.
    // Scoping to header/nav keeps the footer's "Solutions" column out of the results.
    private static readonly By SolutionLinks =
        By.XPath("//*[self::header or self::nav]//a[contains(@href,'/Music-and-copyright-solutions/')]");

    public HeaderComponent(BrowserSession session) : base(session)
    {
    }

    public void ExpandMenu(string menuName)
    {
        var toggle = WaitForDisplayed(
            By.XPath($"//*[self::header or self::nav]//*[normalize-space(text())={TextNormaliser.XPathLiteral(menuName)}]"),
            $"'{menuName}' header menu item");

        toggle.Click();

        // Click opens the menu on most builds; if this menu is hover-driven, fall back to hovering.
        if (!SolutionLinksAppearWithin(TimeSpan.FromSeconds(3)))
        {
            new Actions(Driver).MoveToElement(toggle).Perform();
        }

        Wait.Message = $"Timed out waiting for the '{menuName}' menu items to be displayed";
        Wait.Until(_ => VisibleSolutionLinks().Count > 0);
    }

    public IReadOnlyList<string> GetDisplayedSolutions() =>
        VisibleSolutionLinks()
            .Select(TextOf)
            .Where(text => text.Length > 0)
            .Distinct()
            .ToList();

    public void ClickSolution(string solutionName)
    {
        var link = VisibleSolutionLinks().FirstOrDefault(l => TextNormaliser.Equal(TextOf(l), solutionName));
        if (link == null)
        {
            throw new NoSuchElementException(
                $"Solution '{solutionName}' not found. Displayed: {string.Join(", ", GetDisplayedSolutions())}");
        }

        link.Click();
    }

    private List<IWebElement> VisibleSolutionLinks() =>
        Driver.FindElements(SolutionLinks).Where(e => e.Displayed).ToList();

    private bool SolutionLinksAppearWithin(TimeSpan timeout)
    {
        try
        {
            new WebDriverWait(Driver, timeout).Until(_ => VisibleSolutionLinks().Count > 0);
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }
}
