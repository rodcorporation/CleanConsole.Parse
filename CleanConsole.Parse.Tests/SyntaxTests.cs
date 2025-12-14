using Xunit;
using CleanConsole.Parse;
using CleanConsole.Parse.Attributes;
using CleanConsole.Parse.Exceptions;

namespace CleanConsole.Parse.Tests;

public class SyntaxTests
{
    [ProgramDef(Name = "Test", Description = "Test")]
    public class SimpleConfig
    {
        [Option("port")]
        public int Port { get; set; }

        [Option("msg")]
        public string Msg { get; set; }
    }

    [Fact]
    public void Should_Accept_Valid_Syntax_Colon()
    {
        // Should not throw
        CleanParser.Parse<SimpleConfig>(new[] { "-port:80" });
        CleanParser.Parse<SimpleConfig>(new[] { "/port:80" });
        CleanParser.Parse<SimpleConfig>(new[] { "--port:80" });
    }

    [Fact]
    public void Should_Accept_Valid_Syntax_Equals()
    {
        // Should not throw
        CleanParser.Parse<SimpleConfig>(new[] { "-port=80" });
        CleanParser.Parse<SimpleConfig>(new[] { "/port=80" });
        CleanParser.Parse<SimpleConfig>(new[] { "--port=80" });
    }

    [Fact]
    public void Should_Accept_Quoted_Values()
    {
        // Should not throw
        CleanParser.Parse<SimpleConfig>(new[] { "--msg=\"Hello World\"" });
        CleanParser.Parse<SimpleConfig>(new[] { "--msg:\"Hello World\"" });
    }

    [Fact]
    public void Should_Throw_On_Space_Separator()
    {
        var ex = Assert.Throws<CleanParserException>(() => 
            CleanParser.Parse<SimpleConfig>(new[] { "-port", "80" }));
        
        // The error happens on the second token "80" because it doesn't have a prefix
        Assert.Contains("Erro de sintaxe", ex.Message);
        Assert.Contains("80", ex.Message);
    }

    [Fact]
    public void Should_Throw_On_Loose_Token()
    {
        var ex = Assert.Throws<CleanParserException>(() => 
            CleanParser.Parse<SimpleConfig>(new[] { "filename.txt" }));
        
        Assert.Contains("Erro de sintaxe", ex.Message);
    }
}
