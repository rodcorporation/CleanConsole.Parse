using Xunit;
using CleanConsole.Parse;

namespace CleanConsole.Parse.Tests;

public class ValidationTests
{
    [Fact]
    public void Should_Throw_When_Unsupported_Type_Is_Used()
    {
        var ex = Assert.Throws<CleanParserException>(() => CleanParser.Parse<UnsupportedTypeConfig>(new string[0]));
        Assert.Contains("unsupported type", ex.Message);
    }

    [Fact]
    public void Should_Throw_When_Duplicate_OptionName()
    {
        var ex = Assert.Throws<CleanParserException>(() => CleanParser.Parse<DuplicateNameConfig>(new string[0]));
        Assert.Contains("Duplicate OptionName", ex.Message);
    }

    [Fact]
    public void Should_Throw_When_Group_Reference_Is_Invalid()
    {
        var ex = Assert.Throws<CleanParserException>(() => CleanParser.Parse<InvalidGroupConfig>(new string[0]));
        Assert.Contains("references undefined group", ex.Message);
    }

    [Fact]
    public void Should_Throw_When_Name_Has_Prefix()
    {
         var ex = Assert.Throws<CleanParserException>(() => CleanParser.Parse<PrefixNameConfig>(new string[0]));
         Assert.Contains("must not start with prefixes", ex.Message);
    }

    [ProgramDef(Name = "Test", Description = "Test")]
    private class UnsupportedTypeConfig
    {
        [Option("date")]
        public DateTime Date { get; set; }
    }

    [ProgramDef(Name = "Test", Description = "Test")]
    private class DuplicateNameConfig
    {
        [Option("file")]
        public string? File1 { get; set; }

        [Option("file")]
        public string? File2 { get; set; }
    }

    [ProgramDef(Name = "Test", Description = "Test")]
    private class InvalidGroupConfig
    {
        [Option("file", Group = "NonExistent")]
        public string? File { get; set; }
    }

    [ProgramDef(Name = "Test", Description = "Test")]
    private class PrefixNameConfig
    {
        [Option("-file")]
        public string? File { get; set; }
    }
}