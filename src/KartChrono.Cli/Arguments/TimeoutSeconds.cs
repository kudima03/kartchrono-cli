using System.Globalization;
using Pure.Primitives.Abstractions.Number;
using String = Pure.Primitives.String.String;

namespace KartChrono.Cli.Arguments;

public sealed record TimeoutSeconds : INumber<int>
{
    private const int Longest = int.MaxValue / 1000;

    private readonly IEnumerable<string> _arguments;

    public TimeoutSeconds(params IEnumerable<string> arguments)
    {
        _arguments = arguments;
    }

    public int NumberValue =>
        !new OptionPresence(new String("timeout"), _arguments).BoolValue ? 0
        : int.TryParse(
            new OptionValue(new String("timeout"), _arguments).TextValue,
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out int seconds
        ) && seconds is > 0 and <= Longest
            ? seconds
        : throw new ArgumentException(
            string.Format(
                CultureInfo.InvariantCulture,
                "--timeout expects a whole number of seconds from 1 to {0}.",
                Longest
            )
        );

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }
}
