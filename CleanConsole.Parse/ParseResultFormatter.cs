using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CleanConsole.Parse;

/// <summary>
/// Provides textual formatting helpers for ParseResult content.
/// </summary>
internal static class ParseResultFormatter
{
    /// <summary>
    /// Builds the help description using the UX-approved layout.
    /// </summary>
    internal static string BuildHelpDescription(ParseHelpPayload help)
    {
        if (help == null) throw new ArgumentNullException(nameof(help));

        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(help.Title) || !string.IsNullOrWhiteSpace(help.Description))
        {
            if (!string.IsNullOrWhiteSpace(help.Title) && !string.IsNullOrWhiteSpace(help.Description))
            {
                sb.AppendLine($"{help.Title} - {help.Description}");
            }
            else
            {
                sb.AppendLine(help.Title ?? help.Description);
            }

            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(help.Usage))
        {
            sb.AppendLine("Usage:");
            sb.AppendLine($"  {help.Usage}");
            sb.AppendLine();
        }

        var groupedOptions = help.Options
            .Where(option => !string.IsNullOrEmpty(option.GroupName))
            .GroupBy(option => option.GroupName!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.OrdinalIgnoreCase);

        var ungroupedOptions = help.Options
            .Where(option => string.IsNullOrEmpty(option.GroupName) || !groupedOptions.ContainsKey(option.GroupName!))
            .ToList();

        if (ungroupedOptions.Count > 0)
        {
            sb.AppendLine("Options:");
            foreach (var option in ungroupedOptions)
            {
                sb.AppendLine(FormatOptionLine(option));
            }
            sb.AppendLine();
        }

        foreach (var group in help.Groups)
        {
            var requirementLabel = FormatRequirementLabel(group.Requirement);
            var groupDescription = string.IsNullOrWhiteSpace(group.Description) ? string.Empty : $" - {group.Description}";
            sb.AppendLine($"Group: {group.Name} (Requirement: {requirementLabel}){groupDescription}");

            if (groupedOptions.TryGetValue(group.Name, out var optionsInGroup))
            {
                foreach (var option in optionsInGroup)
                {
                    sb.AppendLine(FormatOptionLine(option));
                }
            }

            sb.AppendLine();
        }

        if (help.Examples.Count > 0)
        {
            sb.AppendLine("Examples:");
            foreach (var example in help.Examples)
            {
                sb.AppendLine($"> {example}");
            }
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    internal static string BuildSelectedSummary<T>(ParseResult<T> result) where T : class
    {
        if (result == null) throw new ArgumentNullException(nameof(result));

        if (result.SelectedOptions.Count == 0)
        {
            return "No options were selected.";
        }

        var sb = new StringBuilder();

        var ungrouped = result.SelectedOptions
            .Where(selection => string.IsNullOrEmpty(selection.GroupName))
            .ToList();

        if (ungrouped.Count > 0)
        {
            sb.AppendLine("Selected Options:");
            foreach (var option in ungrouped)
            {
                sb.AppendLine(FormatSelectionLine(option));
            }
        }

        var grouped = result.SelectedOptions
            .Where(selection => !string.IsNullOrEmpty(selection.GroupName))
            .GroupBy(selection => selection.GroupName!, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var group in grouped)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            sb.AppendLine($"Group: {group.Key}");
            foreach (var option in group)
            {
                sb.AppendLine(FormatSelectionLine(option));
            }
        }

        return sb.ToString().TrimEnd();
    }

    private static string FormatOptionLine(ParseHelpOption option)
    {
        var shortName = !string.IsNullOrEmpty(option.ShortName) ? $"-{option.ShortName}, " : "    ";
        var longName = $"--{option.LongName}{(option.RequiresValue ? " <value>" : string.Empty)}";

        var line = $"  {shortName}{longName}";
        if (line.Length < 30)
        {
            line = line.PadRight(30);
        }

        if (!string.IsNullOrEmpty(option.Description))
        {
            line += option.Description;
        }

        return line.TrimEnd();
    }

    private static string FormatSelectionLine(ParsedOptionSnapshot option)
    {
        var metadataSegments = new List<string>();

        if (!string.IsNullOrEmpty(option.ShortName))
        {
            metadataSegments.Add($"alias -{option.ShortName}");
        }

        if (!string.IsNullOrEmpty(option.GroupName))
        {
            metadataSegments.Add($"group {option.GroupName}");
        }

        var metadata = metadataSegments.Count > 0
            ? $" ({string.Join(", ", metadataSegments)})"
            : string.Empty;

        var valueText = FormatValue(option.Value, option.ValueType);

        return $"  --{option.LongName}{metadata} => {valueText}";
    }

    private static string FormatValue(object? value, Type valueType)
    {
        if (value is null)
        {
            return "null";
        }

        if (valueType == typeof(bool))
        {
            return ((bool)value) ? "true" : "false";
        }

        if (valueType == typeof(string))
        {
            var text = value as string ?? string.Empty;
            if (string.IsNullOrEmpty(text))
            {
                return "\"\"";
            }

            return text.Any(char.IsWhiteSpace) ? $"\"{text}\"" : text;
        }

        if (value is IFormattable formattable)
        {
            return formattable.ToString(null, CultureInfo.InvariantCulture);
        }

        return value.ToString() ?? string.Empty;
    }

    private static string FormatRequirementLabel(OptionGroupRequirement requirement)
    {
        return requirement switch
        {
            OptionGroupRequirement.All => "All (todas obrigatórias)",
            _ => requirement.ToString()
        };
    }
}
