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

        // Parsing logic will be implemented in future tasks (Task 4+)
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
}
