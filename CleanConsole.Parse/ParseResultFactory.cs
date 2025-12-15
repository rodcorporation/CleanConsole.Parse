using System.Collections.Generic;

namespace CleanConsole.Parse;

/// <summary>
/// Provides centralized creation methods for ParseResult instances.
/// </summary>
internal static class ParseResultFactory
{
    /// <summary>
    /// Creates a successful parse result for the provided options instance.
    /// </summary>
    internal static ParseResult<T> Success<T>(T options) where T : class
    {
        return ParseResult<T>.Success(options);
    }

    /// <summary>
    /// Creates a help response parse result preserving the parsed options.
    /// </summary>
    internal static ParseResult<T> Help<T>(T? options, ParseHelpPayload help) where T : class
    {
        return ParseResult<T>.HelpResponse(options, help);
    }

    /// <summary>
    /// Creates an error parse result aggregating the provided errors.
    /// </summary>
    internal static ParseResult<T> Failure<T>(IEnumerable<ParseError> errors) where T : class
    {
        return ParseResult<T>.Failure(errors);
    }
}
