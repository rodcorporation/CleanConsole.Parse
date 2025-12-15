using System;

namespace CleanConsole.Parse;

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
    public string Name { get; }

    /// <summary>
    /// The validation rule to apply to this group.
    /// </summary>
    public OptionGroupRequirement Require { get; }

    /// <summary>
    /// A brief description of what this group represents.
    /// Used when generating help text.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionGroupAttribute"/> class.
    /// </summary>
    /// <param name="name">The unique name of the group.</param>
    /// <param name="require">The validation rule to apply to this group.</param>
    public OptionGroupAttribute(string name, OptionGroupRequirement require)
    {
        Name = name;
        Require = require;
    }
}
