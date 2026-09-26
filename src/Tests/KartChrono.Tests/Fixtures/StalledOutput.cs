using KartChrono.Abstractions.Output;
using Pure.Primitives.Abstractions.String;

namespace KartChrono.Tests.Fixtures;

public sealed record StalledOutput : IOutput
{
    private readonly IOutput _output;

    public StalledOutput(IOutput output)
    {
        _output = output;
    }

    public async IAsyncEnumerator<IString> GetAsyncEnumerator(
        CancellationToken cancellationToken = default
    )
    {
        await foreach (IString line in _output.WithCancellation(cancellationToken))
        {
            yield return line;
        }

        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }
}
