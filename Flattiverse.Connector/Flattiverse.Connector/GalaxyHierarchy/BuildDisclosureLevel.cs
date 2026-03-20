namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Highest disclosed build-assistance level for one aspect.
/// </summary>
public enum BuildDisclosureLevel : byte
{
    /// <summary>
    /// No search engine or LLM help was used.
    /// </summary>
    None = 0,

    /// <summary>
    /// Search engines or documentation were used without LLMs.
    /// </summary>
    SearchOnly = 1,

    /// <summary>
    /// Free-tier LLMs were used.
    /// </summary>
    FreeLlm = 2,

    /// <summary>
    /// Paid chat-grade LLMs were used.
    /// </summary>
    PaidLlm = 3,

    /// <summary>
    /// Editor-integrated LLM tooling was used.
    /// </summary>
    IntegratedLlm = 4,

    /// <summary>
    /// Agentic coding tools such as Codex or Claude Code were used.
    /// </summary>
    AgenticTool = 5,
}
