using KartChrono.Abstractions.Output;
using KartChrono.Cli.Commands;
using Pure.Primitives.Abstractions.String;

namespace KartChrono.Cli.Tests.Commands;

public sealed record CommandTests
{
    [Fact]
    public async Task ShowsVersionWhenRequested()
    {
        Assert.Equal(["0.1.0"], await Rendered("--version"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("--help")]
    [InlineData("nonsense")]
    [InlineData("live --help")]
    public async Task ShowsUsageWhenNoCommandApplies(string arguments)
    {
        IReadOnlyList<string> lines = await Rendered(
            arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        );

        Assert.Contains(
            lines,
            line => line.Contains("kartchrono tracks", StringComparison.Ordinal)
        );
    }

    [Fact]
    public async Task PrefersVersionOverHelp()
    {
        Assert.Equal(["0.1.0"], await Rendered("--help", "--version"));
    }

    [Fact]
    public async Task RendersWithinTimeout()
    {
        Assert.Equal(["0.1.0"], await Rendered("--version", "--timeout", "5"));
    }

    [Fact]
    public async Task RejectsMalformedTimeout()
    {
        _ = await Assert.ThrowsAsync<ArgumentException>(() =>
            Rendered("--version", "--timeout", "soon")
        );
    }

    private static async Task<IReadOnlyList<string>> Rendered(
        params IEnumerable<string> arguments
    )
    {
        using HttpClient client = new HttpClient();

        List<string> lines = [];

        IOutput command = new Command(client, arguments);

        await foreach (IString line in command)
        {
            lines.Add(line.TextValue);
        }

        return lines;
    }
}
