using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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

    /// <summary>
    /// Parses the command line arguments into an instance of type T.
    /// Performs validation on the configuration of T before parsing.
    /// </summary>
    /// <typeparam name="T">The type to parse into. Must be a class with a parameterless constructor.</typeparam>
    /// <param name="args">The command line arguments.</param>
    /// <returns>A populated instance of T.</returns>
    /// <exception cref="CleanParserException">Thrown when configuration is invalid or parsing fails.</exception>
    public static T Parse<T>(string[] args) where T : class, new()
    {
        ValidateConfiguration<T>();

        var tokens = Tokenize(args);
        var instance = new T();
        var type = typeof(T);
        var properties = type.GetProperties();
        var definedGroups = type.GetCustomAttributes<OptionGroupAttribute>().ToList();
        
        // Rastreamento para validação de grupos
        // Key: GroupName, Value: Count of options set
        var groupCounts = definedGroups.ToDictionary(g => g.Name, g => 0);

        // Map tokens to properties
        foreach (var prop in properties)
        {
            var optionAttr = prop.GetCustomAttribute<OptionAttribute>();
            if (optionAttr == null) continue;

            // Encontrar o último token que corresponde a esta opção (Last Wins)
            var match = tokens.LastOrDefault(t => 
                string.Equals(t.Key, optionAttr.OptionName, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(optionAttr.ShortOptionName) && string.Equals(t.Key, optionAttr.ShortOptionName, StringComparison.OrdinalIgnoreCase))
            );

            // Se não encontrou token para esta opção, pula (mantém valor default)
            // Note: tokens é List<(string Key, string? Value)>, default é (null, null) se struct, mas LastOrDefault retorna default(ValueTuple) que é (null, null).
            // Precisamos verificar se Key não é null.
            if (match.Key == null) continue;

            // Increment group count if applicable
            if (!string.IsNullOrEmpty(optionAttr.Group))
            {
                if (groupCounts.ContainsKey(optionAttr.Group))
                {
                    groupCounts[optionAttr.Group]++;
                }
            }

            // 5.6 Lógica Bool/Flag e 5.7 Validar Valor Ausente
            if (prop.PropertyType == typeof(bool))
            {
                if (match.Value == null)
                {
                    // Flag presente sem valor explícito = true
                    prop.SetValue(instance, true);
                }
                else
                {
                    // Flag com valor explícito (ex: --verbose:false)
                    if (bool.TryParse(match.Value, out bool boolResult))
                    {
                        prop.SetValue(instance, boolResult);
                    }
                    else
                    {
                        throw new CleanParserException($"O valor '{match.Value}' não é válido para o argumento '{optionAttr.OptionName}'. Esperava-se um 'Boolean'.");
                    }
                }
            }
            else
            {
                // Para não-booleanos, valor é obrigatório se a chave foi passada
                if (match.Value == null)
                {
                    throw new CleanParserException($"O argumento '{match.Key}' exige um valor, mas nenhum foi fornecido.");
                }

                // Conversão de Tipos
                try
                {
                    object? convertedValue = null;

                    if (prop.PropertyType == typeof(string))
                    {
                        convertedValue = match.Value;
                    }
                    else if (prop.PropertyType == typeof(int))
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
                    else if (prop.PropertyType == typeof(double))
                    {
                        // 5.5 Conversão Double (Invariant Culture)
                        if (double.TryParse(match.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double doubleResult))
                        {
                            convertedValue = doubleResult;
                        }
                        else
                        {
                            throw new CleanParserException($"O valor '{match.Value}' não é válido para o argumento '{optionAttr.OptionName}'. Esperava-se um 'Double'.");
                        }
                    }

                    prop.SetValue(instance, convertedValue);
                }
                catch (Exception ex) when (ex is not CleanParserException)
                {
                    // Caso genérico de erro de set (improvável com os checks acima, mas segurança)
                    throw new CleanParserException($"Erro ao definir valor para '{optionAttr.OptionName}': {ex.Message}", ex);
                }
            }
        }

        // 6. Validação de Regras de Negócio (Grupos)
        foreach (var group in definedGroups)
        {
            int count = groupCounts[group.Name];

            switch (group.Require)
            {
                case OptionGroupRequirement.ExactOne:
                    if (count != 1)
                    {
                        throw new CleanParserException($"Conflito de opções: O grupo '{group.Name}' exige exatamente uma opção, mas foram fornecidas: {count}.");
                    }
                    break;

                case OptionGroupRequirement.AtLeastOne:
                    if (count == 0)
                    {
                        throw new CleanParserException($"Requisito não atendido: Pelo menos uma opção do grupo '{group.Name}' deve ser fornecida.");
                    }
                    break;

                case OptionGroupRequirement.AtMostOne:
                    if (count > 1)
                    {
                        throw new CleanParserException($"Conflito de opções: O grupo '{group.Name}' permite no máximo uma opção, mas foram fornecidas: {count}.");
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
            PrintSummary(instance, properties);
        }

        return instance;
    }

    /// <summary>
    /// Generates a help text based on the attributes defined in type T.
    /// </summary>
    /// <typeparam name="T">The configuration type.</typeparam>
    /// <returns>A formatted help string.</returns>
    public static string GetHelpText<T>()
    {
        var type = typeof(T);
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

        sb.AppendLine("Options:");

        var properties = type.GetProperties();
        foreach (var prop in properties)
        {
            var opt = prop.GetCustomAttribute<OptionAttribute>();
            if (opt == null) continue;

            var shortName = !string.IsNullOrEmpty(opt.ShortOptionName) ? $"-{opt.ShortOptionName}, " : "    ";
            var longName = $"--{opt.OptionName}";
            
            var line = $"  {shortName}{longName}";
            
            // Padding para alinhar
            if (line.Length < 30)
                line = line.PadRight(30);
            
            if (!string.IsNullOrEmpty(opt.Group))
            {
                line += $" [Group: {opt.Group}]";
            }

            sb.AppendLine(line);
        }

        return sb.ToString();
    }

    private static void PrintSummary<T>(T instance, PropertyInfo[] properties)
    {
        Console.WriteLine("Summary of Options:");
        foreach (var prop in properties)
        {
            var opt = prop.GetCustomAttribute<OptionAttribute>();
            if (opt == null) continue;

            var value = prop.GetValue(instance);
            Console.WriteLine($"  {opt.OptionName}: {value}");
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Validates the configuration of the target type T.
    /// Checks for supported types, duplicate names, and group integrity.
    /// </summary>
    private static void ValidateConfiguration<T>()
    {
        var type = typeof(T);
        var properties = type.GetProperties();
        var definedGroups = type.GetCustomAttributes<OptionGroupAttribute>().ToList();

        // 3.6 Validar Duplicidade de Grupos
        var groupNames = definedGroups.Select(g => g.Name).ToList();
        if (groupNames.Count != groupNames.Distinct().Count())
        {
            throw new CleanParserException($"Configuration Error: Duplicate [OptionGroup] names found on type '{type.Name}'.");
        }

        var optionNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var shortOptionNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var prop in properties)
        {
            var optionAttr = prop.GetCustomAttribute<OptionAttribute>();
            if (optionAttr == null) continue;

            // 3.3 Validar Tipos Suportados
            if (!SupportedTypes.Contains(prop.PropertyType))
            {
                throw new CleanParserException($"Configuration Error: Property '{prop.Name}' has unsupported type '{prop.PropertyType.Name}'. Supported types are: string, int, double, bool.");
            }

            // 3.7 Validar Formato dos Nomes
            ValidateNameFormat(optionAttr.OptionName, "OptionName");
            if (!string.IsNullOrEmpty(optionAttr.ShortOptionName))
            {
                ValidateNameFormat(optionAttr.ShortOptionName, "ShortOptionName");
            }

            // 3.4 Validar Duplicidade de Opções
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
            
            // 3.5 Validar Referência de Grupos
            if (!string.IsNullOrEmpty(optionAttr.Group))
            {
                if (!groupNames.Contains(optionAttr.Group))
                {
                    throw new CleanParserException($"Configuration Error: Property '{prop.Name}' references undefined group '{optionAttr.Group}'.");
                }
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
