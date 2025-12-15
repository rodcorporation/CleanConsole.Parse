using Xunit;
using CleanConsole.Parse;

namespace CleanConsole.Parse.Tests;

public class SyntaxTests
{
    [ProgramDefinition(Name = "Test", Description = "Test")]
    public class SimpleConfig
    {
        [Option("port")]
        public int Port { get; set; }

        [Option("msg")]
        public string? Msg { get; set; }
    }

    [Fact]
    public void Should_Accept_Valid_Syntax_Colon()
    {
        var resultHyphen = CleanParser.Parse<SimpleConfig>(new[] { "-port:80" });
        Assert.True(resultHyphen.IsSuccess);
        Assert.Equal(80, Assert.IsType<SimpleConfig>(resultHyphen.Options).Port);

        var resultSlash = CleanParser.Parse<SimpleConfig>(new[] { "/port:80" });
        Assert.True(resultSlash.IsSuccess);
        Assert.Equal(80, Assert.IsType<SimpleConfig>(resultSlash.Options).Port);

        var resultDoubleHyphen = CleanParser.Parse<SimpleConfig>(new[] { "--port:80" });
        Assert.True(resultDoubleHyphen.IsSuccess);
        Assert.Equal(80, Assert.IsType<SimpleConfig>(resultDoubleHyphen.Options).Port);
    }

    [Fact]
    public void Should_Accept_Valid_Syntax_Equals()
    {
        var resultHyphen = CleanParser.Parse<SimpleConfig>(new[] { "-port=80" });
        Assert.True(resultHyphen.IsSuccess);
        Assert.Equal(80, Assert.IsType<SimpleConfig>(resultHyphen.Options).Port);

        var resultSlash = CleanParser.Parse<SimpleConfig>(new[] { "/port=80" });
        Assert.True(resultSlash.IsSuccess);
        Assert.Equal(80, Assert.IsType<SimpleConfig>(resultSlash.Options).Port);

        var resultDoubleHyphen = CleanParser.Parse<SimpleConfig>(new[] { "--port=80" });
        Assert.True(resultDoubleHyphen.IsSuccess);
        Assert.Equal(80, Assert.IsType<SimpleConfig>(resultDoubleHyphen.Options).Port);
    }

    [Fact]
    public void Should_Accept_Quoted_Values()
    {
        var resultEquals = CleanParser.Parse<SimpleConfig>(new[] { "--msg=\"Hello World\"" });
        Assert.True(resultEquals.IsSuccess);
        Assert.Equal("Hello World", Assert.IsType<SimpleConfig>(resultEquals.Options).Msg);

        var resultColon = CleanParser.Parse<SimpleConfig>(new[] { "--msg:\"Hello World\"" });
        Assert.True(resultColon.IsSuccess);
        Assert.Equal("Hello World", Assert.IsType<SimpleConfig>(resultColon.Options).Msg);
    }

    [Fact]
    public void Should_Throw_On_Space_Separator()
    {
        var result = CleanParser.Parse<SimpleConfig>(new[] { "-port", "80" });

        Assert.True(result.HasErrors);
        var error = Assert.Single(result.Errors);
        Assert.Equal(ParseErrorKind.Syntax, error.Kind);
        Assert.Contains("Erro de sintaxe", error.Message);
        Assert.Contains("80", error.Message);
    }

    [Fact]
    public void Should_Throw_On_Loose_Token()
    {
        var result = CleanParser.Parse<SimpleConfig>(new[] { "filename.txt" });

        Assert.True(result.HasErrors);
        var error = Assert.Single(result.Errors);
        Assert.Equal(ParseErrorKind.Syntax, error.Kind);
        Assert.Contains("Erro de sintaxe", error.Message);
    }

    [Fact]
    public void Should_Handle_Mixed_Prefixes_Last_Wins()
    {
        // -port:10 then /port:20 -> Result 20
        var result = CleanParser.Parse<SimpleConfig>(new[] { "-port:10", "/port:20" });

        Assert.True(result.IsSuccess);
        var options = Assert.IsType<SimpleConfig>(result.Options);
        Assert.Equal(20, options.Port);
    }
}
