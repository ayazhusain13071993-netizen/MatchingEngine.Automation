using MatchingEngine.Automation.Support;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MatchingEngine.Automation.Pages;

/// <summary>A titled content card, e.g. "Prevent missing payments" + its description.</summary>
public sealed record ContentCard(string Title, string Description, bool IsDisplayed);

public sealed class DistributionProcessingPage : BasePage
{
    private const string UrlFragment = "Distribution-processing";

    public DistributionProcessingPage(BrowserSession session) : base(session)
    {
    }

    public void WaitUntilLoaded(string expectedHeading)
    {
        Wait.Message = $"Timed out waiting for the '{expectedHeading}' page URL";
        Wait.Until(d => d.Url.IndexOf(UrlFragment, StringComparison.OrdinalIgnoreCase) >= 0);
        WaitForPageReady();
        WaitForHeading(expectedHeading);
    }

    public void ScrollToSection(string sectionHeading)
    {
        var heading = WaitForHeading(sectionHeading);
        ScrollIntoView(heading);

        Wait.Message = $"Timed out waiting for '{sectionHeading}' to scroll into view";
        Wait.Until(_ => IsInViewport(heading));
    }

    public bool IsSectionInView(string sectionHeading) => IsInViewport(WaitForHeading(sectionHeading));

    /// <summary>
    /// Returns the card with the given title that sits under the section heading.
    /// The page renders cards lazily ("Loading component..."), so this polls until the card
    /// and its description are present.
    /// </summary>
    public ContentCard GetCard(string sectionHeading, string cardTitle)
    {
        Wait.Message = $"Timed out waiting for card '{cardTitle}' in section '{sectionHeading}'";
        return Wait.Until(_ => TryReadCard(sectionHeading, cardTitle));
    }
    public bool IsTextDisplayed(string text)
    {
        try
        {
            Wait.Message = $"Timed out waiting for text '{text}'";
            return Wait.Until(_ =>
            {
                var texts = (IReadOnlyCollection<object>)((IJavaScriptExecutor)Driver).ExecuteScript(
                    "return Array.from(document.querySelectorAll('p,li,strong,span,h2,h3,h4'))" +
                    ".filter(e => e.offsetParent !== null).map(e => e.textContent);");

                return texts.Any(t => TextNormaliser.Equal(t?.ToString(), text));
            });
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }
    private ContentCard TryReadCard(string sectionHeading, string cardTitle)
    {
        var sectionHeadingElement = FindHeading(sectionHeading);
        if (sectionHeadingElement == null)
        {
            return null;
        }

        // Card titles are headings that come after the section heading in the DOM.
        var title = sectionHeadingElement
            .FindElements(By.XPath("following::*[self::h3 or self::h4 or self::h5 or self::h6]"))
            .FirstOrDefault(h => h.Displayed && TextNormaliser.Equal(TextOf(h), cardTitle));
        if (title == null)
        {
            return null;
        }

        // Walk up to the first ancestor that holds more text than the title: that is the card.
        var titleText = TextOf(title);
        var current = title;
        for (var level = 0; level < 4; level++)
        {
            current = current.FindElement(By.XPath("./.."));
            var cardText = TextOf(current);
            if (cardText.Length > titleText.Length)
            {
                var description = cardText.StartsWith(titleText, StringComparison.OrdinalIgnoreCase)
                    ? cardText.Substring(titleText.Length).Trim()
                    : cardText.Replace(titleText, string.Empty).Trim();

                return new ContentCard(titleText, description, title.Displayed && current.Displayed);
            }
        }

        return null; // description not rendered yet - keep polling
    }
}
