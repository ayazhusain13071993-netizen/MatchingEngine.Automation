namespace MatchingEngine.Automation.Support;

public static class ExpectedData
{
    public const string SolutionsMenu = "Solutions";
    public const string DistributionProcessingLink = "Distribution Processing";
    public const string DistributionProcessingHeading = "Distribution processing";
    public const string AllInOneSection = "All-in-one solution for scale";

    public static readonly string[] Solutions =
    {
        "Repertoire management",
        "Repertoire and usage matching",
        "Data ingestion and integration",
        "Distribution processing",
        "Member management",
        "Member self service"
    };

    public static readonly Dictionary<string, string> AllInOneCards = new()
    {
        ["Prevent missing payments"] =
            "Matching Engine has advanced matching capability that ensures maximum automation to get money to Collective Management Organisation members quickly, while also preventing any missing payments.",
        ["Comply with international standards"] =
            "The system complies with international metadata and encoding standards (ISWC, ISRC, DDEX, Unicode), and supports multilingual and non-latin script works.",
        ["Run analytics and queries"] =
            "Collective Management Organisations can use Rest API to integrate other parts of their system. Its full data lakehouse gives access to underlying repertoire data to run analytics and queries.",
        ["Manage different collection share pictures"] =
            "Matching Engine is designed to manage different collection share pictures based on territories and right types. This includes advanced support for carve outs and carve ins.",
        ["Minimise manual intervention"] =
            "Advanced agreement support facilitates implementation of direct licensing carve outs and carve ins, without manual intervention on a work-by-work basis.",
        ["Tackle fluctuating data volumes"] =
            "The Matching Engine system is based on a modern cloud-based architecture that scales dynamically to support fluctuating data volumes for Collective Management Organisations."
    };
    public static readonly string[] AllInOneIntro =
{
    "Imagine an technology solution for collective management organisations that stays ahead of industry trends.",
    "With Matching Engine's distribution processing solution, you can:"
};

    public static readonly string[] AllInOneBullets =
    {
    "Distribute royalty payments quickly",
    "Provide full detail of music usage to members",
    "Reduce cost-to-distribution ratios."
};
}