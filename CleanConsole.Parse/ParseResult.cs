using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanConsole.Parse;

/// <summary>
/// Represents the outcome of parsing command line arguments into an options object of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The configuration type requested by the consumer.</typeparam>
public sealed class ParseResult<T> where T : class
{
    private static readonly IReadOnlyList<ParseError> EmptyErrors = Array.Empty<ParseError>();
    private static readonly IReadOnlyList<ParsedOptionSnapshot> EmptySelections = Array.Empty<ParsedOptionSnapshot>();

    private ParseResult(
        T? options,
        bool helpRequested,
        ParseHelpPayload? help,
        IReadOnlyList<ParseError> errors,
        IReadOnlyList<ParsedOptionSnapshot> selections)
    {
        Options = options;
        HelpRequested = helpRequested;
        Help = help;
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        SelectedOptions = selections ?? throw new ArgumentNullException(nameof(selections));
    }

    /// <summary>
    /// Gets the parsed options instance when parsing succeeds or help is generated.
    /// </summary>
    public T? Options { get; }

    /// <summary>
    /// Provides an alias to <see cref="Options"/> to keep a value-oriented naming style.
    /// </summary>
    public T? Value => Options;

    /// <summary>
    /// Indicates whether the parser detected a help request instead of executing validation errors.
    /// </summary>
    public bool HelpRequested { get; }

    /// <summary>
    /// Indicates whether the parsing ended with at least one error.
    /// </summary>
    public bool HasErrors => Errors.Count > 0;

    /// <summary>
    /// Gets the collection of parse errors encountered during execution.
    /// </summary>
    public IReadOnlyList<ParseError> Errors { get; }

    /// <summary>
    /// Gets the help payload when the invocation corresponds to a help request.
    /// </summary>
    public ParseHelpPayload? Help { get; }

    /// <summary>
    /// Indicates if the parsing completed successfully without help or errors.
    /// </summary>
    public bool IsSuccess => !HelpRequested && !HasErrors;

    internal IReadOnlyList<ParsedOptionSnapshot> SelectedOptions { get; }

    /// <summary>
    /// Builds the formatted help description based on the stored help payload.
    /// </summary>
    public string GetHelpDescription()
    {
        if (Help is null)
        {
            return string.Empty;
        }

        return ParseResultFormatter.BuildHelpDescription(Help);
    }

    /// <summary>
    /// Builds the formatted summary of the selected options.
    /// </summary>
    public string GetSelectedSummary()
    {
        if (SelectedOptions.Count == 0)
        {
            return "No options were selected.";
        }

        return ParseResultFormatter.BuildSelectedSummary(this);
    }

    /// <summary>
    /// Enables implicit conversion to the options instance for backward compatibility scenarios.
    /// </summary>
    /// <param name="result">The parse result to extract the value from.</param>
    public static implicit operator T?(ParseResult<T> result)
    {
        if (result is null) throw new ArgumentNullException(nameof(result));
        return result.Options;
    }

    internal static ParseResult<T> Success(T options, IReadOnlyList<ParsedOptionSnapshot> selections)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        return new ParseResult<T>(options, false, null, EmptyErrors, selections ?? EmptySelections);
    }

    internal static ParseResult<T> HelpResponse(T? options, ParseHelpPayload help, IReadOnlyList<ParsedOptionSnapshot> selections)
    {
        if (help is null) throw new ArgumentNullException(nameof(help));
        return new ParseResult<T>(options, true, help, EmptyErrors, selections ?? EmptySelections);
    }

    internal static ParseResult<T> Failure(IEnumerable<ParseError> errors)
    {
        if (errors is null) throw new ArgumentNullException(nameof(errors));
        var materialized = errors as IReadOnlyList<ParseError> ?? errors.ToList();
        if (materialized.Count == 0)
        {
            throw new ArgumentException("At least one error is required to create a failure result.", nameof(errors));
        }

        return new ParseResult<T>(null, false, null, materialized, EmptySelections);
    }
}

/// <summary>
/// Represents a parsing error captured while processing command line arguments.
/// </summary>
public sealed class ParseError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParseError"/> class.
    /// </summary>
    /// <param name="kind">The category of the error.</param>
    /// <param name="message">The human-friendly error message.</param>
    /// <param name="optionName">Optional option identifier associated with the error.</param>
    public ParseError(ParseErrorKind kind, string message, string? optionName = null)
    {
        Kind = kind;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        OptionName = optionName;
    }

    /// <summary>
    /// Gets the category of the error.
    /// </summary>
    public ParseErrorKind Kind { get; }

    /// <summary>
    /// Gets the error message intended for the user.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the option identifier related to the error, when available.
    /// </summary>
    public string? OptionName { get; }
}

/// <summary>
/// Categorizes the different types of parse errors supported by the library.
/// </summary>
public enum ParseErrorKind
{
    /// <summary>
    /// Errors raised while validating option metadata before parsing.
    /// </summary>
    Configuration,

