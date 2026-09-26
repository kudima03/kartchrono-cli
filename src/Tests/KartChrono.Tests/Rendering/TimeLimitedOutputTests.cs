using System.Diagnostics;
using KartChrono.Abstractions.Output;
using KartChrono.Rendering;
using KartChrono.Tests.Fixtures;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Number;

namespace KartChrono.Tests.Rendering;

public sealed record TimeLimitedOutputTests
{
    [Fact]
    public async Task StopsStalledOutputOnceTimeoutElapses()
    {
        List<string> lines = [];
        Stopwatch clock = Stopwatch.StartNew();

        _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (
                IString line in new TimeLimitedOutput(
                    new StalledOutput(new LinesOutput("first")),
                    new Int(1)
                )
            )
            {
                lines.Add(line.TextValue);
            }
        });

        Assert.Equal(["first"], lines);
        Assert.InRange(
            clock.Elapsed,
            TimeSpan.FromSeconds(0.9),
            TimeSpan.FromSeconds(10)
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(60)]
    public async Task PassesFiniteOutputThrough(int seconds)
    {
        List<string> lines = [];

        await foreach (
            IString line in new TimeLimitedOutput(
                new LinesOutput("first", "second"),
                new Int(seconds)
            )
        )
        {
            lines.Add(line.TextValue);
        }

        Assert.Equal(["first", "second"], lines);
    }

    [Fact]
    public async Task HonoursCallerCancellationWithoutTimeout()
    {
        using CancellationTokenSource caller = new CancellationTokenSource(
            TimeSpan.FromMilliseconds(100)
        );

        IOutput output = new TimeLimitedOutput(
            new StalledOutput(new LinesOutput("first")),
            new Int(0)
        );

        _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (IString line in output.WithCancellation(caller.Token))
            {
                Assert.Equal("first", line.TextValue);
            }
        });
    }
}
