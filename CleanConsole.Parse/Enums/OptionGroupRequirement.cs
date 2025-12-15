namespace CleanConsole.Parse;

/// <summary>
/// Defines the validation rule for a group of options.
/// </summary>
public enum OptionGroupRequirement
{
    /// <summary>
    /// Exactly one option from the group must be provided.
    /// Error if 0 or >1 options are present.
    /// </summary>
    ExactOne,

    /// <summary>
    /// At least one option from the group must be provided.
    /// Error if 0 options are present. Multiple are allowed.
    /// </summary>
    AtLeastOne,

    /// <summary>
    /// No option from the group is mandatory. Any number of options (0 to N) can be provided.
    /// </summary>
    None,

    /// <summary>
    /// No option from the group is mandatory, but at most one option can be provided.
    /// Error if >1 options are present.
    /// </summary>
    AtMostOne
}
