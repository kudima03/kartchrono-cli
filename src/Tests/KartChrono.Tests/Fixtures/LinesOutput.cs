using KartChrono.Abstractions.Output;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace KartChrono.Tests.Fixtures;

public sealed record LinesOutput : IOutput
{
    private readonly IEnumerable<string> _lines;

    public LinesOutput(params IEnumerable<string> lines)
    {
        _lines = lines;
    }

    public async IAsyncEnumerator<IString> GetAsyncEnumerator(
        CancellationToken cancellationToken = default
    )
    {
        foreach (string line in _lines)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            yield return new String(line);
        }
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
