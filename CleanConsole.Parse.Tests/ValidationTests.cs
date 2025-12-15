using System;
using Xunit;
using CleanConsole.Parse;

namespace CleanConsole.Parse.Tests;

public class ValidationTests
{
    [Fact]
    public void Should_Throw_When_Unsupported_Type_Is_Used()
    {
        var result = CleanParser.Parse<UnsupportedTypeConfig>(Array.Empty<string>());

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Kind == ParseErrorKind.Configuration && e.Message.Contains("unsupported type"));
    }

    [Fact]
    public void Should_Throw_When_Duplicate_OptionName()
    {
        var result = CleanParser.Parse<DuplicateNameConfig>(Array.Empty<string>());

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Kind == ParseErrorKind.Configuration && e.Message.Contains("Duplicate OptionName"));
    }

    [Fact]
    public void Should_Throw_When_Group_Reference_Is_Invalid()
    {
        var result = CleanParser.Parse<InvalidGroupConfig>(Array.Empty<string>());

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Kind == ParseErrorKind.Configuration && e.Message.Contains("references undefined group"));
    }

    [Fact]
    public void Should_Throw_When_Name_Has_Prefix()
    {
        var result = CleanParser.Parse<PrefixNameConfig>(Array.Empty<string>());

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Kind == ParseErrorKind.Configuration && e.Message.Contains("must not start with prefixes"));
    }

    [Fact]
    public void Should_Throw_When_All_Group_Has_No_Options()
    {
        var result = CleanParser.Parse<AllGroupWithoutOptions>(Array.Empty<string>());

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Kind == ParseErrorKind.Configuration && e.Message.Contains("is marked as 'All'"));
    }

    [ProgramDefinition(Name = "Test", Description = "Test")]
    private class UnsupportedTypeConfig
    {
        [Option("date")]
        public DateTime Date { get; set; }
    }

    [ProgramDefinition(Name = "Test", Description = "Test")]
    private class DuplicateNameConfig
    {
        [Option("file")]
        public string? File1 { get; set; }

        [Option("file")]
        public string? File2 { get; set; }
    }

    [ProgramDefinition(Name = "Test", Description = "Test")]
    private class InvalidGroupConfig
    {
        [Option("file", Group = "NonExistent")]
        public string? File { get; set; }
    }

    [ProgramDefinition(Name = "Test", Description = "Test")]
    private class PrefixNameConfig
    {
        [Option("-file")]
        public string? File { get; set; }
    }

    [ProgramDefinition(Name = "Test", Description = "Test")]
    [OptionGroup("All", OptionGroupRequirement.All)]
    private class AllGroupWithoutOptions
    {
        [Option("other")]
        public string? Other { get; set; }
    }
}
