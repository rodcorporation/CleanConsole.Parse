using System;
using CleanConsole.Parse.Enums;

namespace CleanConsole.Parse.Attributes;

/// <summary>
/// Defines a logical group of options with specific validation rules.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class OptionGroupAttribute : Attribute
{
    /// <summary>
    /// The unique name of the group.
    /// Used to link properties via the [Option(Group = "Name")] property.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The validation rule to apply to this group.
    /// </summary>
    public OptionGroupType Type { get; set; }
}
