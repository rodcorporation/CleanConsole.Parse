using System;

namespace CleanConsole.Parse;

/// <summary>
/// Captures a snapshot of a parsed option and its resulting value.
/// </summary>
internal sealed class ParsedOptionSnapshot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedOptionSnapshot"/> class.
    /// </summary>
    internal ParsedOptionSnapshot(
        string longName,
        string? shortName,
        string? groupName,
        object? value,
        Type valueType,
        bool requiresValue)
    {
        LongName = longName;
        ShortName = shortName;
        GroupName = groupName;
        Value = value;
        ValueType = valueType;
        RequiresValue = requiresValue;
    }

    internal string LongName { get; }

    internal string? ShortName { get; }

    internal string? GroupName { get; }

    internal object? Value { get; }

    internal Type ValueType { get; }

    internal bool RequiresValue { get; }
}
