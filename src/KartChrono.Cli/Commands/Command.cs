using KartChrono.Abstractions.Output;
using KartChrono.Abstractions.Timing;
using KartChrono.Cli.Arguments;
using KartChrono.Protocol;
using KartChrono.Rendering;
using KartChrono.Rendering.Json;
using KartChrono.Timing;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Number;
using String = Pure.Primitives.String.String;

namespace KartChrono.Cli.Commands;

public sealed record Command : IOutput
{
    private readonly IEnumerable<string> _arguments;

    private readonly HttpClient _client;

    public Command(HttpClient client, params IEnumerable<string> arguments)
    {
        _client = client;
        _arguments = arguments;
    }

    private IAsyncEnumerable<ITrack> Directory => new Tracks(new TrackDirectory(_client));

    private IString Slug => new OptionValue(new String("track"), _arguments);

    private IString Kart => new OptionValue(new String("kart"), _arguments);

    private IFeed Feed => new FeedOfKart(new TrackFeed(Directory, Slug), Kart);

    private bool AsJson => new OptionPresence(new String("json"), _arguments).BoolValue;

    private IString Period
    {
        get
        {
            IString requested = new OptionValue(new String("period"), _arguments);

            return requested.TextValue.Length == 0 ? new String("today") : requested;
        }
    }

    private IOutput Chosen =>
        new OptionPresence(new String("version"), _arguments).BoolValue
            ? new VersionOutput(new String(Version.Text))
        : new OptionPresence(new String("help"), _arguments).BoolValue ? new HelpOutput()
        : new CommandName(_arguments).TextValue switch
        {
            "tracks" => AsJson ? new TracksJson(Directory) : new TracksOutput(Directory),
            "session" => AsJson
                ? new SessionJson(new SettledFeed(Feed, new Int(1)))
                : new LeaderboardOutput(new SettledFeed(Feed, new Int(1))),
            "live" => AsJson ? new SessionJson(Feed) : new LeaderboardOutput(Feed),
            "laps" => AsJson
                ? new LapsJson(new SettledFeed(Feed, new Int(3)))
                : new LapsOutput(new SettledFeed(Feed, new Int(3))),
            "records" => AsJson
                ? new RecordsJson(new Records(new RecordsPage(_client, Slug, Period)))
                : new RecordsOutput(new Records(new RecordsPage(_client, Slug, Period))),
            _ => new HelpOutput(),
        };

    private IOutput Limited =>
        new TimeLimitedOutput(Chosen, new TimeoutSeconds(_arguments));

    public IAsyncEnumerator<IString> GetAsyncEnumerator(
        CancellationToken cancellationToken = default
    )
    {
        return Limited.GetAsyncEnumerator(cancellationToken);
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
