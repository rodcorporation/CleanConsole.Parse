using System;

namespace CleanConsole.Parse;

/// <summary>
/// Represents errors that occur during argument parsing or configuration validation.
/// </summary>
public class CleanParserException : Exception
{
    public CleanParserException(string message) : base(message)
    {
    }

    public CleanParserException(string message, Exception innerException) : base(message, innerException)
    {
    }
}