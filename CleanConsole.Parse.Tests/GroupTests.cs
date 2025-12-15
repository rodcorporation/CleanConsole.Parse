using Xunit;
using CleanConsole.Parse;

namespace CleanConsole.Parse.Tests;

public class GroupTests
{
    [ProgramDefinition(Name = "Test", Description = "Test")]
    [OptionGroup("Exact", OptionGroupRequirement.ExactOne)]
    [OptionGroup("AtLeast", OptionGroupRequirement.AtLeastOne)]
    [OptionGroup("None", OptionGroupRequirement.None)]
    [OptionGroup("AtMost", OptionGroupRequirement.AtMostOne)]
    public class GroupConfig
    {
        [Option("a", Group = "Exact")]
        public string? A { get; set; }

        [Option("b", Group = "Exact")]
        public string? B { get; set; }

        [Option("x", Group = "AtLeast")]
        public string? X { get; set; }

        [Option("y", Group = "AtLeast")]
        public string? Y { get; set; }

        [Option("n1", Group = "None")]
        public string? N1 { get; set; }

        [Option("n2", Group = "None")]
        public string? N2 { get; set; }

        [Option("m1", Group = "AtMost")]
        public string? M1 { get; set; }

        [Option("m2", Group = "AtMost")]
        public string? M2 { get; set; }
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

    // --- None Tests ---

    [Fact]
    public void Should_Succeed_When_None_Is_Zero()
    {
        var res = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1" });
        Assert.Null(res.N1);
        Assert.Null(res.N2);
    }

    [Fact]
    public void Should_Succeed_When_None_Is_One()
    {
        var res = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1", "-n1:test" });
        Assert.Equal("test", res.N1);
        Assert.Null(res.N2);
    }

    [Fact]
    public void Should_Succeed_When_None_Is_Many()
    {
        var res = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1", "-n1:test1", "-n2:test2" });
        Assert.Equal("test1", res.N1);
        Assert.Equal("test2", res.N2);
    }

    // --- AtMostOne Tests ---

    [Fact]
    public void Should_Succeed_When_AtMostOne_Is_Zero()
    {
        var res = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1" });
        Assert.Null(res.M1);
        Assert.Null(res.M2);
    }

    [Fact]
    public void Should_Succeed_When_AtMostOne_Is_One()
    {
        var res = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1", "-m1:test" });
        Assert.Equal("test", res.M1);
        Assert.Null(res.M2);
    }

    [Fact]
    public void Should_Throw_When_AtMostOne_Is_Many()
    {
        var ex = Assert.Throws<CleanParserException>(() => 
            CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1", "-m1:test1", "-m2:test2" }));
        Assert.Contains("permite no máximo uma opção", ex.Message);
        Assert.Contains("foram fornecidas: 2", ex.Message);
    }
}