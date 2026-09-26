using KartChrono.Cli.Arguments;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace KartChrono.Cli.Tests.Arguments;

public sealed record ArgumentTests
{
    [Theory]
    [InlineData(new string[0], "")]
    [InlineData(new[] { "tracks" }, "tracks")]
    [InlineData(new[] { "live", "--track", "mayak" }, "live")]
    [InlineData(new[] { "--version" }, "")]
    [InlineData(new[] { "--track", "mayak" }, "")]
    public void TakesLeadingCommandName(string[] arguments, string expected)
    {
        IString command = new CommandName(arguments);

        Assert.Equal(expected, command.TextValue);
    }

    [Theory]
    [InlineData(new[] { "live", "--track", "mayak" }, "track", "mayak")]
    [InlineData(new[] { "live", "--track", "mayak" }, "kart", "")]
    [InlineData(new[] { "live", "--track" }, "track", "")]
    [InlineData(new[] { "live", "--track", "--json" }, "track", "")]
    [InlineData(new[] { "live", "--json", "--kart", "25" }, "kart", "25")]
    public void TakesValueFollowingOption(
        string[] arguments,
        string name,
        string expected
    )
    {
        IString value = new OptionValue(new String(name), arguments);

        Assert.Equal(expected, value.TextValue);
    }

    [Theory]
    [InlineData(new[] { "live", "--json" }, "json", true)]
    [InlineData(new[] { "live", "--json" }, "no-color", false)]
    [InlineData(new string[0], "json", false)]
    [InlineData(new[] { "--json-lines" }, "json", false)]
    public void DetectsOptionPresence(string[] arguments, string name, bool expected)
    {
        IBool present = new OptionPresence(new String(name), arguments);

        Assert.Equal(expected, present.BoolValue);
    }

    [Theory]
    [InlineData(new[] { "live", "--track", "mayak" }, 0)]
    [InlineData(new[] { "live", "--timeout", "3" }, 3)]
    [InlineData(new[] { "--timeout", "1", "live" }, 1)]
    [InlineData(new[] { "live", "--timeout", "2147483" }, 2147483)]
    public void TakesTimeoutInSeconds(string[] arguments, int expected)
    {
        INumber<int> timeout = new TimeoutSeconds(arguments);

        Assert.Equal(expected, timeout.NumberValue);
    }

    [Theory]
    [InlineData("live --timeout")]
    [InlineData("live --timeout --json")]
    [InlineData("live --timeout 0")]
    [InlineData("live --timeout -5")]
    [InlineData("live --timeout 1.5")]
    [InlineData("live --timeout soon")]
    [InlineData("live --timeout 2147484")]
    public void RejectsTimeoutThatIsNotPositiveWholeSeconds(string arguments)
    {
        INumber<int> timeout = new TimeoutSeconds(arguments.Split(' '));

        ArgumentException error = Assert.Throws<ArgumentException>(() =>
            timeout.NumberValue
        );

        Assert.Contains("--timeout", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void EnumeratesParsedValuesAsCharacters()
    {
        IString value = new OptionValue(new String("track"), "live", "--track", "mayak");

        Assert.Equal(['m', 'a', 'y', 'a', 'k'], value.Select(symbol => symbol.CharValue));
        Assert.Equal(
            ['l', 'i', 'v', 'e'],
            new CommandName("live").Select(symbol => symbol.CharValue)
        );
    }
}
