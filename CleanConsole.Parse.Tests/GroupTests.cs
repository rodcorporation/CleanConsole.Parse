using Xunit;
using CleanConsole.Parse;
using CleanConsole.Parse.Attributes;
using CleanConsole.Parse.Exceptions;
using CleanConsole.Parse.Enums;

namespace CleanConsole.Parse.Tests;

public class GroupTests
{
    [ProgramDef(Name = "Test", Description = "Test")]
    [OptionGroup(Name = "Exact", Type = OptionGroupType.ExactOne)]
    [OptionGroup(Name = "AtLeast", Type = OptionGroupType.AtLeastOne)]
    public class GroupConfig
    {
        [Option("a", Group = "Exact")]
        public string A { get; set; }

        [Option("b", Group = "Exact")]
        public string B { get; set; }

        [Option("x", Group = "AtLeast")]
        public string X { get; set; }

        [Option("y", Group = "AtLeast")]
        public string Y { get; set; }
    }

    // --- ExactOne Tests ---

    [Fact]
    public void Should_Throw_When_ExactOne_Is_Zero()
    {
        // Provide AtLeastOne (X) but nothing for ExactOne
        var ex = Assert.Throws<CleanParserException>(() => 
            CleanParser.Parse<GroupConfig>(new[] { "-x:1" }));
        Assert.Contains("exige exatamente uma opção", ex.Message);
    }

    [Fact]
    public void Should_Succeed_When_ExactOne_Is_One()
    {
        var res = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1" });
        Assert.Equal("1", res.A);
    }

    [Fact]
    public void Should_Throw_When_ExactOne_Is_Two()
    {
        var ex = Assert.Throws<CleanParserException>(() => 
            CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-b:2", "-x:1" }));
        Assert.Contains("exige exatamente uma opção", ex.Message);
        Assert.Contains("foram fornecidas: 2", ex.Message);
    }

    // --- AtLeastOne Tests ---

    [Fact]
    public void Should_Throw_When_AtLeastOne_Is_Zero()
    {
        // Provide ExactOne (A) but nothing for AtLeastOne
        var ex = Assert.Throws<CleanParserException>(() => 
            CleanParser.Parse<GroupConfig>(new[] { "-a:1" }));
        Assert.Contains("Requisito não atendido", ex.Message);
    }

    [Fact]
    public void Should_Succeed_When_AtLeastOne_Is_One()
    {
        var res = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1" });
        Assert.Equal("1", res.X);
    }

    [Fact]
    public void Should_Succeed_When_AtLeastOne_Is_Many()
    {
        var res = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1", "-y:2" });
        Assert.Equal("1", res.X);
        Assert.Equal("2", res.Y);
    }
}
