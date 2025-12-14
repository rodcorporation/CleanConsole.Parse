using Xunit;
using CleanConsole.Parse;
using CleanConsole.Parse.Attributes;

namespace CleanConsole.Parse.Tests;

public class HelpTests
{
    [ProgramDef(Name = "MyHelpApp", Description = "Does helpful things")]
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
}
