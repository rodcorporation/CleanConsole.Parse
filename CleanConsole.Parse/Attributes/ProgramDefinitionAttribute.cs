using System;

namespace CleanConsole.Parse;

/// <summary>
/// Defines metadata for the CLI application.
/// Must be applied to the class that holds the arguments.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ProgramDefinitionAttribute : Attribute
{
    /// <summary>
    /// The name of the application program.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A brief description of what the application does.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// If true, prints a summary of parsed options to the console after parsing.
    /// Default is false.
    /// </summary>
    public bool PrintSummary { get; set; } = false;
}