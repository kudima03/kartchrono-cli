using KartChrono.Abstractions.Output;
using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;

namespace KartChrono.Rendering;

public sealed record TimeLimitedOutput : IOutput
{
    private readonly IOutput _output;

    private readonly INumber<int> _seconds;

    public TimeLimitedOutput(IOutput output, INumber<int> seconds)
    {
        _output = output;
        _seconds = seconds;
    }

    public async IAsyncEnumerator<IString> GetAsyncEnumerator(
        CancellationToken cancellationToken = default
    )
    {
        int seconds = _seconds.NumberValue;

        using CancellationTokenSource limit =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        if (seconds > 0)
        {
            limit.CancelAfter(TimeSpan.FromSeconds(seconds));
        }

        await foreach (IString line in _output.WithCancellation(limit.Token))
        {
            yield return line;
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
