using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;

namespace CleanConsole.Parse;

/// <summary>
/// The main entry point for parsing command line arguments.
/// </summary>
public static class CleanParser
{
    private static readonly HashSet<Type> SupportedTypes = new()
    {
        typeof(string),
        typeof(int),
        typeof(double),
        typeof(bool)
    };

    private static Func<string[]> _commandLineArgsProvider = () => Environment.GetCommandLineArgs();

    internal static Func<string[]> CommandLineArgsProvider
    {
        get => _commandLineArgsProvider;
        set => _commandLineArgsProvider = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Parses the current process command line arguments into an instance of type T.
    /// </summary>
    /// <typeparam name="T">The type to parse into. Must be a class with a parameterless constructor.</typeparam>
    /// <returns>A ParseResult containing the populated instance of T.</returns>
    /// <exception cref="CleanParserException">Thrown when configuration is invalid or parsing fails.</exception>
    public static ParseResult<T> Parse<T>() where T : class, new()
    {
        var args = CommandLineArgsProvider.Invoke() ?? Array.Empty<string>();
        var effectiveArgs = args.Length > 0 && Path.IsPathRooted(args[0])
            ? args.Skip(1).ToArray()
            : args;

        return Parse<T>(effectiveArgs);
    }

    /// <summary>
    /// Parses the command line arguments into an instance of type T.
    /// Performs validation on the configuration of T before parsing.
    /// </summary>
    /// <typeparam name="T">The type to parse into. Must be a class with a parameterless constructor.</typeparam>
    /// <param name="args">The command line arguments.</param>
    /// <returns>A ParseResult containing the populated instance of T.</returns>
    /// <exception cref="CleanParserException">Thrown when configuration is invalid or parsing fails.</exception>
    public static ParseResult<T> Parse<T>(string[] args) where T : class, new()
    {
        var type = typeof(T);
        var metadata = BuildOptionMetadata(type);

        ValidateConfiguration(metadata);

        var tokens = Tokenize(args);
        var instance = new T();

        // Rastreamento para validação de grupos
        var groupCounts = metadata.Groups.ToDictionary(g => g.Key, _ => 0);
        var groupMissingOptions = metadata.Groups.ToDictionary(
            g => g.Key,
            g => new HashSet<OptionDescriptor>(g.Value.Options));

        foreach (var option in metadata.Options)
        {
            var optionAttr = option.Attribute;

            var match = tokens.LastOrDefault(t =>
                string.Equals(t.Key, optionAttr.OptionName, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(optionAttr.ShortOptionName) && string.Equals(t.Key, optionAttr.ShortOptionName, StringComparison.OrdinalIgnoreCase))
            );

            if (match.Key == null) continue;

            if (!string.IsNullOrEmpty(optionAttr.Group) && groupCounts.ContainsKey(optionAttr.Group))
            {
                groupCounts[optionAttr.Group]++;
                if (groupMissingOptions.TryGetValue(optionAttr.Group, out var missingSet))
                {
                    missingSet.Remove(option);
                }
            }

            if (option.Property.PropertyType == typeof(bool))
            {
                if (match.Value == null)
                {
                    option.Property.SetValue(instance, true);
                }
                else
                {
                    if (bool.TryParse(match.Value, out bool boolResult))
                    {
                        option.Property.SetValue(instance, boolResult);
                    }
                    else
                    {
                        throw new CleanParserException($"O valor '{match.Value}' não é válido para o argumento '{optionAttr.OptionName}'. Esperava-se um 'Boolean'.");
                    }
                }
            }
            else
            {
                if (match.Value == null)
                {
                    throw new CleanParserException($"O argumento '{match.Key}' exige um valor, mas nenhum foi fornecido.");
                }

                try
                {
                    object? convertedValue = null;

                    if (option.Property.PropertyType == typeof(string))
                    {
                        convertedValue = match.Value;
                    }
                    else if (option.Property.PropertyType == typeof(int))
                    {
                        if (int.TryParse(match.Value, out int intResult))
                        {
                            convertedValue = intResult;
                        }
                        else
                        {
                            throw new CleanParserException($"O valor '{match.Value}' não é válido para o argumento '{optionAttr.OptionName}'. Esperava-se um 'Int32'.");
                        }
                    }
                    else if (option.Property.PropertyType == typeof(double))
                    {
                        if (double.TryParse(match.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double doubleResult))
                        {
                            convertedValue = doubleResult;
                        }
                        else
                        {
                            throw new CleanParserException($"O valor '{match.Value}' não é válido para o argumento '{optionAttr.OptionName}'. Esperava-se um 'Double'.");
                        }
                    }

                    option.Property.SetValue(instance, convertedValue);
                }
                catch (Exception ex) when (ex is not CleanParserException)
                {
                    throw new CleanParserException($"Erro ao definir valor para '{optionAttr.OptionName}': {ex.Message}", ex);
                }
            }
        }

        // 6. Validação de Regras de Negócio (Grupos)
        foreach (var groupEntry in metadata.Groups)
        {
            var groupAttr = groupEntry.Value.Attribute;
            int count = groupCounts[groupAttr.Name];

            switch (groupAttr.Require)
            {
                case OptionGroupRequirement.ExactOne:
                    if (count != 1)
                    {
                        throw new CleanParserException($"Conflito de opções: O grupo '{groupAttr.Name}' exige exatamente uma opção, mas foram fornecidas: {count}.");
                    }
                    break;

                case OptionGroupRequirement.AtLeastOne:
                    if (count == 0)
                    {
                        throw new CleanParserException($"Requisito não atendido: Pelo menos uma opção do grupo '{groupAttr.Name}' deve ser fornecida.");
                    }
                    break;

                case OptionGroupRequirement.AtMostOne:
                    if (count > 1)
                    {
                        throw new CleanParserException($"Conflito de opções: O grupo '{groupAttr.Name}' permite no máximo uma opção, mas foram fornecidas: {count}.");
                    }
                    break;

                case OptionGroupRequirement.All:
                    var missing = groupMissingOptions[groupAttr.Name];
                    if (missing.Count > 0)
                    {
                        var missingList = string.Join(", ", missing.Select(o => $"--{o.Attribute.OptionName}"));
                        throw new CleanParserException($"Requisito não atendido: Todas as opções do grupo '{groupAttr.Name}' devem ser fornecidas. Ausentes: {missingList}.");
                    }
                    break;

                case OptionGroupRequirement.None:
                default:
                    break;
            }
        }

        // 7.3 Implementar PrintSummary
        var programDef = type.GetCustomAttribute<ProgramDefinitionAttribute>();
        if (programDef != null && programDef.PrintSummary)
        {
            PrintSummary(instance, metadata.Options);
        }

        return ParseResultFactory.Success(instance);
    }

    /// <summary>
    /// Generates a help text based on the attributes defined in type T.
    /// </summary>
    /// <typeparam name="T">The configuration type.</typeparam>
    /// <returns>A formatted help string.</returns>
    public static string GetHelpText<T>()
    {
        var type = typeof(T);
        var metadata = BuildOptionMetadata(type);
        ValidateConfiguration(metadata);
        var programDef = type.GetCustomAttribute<ProgramDefinitionAttribute>();
        var sb = new StringBuilder();

        if (programDef != null)
        {
            if (!string.IsNullOrEmpty(programDef.Name))
                sb.AppendLine(programDef.Name);
            
            if (!string.IsNullOrEmpty(programDef.Description))
                sb.AppendLine(programDef.Description);
            
            sb.AppendLine();
        }

        var groupedOptions = metadata.Groups
            .Where(g => g.Value.Options.Count > 0)
            .ToDictionary(g => g.Key, g => g.Value.Options);

        var ungroupedOptions = metadata.Options
            .Where(opt => string.IsNullOrEmpty(opt.Attribute.Group) || !groupedOptions.ContainsKey(opt.Attribute.Group!))
            .ToList();

        if (ungroupedOptions.Any())
        {
            sb.AppendLine("Options:");
            foreach (var option in ungroupedOptions)
            {
                sb.AppendLine(FormatOptionLine(option));
            }
            sb.AppendLine();
        }

        foreach (var groupName in groupedOptions.Keys)
        {
            var group = metadata.Groups[groupName];
            var groupAttr = group.Attribute;
            var groupDesc = !string.IsNullOrEmpty(groupAttr.Description) ? $" - {groupAttr.Description}" : "";
            var requirementLabel = FormatRequirementLabel(groupAttr.Require);
            sb.AppendLine($"Group: {groupName} (Requirement: {requirementLabel}){groupDesc}");
            
            foreach (var option in groupedOptions[groupName])
            {
                sb.AppendLine(FormatOptionLine(option));
            }
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    private static string FormatRequirementLabel(OptionGroupRequirement requirement)
    {
        return requirement switch
        {
            OptionGroupRequirement.All => "All (todas obrigatórias)",
            _ => requirement.ToString()
        };
    }

    private static string FormatOptionLine(OptionDescriptor option)
    {
        var opt = option.Attribute;
        var shortName = !string.IsNullOrEmpty(opt.ShortOptionName) ? $"-{opt.ShortOptionName}, " : "    ";
        var longName = $"--{opt.OptionName}";
        
        var line = $"  {shortName}{longName}";
        
        // Padding para alinhar
        if (line.Length < 30)
            line = line.PadRight(30);
        
        if (!string.IsNullOrEmpty(opt.Description))
        {
            line += $"{opt.Description}";
        }

        return line;
    }

    private static void PrintSummary<T>(T instance, IReadOnlyList<OptionDescriptor> options)
    {
        Console.WriteLine("Summary of Options:");
        foreach (var option in options)
        {
            var value = option.Property.GetValue(instance);
            Console.WriteLine($"  {option.Attribute.OptionName}: {value}");
        }
        Console.WriteLine();
    }

    private static OptionMetadata BuildOptionMetadata(Type type)
    {
        var groupAttributes = type.GetCustomAttributes<OptionGroupAttribute>().ToList();
        var groups = new Dictionary<string, GroupDescriptor>();

        foreach (var groupAttr in groupAttributes)
        {
            if (!groups.ContainsKey(groupAttr.Name))
            {
                groups[groupAttr.Name] = new GroupDescriptor(groupAttr);
            }
        }

        var options = new List<OptionDescriptor>();

        foreach (var property in type.GetProperties())
        {
            var optionAttr = property.GetCustomAttribute<OptionAttribute>();
            if (optionAttr == null) continue;

            var descriptor = new OptionDescriptor(property, optionAttr);
            options.Add(descriptor);

            if (!string.IsNullOrEmpty(optionAttr.Group) && groups.TryGetValue(optionAttr.Group, out var group))
            {
                group.Options.Add(descriptor);
            }
        }

        return new OptionMetadata(type, options, groupAttributes, groups);
    }

    private sealed class OptionMetadata
    {
        internal OptionMetadata(Type type, IReadOnlyList<OptionDescriptor> options, IReadOnlyList<OptionGroupAttribute> groupAttributes, IReadOnlyDictionary<string, GroupDescriptor> groups)
        {
            Type = type;
            Options = options;
            GroupAttributes = groupAttributes;
            Groups = groups;
        }

        internal Type Type { get; }
        internal IReadOnlyList<OptionDescriptor> Options { get; }
        internal IReadOnlyList<OptionGroupAttribute> GroupAttributes { get; }
        internal IReadOnlyDictionary<string, GroupDescriptor> Groups { get; }
    }

    private sealed class OptionDescriptor
    {
        internal OptionDescriptor(PropertyInfo property, OptionAttribute attribute)
        {
            Property = property;
            Attribute = attribute;
        }

        internal PropertyInfo Property { get; }
        internal OptionAttribute Attribute { get; }
    }

    private sealed class GroupDescriptor
    {
        internal GroupDescriptor(OptionGroupAttribute attribute)
        {
            Attribute = attribute;
            Options = new List<OptionDescriptor>();
        }

        internal OptionGroupAttribute Attribute { get; }
        internal List<OptionDescriptor> Options { get; }
    }

    /// <summary>
    /// Validates the configuration of the target type T.
    /// Checks for supported types, duplicate names, and group integrity.
    /// </summary>
    private static void ValidateConfiguration(OptionMetadata metadata)
    {
        var type = metadata.Type;

        var groupNames = metadata.GroupAttributes.Select(g => g.Name).ToList();
        if (groupNames.Count != groupNames.Distinct().Count())
        {
            throw new CleanParserException($"Configuration Error: Duplicate [OptionGroup] names found on type '{type.Name}'.");
        }

        var optionNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var shortOptionNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var option in metadata.Options)
        {
            var prop = option.Property;
            var optionAttr = option.Attribute;

            if (!SupportedTypes.Contains(prop.PropertyType))
            {
                throw new CleanParserException($"Configuration Error: Property '{prop.Name}' has unsupported type '{prop.PropertyType.Name}'. Supported types are: string, int, double, bool.");
            }

            ValidateNameFormat(optionAttr.OptionName, "OptionName");
            if (!string.IsNullOrEmpty(optionAttr.ShortOptionName))
            {
                ValidateNameFormat(optionAttr.ShortOptionName, "ShortOptionName");
            }

            if (!optionNames.Add(optionAttr.OptionName))
            {
                throw new CleanParserException($"Configuration Error: Duplicate OptionName '{optionAttr.OptionName}' found.");
            }

            if (!string.IsNullOrEmpty(optionAttr.ShortOptionName))
            {
                if (!shortOptionNames.Add(optionAttr.ShortOptionName))
                {
                    throw new CleanParserException($"Configuration Error: Duplicate ShortOptionName '{optionAttr.ShortOptionName}' found.");
                }
            }

            if (!string.IsNullOrEmpty(optionAttr.Group))
            {
                if (!metadata.Groups.ContainsKey(optionAttr.Group))
                {
                    throw new CleanParserException($"Configuration Error: Property '{prop.Name}' references undefined group '{optionAttr.Group}'.");
                }
            }
        }

        foreach (var group in metadata.Groups.Values)
        {
            if (group.Attribute.Require == OptionGroupRequirement.All && group.Options.Count == 0)
            {
                throw new CleanParserException($"Configuration Error: Group '{group.Attribute.Name}' is marked as 'All' but does not contain any [Option] members.");
            }
        }
    }

    private static void ValidateNameFormat(string name, string context)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new CleanParserException($"Configuration Error: {context} cannot be empty.");

        if (name.StartsWith("-") || name.StartsWith("/"))
            throw new CleanParserException($"Configuration Error: {context} '{name}' must not start with prefixes like '-' or '/'.");

        if (name.Contains(" ") || name.Contains(":") || name.Contains("="))
            throw new CleanParserException($"Configuration Error: {context} '{name}' contains invalid characters (spaces or separators).");
    }

    private static List<(string Key, string? Value)> Tokenize(string[] args)
    {
        var tokens = new List<(string Key, string? Value)>();

        foreach (var arg in args)
        {
            if (string.IsNullOrWhiteSpace(arg)) continue;

            // RF04 - Proibição de Espaços / Validação de Prefixo
            if (!arg.StartsWith("-") && !arg.StartsWith("/"))
            {
                 throw new CleanParserException($"Erro de sintaxe no argumento '{arg}'. Espaços não são permitidos. Use o formato 'Opcao:valor' ou 'Opcao=valor' para corrigir.");
            }

            // Normalizar Prefixo e Extrair Key
            var cleanArg = arg;
            if (cleanArg.StartsWith("--")) cleanArg = cleanArg.Substring(2);
            else if (cleanArg.StartsWith("-")) cleanArg = cleanArg.Substring(1);
            else if (cleanArg.StartsWith("/")) cleanArg = cleanArg.Substring(1);

            // Split Key/Value
            string key;
            string? value = null;

            int sepIndex = cleanArg.IndexOfAny(new[] { ':', '=' });
            if (sepIndex >= 0)
            {
                key = cleanArg.Substring(0, sepIndex);
                value = cleanArg.Substring(sepIndex + 1);

                // Remove aspas do valor (Sanitização)
                if (!string.IsNullOrEmpty(value) && value.Length >= 2 && value.StartsWith("\"") && value.EndsWith("\""))
                {
                    value = value.Substring(1, value.Length - 2);
                }
            }
            else
            {
                key = cleanArg;
            }

            tokens.Add((key, value));
        }

        return tokens;
    }
}
