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
        var result = CleanParser.Parse<GroupConfig>(new[] { "-x:1" });

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Kind == ParseErrorKind.GroupRule && e.Message.Contains("exige exatamente uma opção"));
    }

    [Fact]
    public void Should_Succeed_When_ExactOne_Is_One()
    {
        var result = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1" });

        Assert.True(result.IsSuccess);
        Assert.Equal("1", Assert.IsType<GroupConfig>(result.Options).A);
    }

    [Fact]
    public void Should_Throw_When_ExactOne_Is_Two()
    {
        var result = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-b:2", "-x:1" });

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Kind == ParseErrorKind.GroupRule && e.Message.Contains("exige exatamente uma opção"));
        Assert.Contains(result.Errors, e => e.Message.Contains("foram fornecidas: 2"));
    }

    // --- AtLeastOne Tests ---

    [Fact]
    public void Should_Throw_When_AtLeastOne_Is_Zero()
    {
        // Provide ExactOne (A) but nothing for AtLeastOne
        var result = CleanParser.Parse<GroupConfig>(new[] { "-a:1" });

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Kind == ParseErrorKind.GroupRule && e.Message.Contains("Requisito não atendido"));
    }

    [Fact]
    public void Should_Succeed_When_AtLeastOne_Is_One()
    {
        var result = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1" });

        Assert.True(result.IsSuccess);
        Assert.Equal("1", Assert.IsType<GroupConfig>(result.Options).X);
    }

    [Fact]
    public void Should_Succeed_When_AtLeastOne_Is_Many()
    {
        var result = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1", "-y:2" });

        Assert.True(result.IsSuccess);
        var options = Assert.IsType<GroupConfig>(result.Options);
        Assert.Equal("1", options.X);
        Assert.Equal("2", options.Y);
    }

    // --- None Tests ---

    [Fact]
    public void Should_Succeed_When_None_Is_Zero()
    {
        var result = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1" });

        Assert.True(result.IsSuccess);
        var options = Assert.IsType<GroupConfig>(result.Options);
        Assert.Null(options.N1);
        Assert.Null(options.N2);
    }

    [Fact]
    public void Should_Succeed_When_None_Is_One()
    {
        var result = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1", "-n1:test" });

        Assert.True(result.IsSuccess);
        var options = Assert.IsType<GroupConfig>(result.Options);
        Assert.Equal("test", options.N1);
        Assert.Null(options.N2);
    }

    [Fact]
    public void Should_Succeed_When_None_Is_Many()
    {
        var result = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1", "-n1:test1", "-n2:test2" });

        Assert.True(result.IsSuccess);
        var options = Assert.IsType<GroupConfig>(result.Options);
        Assert.Equal("test1", options.N1);
        Assert.Equal("test2", options.N2);
    }

    // --- AtMostOne Tests ---

    [Fact]
    public void Should_Succeed_When_AtMostOne_Is_Zero()
    {
        var result = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1" });

        Assert.True(result.IsSuccess);
        var options = Assert.IsType<GroupConfig>(result.Options);
        Assert.Null(options.M1);
        Assert.Null(options.M2);
    }

    [Fact]
    public void Should_Succeed_When_AtMostOne_Is_One()
    {
        var result = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1", "-m1:test" });

        Assert.True(result.IsSuccess);
        var options = Assert.IsType<GroupConfig>(result.Options);
        Assert.Equal("test", options.M1);
        Assert.Null(options.M2);
    }

    [Fact]
    public void Should_Throw_When_AtMostOne_Is_Many()
    {
        var result = CleanParser.Parse<GroupConfig>(new[] { "-a:1", "-x:1", "-m1:test1", "-m2:test2" });

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Kind == ParseErrorKind.GroupRule && e.Message.Contains("permite no máximo uma opção"));
        Assert.Contains(result.Errors, e => e.Message.Contains("foram fornecidas: 2"));
    }

    // --- All Tests ---

    [ProgramDefinition(Name = "Test", Description = "Test")]
    [OptionGroup("AllRequired", OptionGroupRequirement.All)]
    [OptionGroup("Modes", OptionGroupRequirement.AtLeastOne)]
    private class AllGroupConfig
    {
        [Option("source", Group = "AllRequired")]
        public string? Source { get; set; }

        [Option("retries", Group = "AllRequired")]
        public int Retries { get; set; }

        [Option("verbose", Group = "AllRequired")]
        public bool Verbose { get; set; }

        [Option("mode", Group = "Modes")]
        public string? Mode { get; set; }

        [Option("level", Group = "Modes")]
        public string? Level { get; set; }
    }

    [Fact]
    public void Should_Succeed_When_All_Group_Is_Complete()
    {
        var result = CleanParser.Parse<AllGroupConfig>(new[] { "-source:input", "-retries:3", "-verbose", "-mode:copy" });

        Assert.True(result.IsSuccess);
        var options = Assert.IsType<AllGroupConfig>(result.Options);
        Assert.Equal("input", options.Source);
        Assert.Equal(3, options.Retries);
        Assert.True(options.Verbose);
        Assert.Equal("copy", options.Mode);
    }

    [Fact]
    public void Should_Throw_When_All_Group_Missing_Single_Option()
    {
        var result = CleanParser.Parse<AllGroupConfig>(new[] { "-source:input", "-verbose", "-mode:copy" });

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Kind == ParseErrorKind.GroupRule && e.Message.Contains("Todas as opções do grupo"));
        Assert.Contains(result.Errors, e => e.Message.Contains("--retries"));
    }

    [Fact]
    public void Should_Throw_When_All_Group_Missing_Multiple_Options()
    {
        var result = CleanParser.Parse<AllGroupConfig>(new[] { "-retries:3", "-mode:copy" });

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Kind == ParseErrorKind.GroupRule && e.Message.Contains("Todas as opções do grupo"));
        Assert.Contains(result.Errors, e => e.Message.Contains("--source"));
        Assert.Contains(result.Errors, e => e.Message.Contains("--verbose"));
    }

    [Fact]
    public void Should_Still_Validate_Other_Groups_When_All_Is_Satisfied()
    {
        var result = CleanParser.Parse<AllGroupConfig>(new[] { "-source:input", "-retries:3", "-verbose" });

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Kind == ParseErrorKind.GroupRule && e.Message.Contains("Pelo menos uma opção do grupo 'Modes'"));
    }
}