    /// <summary>
    /// Errors raised while converting raw values to their target types.
    /// </summary>
    Conversion,

    /// <summary>
    /// Errors raised while tokenizing or interpreting the command line syntax.
    /// </summary>
    Syntax,

    /// <summary>
    /// Errors triggered by option group rules (ExactOne, AtLeastOne, AtMostOne, All).
    /// </summary>
    GroupRule,

    /// <summary>
    /// Non-fatal errors that still allow help generation (e.g., missing configuration).
    /// </summary>
    HelpRequest
}

/// <summary>
/// Captures the structured help information associated with a parsing request.
/// </summary>
public sealed class ParseHelpPayload
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParseHelpPayload"/> class.
    /// </summary>
    /// <param name="title">The help title, usually sourced from ProgramDefinitionAttribute.</param>
    /// <param name="description">The help description or subtitle.</param>
    /// <param name="usage">The usage line displayed at the top of the help page.</param>
    /// <param name="options">The flat list of options available.</param>
    /// <param name="groups">The set of option groups and their metadata.</param>
    /// <param name="examples">Optional examples displayed at the bottom of the help text.</param>
    public ParseHelpPayload(
        string? title,
        string? description,
        string? usage,
        IReadOnlyList<ParseHelpOption> options,
        IReadOnlyList<ParseHelpGroup> groups,
        IReadOnlyList<string> examples)
    {
        Title = title;
        Description = description;
        Usage = usage;
        Options = options ?? throw new ArgumentNullException(nameof(options));
        Groups = groups ?? throw new ArgumentNullException(nameof(groups));
        Examples = examples ?? throw new ArgumentNullException(nameof(examples));
    }

    /// <summary>
    /// Gets the title displayed in the help output.
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// Gets the description displayed underneath the title.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the usage line.
    /// </summary>
    public string? Usage { get; }

    /// <summary>
    /// Gets the collection of options with metadata ready for rendering.
    /// </summary>
    public IReadOnlyList<ParseHelpOption> Options { get; }

    /// <summary>
    /// Gets the set of option groups that the options belong to.
    /// </summary>
    public IReadOnlyList<ParseHelpGroup> Groups { get; }

    /// <summary>
    /// Gets the ordered list of examples.
    /// </summary>
    public IReadOnlyList<string> Examples { get; }
}

/// <summary>
/// Represents a single option entry in the help payload.
/// </summary>
public sealed class ParseHelpOption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParseHelpOption"/> class.
    /// </summary>
    /// <param name="longName">The long name of the option (without prefixes).</param>
    /// <param name="shortName">The optional short alias of the option.</param>
    /// <param name="description">The human-friendly description.</param>
    /// <param name="groupName">The associated group name, when any.</param>
    /// <param name="requiresValue">Indicates whether the option expects a value.</param>
    public ParseHelpOption(
        string longName,
        string? shortName,
        string? description,
        string? groupName,
        bool requiresValue)
    {
        if (string.IsNullOrWhiteSpace(longName))
        {
            throw new ArgumentException("Long name cannot be null or whitespace.", nameof(longName));
        }

        LongName = longName;
        ShortName = shortName;
        Description = description;
        GroupName = groupName;
        RequiresValue = requiresValue;
    }

    /// <summary>
    /// Gets the long name of the option (without prefixes).
    /// </summary>
    public string LongName { get; }

    /// <summary>
    /// Gets the optional short alias of the option.
    /// </summary>
    public string? ShortName { get; }

    /// <summary>
    /// Gets the description rendered in help output.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the name of the group this option belongs to.
    /// </summary>
    public string? GroupName { get; }

    /// <summary>
    /// Gets a value indicating whether this option requires an explicit value.
    /// </summary>
    public bool RequiresValue { get; }
}

/// <summary>
/// Represents an option group entry in the help payload.
/// </summary>
public sealed class ParseHelpGroup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParseHelpGroup"/> class.
    /// </summary>
    /// <param name="name">The group name.</param>
    /// <param name="requirement">The requirement associated with the group.</param>
    /// <param name="description">The optional description of the group.</param>
    /// <param name="optionNames">The option names that belong to this group.</param>
    public ParseHelpGroup(
        string name,
        OptionGroupRequirement requirement,
        string? description,
        IReadOnlyList<string> optionNames)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Group name cannot be null or whitespace.", nameof(name));
        }

        Name = name;
        Requirement = requirement;
        Description = description;
        OptionNames = optionNames ?? throw new ArgumentNullException(nameof(optionNames));
    }

    /// <summary>
    /// Gets the group name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the requirement associated with the group.
    /// </summary>
    public OptionGroupRequirement Requirement { get; }

    /// <summary>
    /// Gets the description of the group.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the option names belonging to the group.
    /// </summary>
    public IReadOnlyList<string> OptionNames { get; }
}
