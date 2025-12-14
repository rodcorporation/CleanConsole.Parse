using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CleanConsole.Parse.Attributes;
using CleanConsole.Parse.Exceptions;

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

        // Parsing logic will be implemented in future tasks (Task 5+)
        return new T();
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
            
            // Check cross-collision (Long name colliding with Short name of another, usually allowed but let's check strictness if needed. 
            // PRD doesn't explicitly forbid collision between Long and Short of different options, but usually they are distinct namespaces.
            // However, ensuring they are unique across the board avoids ambiguity if user inputs "-name".
            // Implementation choice: Keep separate for now unless conflict arises.
            
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
            // Se não começa com prefixo, é considerado um valor solto ("orphan value") que viola a regra de espaços.
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
