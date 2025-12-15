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
        var res = CleanParser.Parse<TypeConfig>(new[] { "-str:Hello" });
        Assert.Equal("Hello", res.Str);
    }

    [Fact]
    public void Should_Bind_Int()
    {
        var res = CleanParser.Parse<TypeConfig>(new[] { "-int=42" });
        Assert.Equal(42, res.IntVal);
    }

    [Fact]
    public void Should_Throw_Invalid_Int()
    {
        var ex = Assert.Throws<CleanParserException>(() => 
            CleanParser.Parse<TypeConfig>(new[] { "-int=abc" }));
        Assert.Contains("não é válido", ex.Message);
        Assert.Contains("Int32", ex.Message);
    }

    [Fact]
    public void Should_Bind_Double_Invariant()
    {
        var res = CleanParser.Parse<TypeConfig>(new[] { "-dbl=10.5" });
        Assert.Equal(10.5, res.DblVal);
    }

    [Fact]
    public void Should_Throw_Invalid_Double()
    {
        var ex = Assert.Throws<CleanParserException>(() => 
            CleanParser.Parse<TypeConfig>(new[] { "-dbl=abc" }));
        Assert.Contains("Double", ex.Message);
    }

    [Fact]
    public void Should_Bind_Bool_Flag()
    {
        var res = CleanParser.Parse<TypeConfig>(new[] { "-bool" });
        Assert.True(res.BoolVal);
    }

    [Fact]
    public void Should_Bind_Bool_Explicit()
    {
        var resTrue = CleanParser.Parse<TypeConfig>(new[] { "-bool:true" });
        Assert.True(resTrue.BoolVal);

        var resFalse = CleanParser.Parse<TypeConfig>(new[] { "-bool:false" });
        Assert.False(resFalse.BoolVal);
    }

    [Fact]
    public void Should_Throw_Missing_Value()
    {
        // -str sem valor
        var ex = Assert.Throws<CleanParserException>(() => 
            CleanParser.Parse<TypeConfig>(new[] { "-str" }));
        Assert.Contains("exige um valor", ex.Message);
    }

    [Fact]
    public void Should_Apply_Last_Wins()
    {
        var res = CleanParser.Parse<TypeConfig>(new[] { "-int:10", "-int:20" });
        Assert.Equal(20, res.IntVal);
    }
}
