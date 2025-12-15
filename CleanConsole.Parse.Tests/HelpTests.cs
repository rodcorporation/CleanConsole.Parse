using Xunit;
using CleanConsole.Parse;

namespace CleanConsole.Parse.Tests;

public class HelpTests
{
    [ProgramDefinition(Name = "MyHelpApp", Description = "Does helpful things")]
    public class HelpConfig
    {
        [Option("file", ShortOptionName = "f")]
        public string? File { get; set; }

        [Option("verbose")]
        public bool Verbose { get; set; }
    }

    [Fact]
    public void Should_Generate_Help_Text()
    {
        var help = CleanParser.GetHelpText<HelpConfig>();

        Assert.Contains("MyHelpApp", help);
        Assert.Contains("Does helpful things", help);
        Assert.Contains("--file", help);
        Assert.Contains("-f", help);
        Assert.Contains("--verbose", help);
    }

    [ProgramDefinition(Name = "DetailedApp", Description = "App description")]
    [OptionGroup("Output", OptionGroupRequirement.None, Description = "Output settings")]
    public class DetailedHelpConfig
    {
        [Option("input", ShortOptionName = "i", Description = "Input file path")]
        public string? Input { get; set; }

        [Option("format", Group = "Output", Description = "Output format")]
        public string? Format { get; set; }
    }

    [Fact]
    public void Should_Include_Descriptions_And_Groups_In_Help()
    {
        var help = CleanParser.GetHelpText<DetailedHelpConfig>();

        Assert.Contains("DetailedApp", help);
        Assert.Contains("App description", help);
        
        // Check Option Description
        Assert.Contains("Input file path", help);
        
        // Check Group presence and description
        Assert.Contains("Group: Output", help);
        Assert.Contains("(Requirement: None)", help);
        Assert.Contains("Output settings", help);
        
        // Check grouped option
        Assert.Contains("Output format", help);
    }
}
