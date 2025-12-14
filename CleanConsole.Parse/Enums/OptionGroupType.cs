namespace CleanConsole.Parse.Enums;

/// <summary>
/// Defines the validation rule for a group of options.
/// </summary>
public enum OptionGroupType
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
    AtLeastOne
}
