# Matching Engine – UI Automation (Selenium + C# + Reqnroll)

Automation solution for the Spanish Point QA Automation technical assessment.

**Stack:** C# / .NET 8 · Selenium WebDriver 4 · Reqnroll (BDD / Gherkin) · NUnit 4 · Google Chrome
**Pattern:** Page Object Model with a shared header component and explicit waits (no `Thread.Sleep`).

## Scenario covered

1. Visit https://www.matchingengine.com/
2. Expand **Solutions** in the header
3. Assert the list of solutions displayed
4. Click **Distribution processing**
5. Scroll to the **All-in-one solution for scale** section
6. Assert the section's content (card titles and descriptions)

The scenario lives in `MatchingEngine.Automation/Features/SolutionsNavigation.feature`.
Expected data sits in the Gherkin tables, so changing expectations never requires a code change.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (or newer)
- Google Chrome (the matching chromedriver is downloaded automatically by Selenium Manager)
- Visual Studio 2022 with the **Reqnroll for Visual Studio** extension (optional but recommended)

## Run the tests

**Visual Studio:** open `MatchingEngine.Automation.sln` → Build → *Test Explorer* → Run All.

**Command line:**

```bash
dotnet test
```

**Headless (e.g. CI):**

```bash
# PowerShell
$env:HEADLESS="true"; dotnet test
# bash
HEADLESS=true dotnet test
```

Settings are in `MatchingEngine.Automation/appsettings.json` and can be overridden with the
`BASE_URL` and `HEADLESS` environment variables.

## Project structure

```
MatchingEngine.Automation/
├── Features/            Gherkin feature files
├── StepDefinitions/     Step bindings (thin – they only call page objects and assert)
├── Pages/               BasePage, HeaderComponent, HomePage, DistributionProcessingPage
├── Hooks/               Browser start / stop, screenshot on failure
├── Support/             BrowserSession (Chrome), TestSettings, TextNormaliser
├── appsettings.json     Base URL, headless flag, timeout
└── reqnroll.json        Reqnroll configuration
```

## Design notes

- **Explicit waits only.** The site renders components lazily, so every interaction waits on a condition.
- **Text assertions use `textContent`**, normalised for whitespace, so CSS `text-transform` cannot cause false failures.
- **One browser per scenario**, created and disposed through Reqnroll context injection.
- **Failure screenshots** are saved to `bin/<config>/net8.0/screenshots` and attached to the NUnit result.
- **CI:** `.github/workflows/tests.yml` runs the suite in headless Chrome on every push and uploads results.

## Extending

Add a new scenario to the feature file, reuse existing steps, and add page objects under `Pages/`
for any new page. Locators are kept at the top of each page class so a markup change is a one-line fix.
