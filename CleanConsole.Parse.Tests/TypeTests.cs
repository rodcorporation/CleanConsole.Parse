using Xunit;
using CleanConsole.Parse;

namespace CleanConsole.Parse.Tests;

public class TypeTests
{
    [ProgramDefinition(Name = "Test", Description = "Test")]
    public class TypeConfig
    {
        [Option("str")]
        public string? Str { get; set; }

        [Option("int")]
        public int IntVal { get; set; }

        [Option("dbl")]
        public double DblVal { get; set; }

        [Option("bool")]
        public bool BoolVal { get; set; }
    }

    [Fact]
    public void Should_Bind_String()
    {
        var result = CleanParser.Parse<TypeConfig>(new[] { "-str:Hello" });

        Assert.True(result.IsSuccess);
        var options = Assert.IsType<TypeConfig>(result.Options);
        Assert.Equal("Hello", options.Str);
    }

    [Fact]
    public void Should_Bind_Int()
    {
        var result = CleanParser.Parse<TypeConfig>(new[] { "-int=42" });

        Assert.True(result.IsSuccess);
        var options = Assert.IsType<TypeConfig>(result.Options);
        Assert.Equal(42, options.IntVal);
    }

    [Fact]
    public void Should_Throw_Invalid_Int()
    {
        var result = CleanParser.Parse<TypeConfig>(new[] { "-int=abc" });

        Assert.True(result.HasErrors);
        var error = Assert.Single(result.Errors);
        Assert.Equal(ParseErrorKind.Conversion, error.Kind);
        Assert.Contains("não é válido", error.Message);
        Assert.Contains("Int32", error.Message);
    }

    [Fact]
    public void Should_Bind_Double_Invariant()
    {
        var result = CleanParser.Parse<TypeConfig>(new[] { "-dbl=10.5" });

        Assert.True(result.IsSuccess);
        var options = Assert.IsType<TypeConfig>(result.Options);
        Assert.Equal(10.5, options.DblVal);
    }

    [Fact]
    public void Should_Throw_Invalid_Double()
    {
        var result = CleanParser.Parse<TypeConfig>(new[] { "-dbl=abc" });

        Assert.True(result.HasErrors);
        var error = Assert.Single(result.Errors);
        Assert.Equal(ParseErrorKind.Conversion, error.Kind);
        Assert.Contains("Double", error.Message);
    }

    [Fact]
    public void Should_Bind_Bool_Flag()
    {
        var result = CleanParser.Parse<TypeConfig>(new[] { "-bool" });

        Assert.True(result.IsSuccess);
        var options = Assert.IsType<TypeConfig>(result.Options);
        Assert.True(options.BoolVal);
    }

    [Fact]
    public void Should_Bind_Bool_Explicit()
    {
        var resultTrue = CleanParser.Parse<TypeConfig>(new[] { "-bool:true" });
        var optionsTrue = Assert.IsType<TypeConfig>(resultTrue.Options);
        Assert.True(optionsTrue.BoolVal);

        var resultFalse = CleanParser.Parse<TypeConfig>(new[] { "-bool:false" });
        var optionsFalse = Assert.IsType<TypeConfig>(resultFalse.Options);
        Assert.False(optionsFalse.BoolVal);
    }

    [Fact]
    public void Should_Throw_Missing_Value()
    {
        // -str sem valor
        var result = CleanParser.Parse<TypeConfig>(new[] { "-str" });

        Assert.True(result.HasErrors);
        var error = Assert.Single(result.Errors);
        Assert.Equal(ParseErrorKind.Syntax, error.Kind);
        Assert.Contains("exige um valor", error.Message);
    }

    [Fact]
    public void Should_Apply_Last_Wins()
    {
        var result = CleanParser.Parse<TypeConfig>(new[] { "-int:10", "-int:20" });

        Assert.True(result.IsSuccess);
        var options = Assert.IsType<TypeConfig>(result.Options);
        Assert.Equal(20, options.IntVal);
    }

    [Fact]
    public void Should_Parse_Using_CommandLineArgs_When_Not_Passed_Explicitly()
    {
        var originalProvider = CleanParser.CommandLineArgsProvider;

        try
        {
            CleanParser.CommandLineArgsProvider = () => new[] { "/app/testhost.dll", "-int=42", "-bool" };

            var result = CleanParser.Parse<TypeConfig>();

            Assert.True(result.IsSuccess);
            var options = Assert.IsType<TypeConfig>(result.Options);
            Assert.Equal(42, options.IntVal);
            Assert.True(options.BoolVal);
        }
        finally
        {
            CleanParser.CommandLineArgsProvider = originalProvider;
        }
    }
}
