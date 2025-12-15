using System;
using System.Linq;
using Xunit;
using CleanConsole.Parse;

namespace CleanConsole.Parse.Tests;

public class ParseResultTests
{
    [ProgramDefinition(Name = "Multi", Description = "Multiple error capture")]
    [OptionGroup("Modes", OptionGroupRequirement.AtLeastOne)]
    private class MultiErrorConfig
    {
        [Option("age")]
        public int Age { get; set; }

        [Option("price")]
        public double Price { get; set; }

        [Option("mode", Group = "Modes")]
        public string? Mode { get; set; }
    }

    [Fact]
    public void Should_Aggregate_Multiple_Errors()
    {
        var result = CleanParser.Parse<MultiErrorConfig>(new[] { "-age=abc", "-price=xyz" });

        Assert.True(result.HasErrors);
        Assert.Equal(3, result.Errors.Count);
        Assert.Contains(result.Errors, error => error.Kind == ParseErrorKind.Conversion && error.OptionName == "age" && error.Message.Contains("Int32"));
        Assert.Contains(result.Errors, error => error.Kind == ParseErrorKind.Conversion && error.OptionName == "price" && error.Message.Contains("Double"));
        Assert.Contains(result.Errors, error => error.Kind == ParseErrorKind.GroupRule && error.OptionName == "Modes" && error.Message.Contains("Pelo menos uma opção do grupo 'Modes'"));
    }

    [ProgramDefinition(Name = "SummaryApp", Description = "Summary verification")]
    private class SummaryConfig
    {
        [Option("flag", ShortOptionName = "f")]
        public bool Flag { get; set; }
    }

    [Fact]
    public void GetSelectedSummary_Should_Return_NoOptions_When_None_Selected()
    {
        var result = CleanParser.Parse<SummaryConfig>(Array.Empty<string>());

        Assert.True(result.IsSuccess);
        Assert.Equal("No options were selected.", result.GetSelectedSummary());
        Assert.Equal(string.Empty, result.GetHelpDescription());
    }

    [ProgramDefinition(Name = "FormatterApp", Description = "Formats summaries")]
    [OptionGroup("Modes", OptionGroupRequirement.AtLeastOne)]
    private class SummaryFormatterConfig
    {
        [Option("source", ShortOptionName = "s")]
        public string? Source { get; set; }

        [Option("mode", ShortOptionName = "m", Group = "Modes")]
        public string? Mode { get; set; }

        [Option("verbose", Group = "Modes")]
        public bool Verbose { get; set; }
    }

    [Fact]
    public void GetSelectedSummary_Should_Format_Selected_Options_With_Groups()
    {
        var args = new[] { "-source:\"Hello World\"", "-mode:copy", "-verbose" };
        var result = CleanParser.Parse<SummaryFormatterConfig>(args);

        Assert.True(result.IsSuccess);

        var summary = result.GetSelectedSummary();

        Assert.Contains("Selected Options:", summary);
        Assert.Contains("--source (alias -s) => \"Hello World\"", summary);
        Assert.Contains("Group: Modes", summary);
        Assert.Contains("--mode (alias -m, group Modes) => copy", summary);
        Assert.Contains("--verbose (group Modes) => true", summary);
    }

    [ProgramDefinition(Name = "RichApp", Description = "Does rich things")]
    [OptionGroup("Output", OptionGroupRequirement.AtLeastOne, Description = "Output options")]
    private class HelpRichConfig
    {
        [Option("input", ShortOptionName = "i", Description = "Input file path")]
        public string? Input { get; set; }

        [Option("format", Group = "Output", Description = "Output format")]
        public string? Format { get; set; }

        [Option("compress", Group = "Output", Description = "Enable compression")]
        public bool Compress { get; set; }
    }

    [Fact]
    public void Help_Should_Render_Description_With_Groups_And_Usage()
    {
        var result = CleanParser.Parse<HelpRichConfig>(new[] { "--help" });

        Assert.True(result.HelpRequested);
        Assert.False(result.HasErrors);
        Assert.NotNull(result.Help);

        var help = result.GetHelpDescription();

        Assert.Contains("RichApp - Does rich things", help);
        Assert.Contains("Usage:", help);
        Assert.Contains("--input <value>", help);
        Assert.Contains("Group: Output (Requirement: AtLeastOne) - Output options", help);
        Assert.Contains("--format <value>", help);
        Assert.Contains("--compress", help);
    }
}
