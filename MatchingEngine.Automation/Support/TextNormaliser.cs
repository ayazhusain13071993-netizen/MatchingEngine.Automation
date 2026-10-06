using System.Text.RegularExpressions;

namespace MatchingEngine.Automation.Support;

public static class TextNormaliser
{
    /// <summary>Collapses all whitespace (incl. non-breaking spaces and new lines) to single spaces.</summary>
    public static string Normalise(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        text = text.Replace('\u00A0', ' ').Replace('\u2019', '\'').Replace('\u2018', '\'');
        return Regex.Replace(text, @"\s+", " ").Trim();
    }

    public static bool Equal(string actual, string expected) =>
        string.Equals(Normalise(actual), Normalise(expected), StringComparison.OrdinalIgnoreCase);

    /// <summary>Safely quotes a value for use inside an XPath expression.</summary>
    public static string XPathLiteral(string value)
    {
        if (!value.Contains('\'')) return $"'{value}'";
        if (!value.Contains('"')) return $"\"{value}\"";
        throw new ArgumentException("Values containing both quote types are not supported.", nameof(value));
    }
}
