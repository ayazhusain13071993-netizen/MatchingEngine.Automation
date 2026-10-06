using MatchingEngine.Automation.Pages;
using MatchingEngine.Automation.Support;
using NUnit.Framework;
using Reqnroll;

namespace MatchingEngine.Automation.StepDefinitions;

[Binding]
public sealed class SolutionsNavigationSteps
{
    private readonly HomePage _homePage;
    private readonly HeaderComponent _header;
    private readonly DistributionProcessingPage _distributionPage;

    public SolutionsNavigationSteps(HomePage homePage, HeaderComponent header, DistributionProcessingPage distributionPage)
    {
        _homePage = homePage;
        _header = header;
        _distributionPage = distributionPage;
    }

    [Given("I am on the Matching Engine home page")]
    public void GivenIAmOnTheHomePage()
    {
        _homePage.Open();
        Assert.That(_homePage.Title, Does.Contain("Matching Engine"));
    }

    [When("I expand the Solutions menu")]
    public void WhenIExpandTheSolutionsMenu()
    {
        _header.ExpandMenu(ExpectedData.SolutionsMenu);
    }

    [Then("I should see the expected list of solutions")]
    public void ThenIShouldSeeTheExpectedSolutions()
    {
        Assert.That(_header.GetDisplayedSolutions(), Is.EqualTo(ExpectedData.Solutions),
            "The Solutions list shown in the header does not match the expected list");
    }

    [When("I click on Distribution Processing")]
    public void WhenIClickOnDistributionProcessing()
    {
        _header.ClickSolution(ExpectedData.DistributionProcessingLink);
    }

    [Then("I should be on the Distribution processing page")]
    public void ThenIShouldBeOnTheDistributionProcessingPage()
    {
        _distributionPage.WaitUntilLoaded(ExpectedData.DistributionProcessingHeading);
    }

    [When("I scroll to the All-in-one solution for scale section")]
    public void WhenIScrollToTheAllInOneSection()
    {
        _distributionPage.ScrollToSection(ExpectedData.AllInOneSection);
        Assert.That(_distributionPage.IsSectionInView(ExpectedData.AllInOneSection), Is.True,
            "The section is not in the viewport after scrolling");
    }
    [Then("I should see the expected All-in-one solution for scale introduction")]
    public void ThenIShouldSeeTheExpectedIntroduction()
    {
        Assert.Multiple(() =>
        {
            foreach (var text in ExpectedData.AllInOneIntro.Concat(ExpectedData.AllInOneBullets))
            {
                Assert.That(_distributionPage.IsTextDisplayed(text), Is.True,
                    $"Expected text not displayed on the page: '{text}'");
            }
        });
    }
    [Then("I should see the expected All-in-one solution for scale content")]
    public void ThenIShouldSeeTheExpectedContent()
    {
        Assert.Multiple(() =>
        {
            foreach (var expected in ExpectedData.AllInOneCards)
            {
                var card = _distributionPage.GetCard(ExpectedData.AllInOneSection, expected.Key);

                Assert.That(card.IsDisplayed, Is.True, $"Card '{expected.Key}' is not displayed");
                Assert.That(card.Description, Is.EqualTo(expected.Value),
                    $"Description of card '{expected.Key}' does not match");
            }
        });
    }
}