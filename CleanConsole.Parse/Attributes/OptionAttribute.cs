using System;

namespace CleanConsole.Parse;

/// <summary>
/// Marks a property as a command-line option.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class OptionAttribute : Attribute
{
    /// <summary>
    /// The long name of the option (without prefixes like --).
    /// Example: "output" for --output.
    /// </summary>
    public string OptionName { get; }

    /// <summary>
    /// The short alias for the option (without prefixes like -).
    /// Example: "o" for -o.
    /// </summary>
    public string? ShortOptionName { get; set; }

    /// <summary>
    /// The name of the group this option belongs to.
    /// Must match an [OptionGroup] defined on the class.
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// A brief description of what this option does.
    /// Used when generating help text.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionAttribute"/> class.
    /// </summary>
    /// <param name="optionName">The mandatory long name of the option.</param>
    public OptionAttribute(string optionName)
    {
        OptionName = optionName;
    }
}