using System.Text.Json;
using KartChrono.Abstractions.Output;
using KartChrono.Protocol;
using KartChrono.Rendering.Json;
using KartChrono.Tests.Fixtures;
using KartChrono.Timing;
using Pure.Primitives.Abstractions.String;

namespace KartChrono.Tests.Rendering.Json;

public sealed record JsonOutputTests
{
    [Fact]
    public async Task WritesOneTrackPerLine()
    {
        IReadOnlyList<JsonElement> lines = await Parsed(
            new TracksJson(new Tracks(new Fixture("gettrackslist.html")))
        );

        Assert.Equal(54, lines.Count);
        Assert.Equal("mayak", lines[0].GetProperty("slug").GetString());
        Assert.Equal("Маяк", lines[0].GetProperty("name").GetString());
    }

    [Fact]
    public async Task WritesSessionAsSingleObjectPerSnapshot()
    {
        IReadOnlyList<JsonElement> lines = await Parsed(
            new SessionJson(new LiveFeed(new Frames("snapshot.json")))
        );

        JsonElement snapshot = Assert.Single(lines);

        Assert.Equal("Заезд №3", snapshot.GetProperty("run").GetString());
        Assert.Equal(334_000, snapshot.GetProperty("elapsedMilliseconds").GetInt64());
        Assert.Equal(3, snapshot.GetProperty("competitors").GetArrayLength());
        Assert.Equal(
            76203,
            snapshot
                .GetProperty("competitors")[0]
                .GetProperty("bestLapMilliseconds")
                .GetInt32()
        );
        Assert.Equal(
            5,
            snapshot
                .GetProperty("competitors")[0]
                .GetProperty("lastSectors")
                .GetArrayLength()
        );
    }

    [Fact]
    public async Task WritesOneLapPerLine()
    {
        IReadOnlyList<JsonElement> lines = await Parsed(
            new LapsJson(
                new LiveFeed(new Frames("snapshot.json", "binlaps.bin", "delta.json"))
            )
        );

        Assert.Equal(11, lines.Count);
        Assert.Equal(4, lines[0].GetProperty("sectors").GetArrayLength());
        Assert.Contains(lines, line => line.GetProperty("kart").GetString() == "25");
    }

    [Fact]
    public async Task WritesOneRecordPerLine()
    {
        IReadOnlyList<JsonElement> lines = await Parsed(
            new RecordsJson(new Records(new Fixture("records.html")))
        );

        Assert.Equal(4, lines.Count);
        Assert.Equal("Клуб", lines[0].GetProperty("kartModel").GetString());
        Assert.Equal(77521, lines[0].GetProperty("milliseconds").GetInt32());
    }

    [Fact]
    public void EscapesWithoutMangingNonAsciiText()
    {
        IString line = new JsonLine(writer => writer.WriteString("name", "Заезд \"№3\""));

        Assert.Equal(
            string.Concat("{", "\"name\":\"Заезд \\\"№3\\\"\"", "}"),
            line.TextValue
        );
        Assert.Equal('{', line.First().CharValue);
    }

    private static async Task<IReadOnlyList<JsonElement>> Parsed(IOutput output)
    {
        List<JsonElement> lines = [];

        await foreach (IString line in output)
        {
            lines.Add(JsonElement.Parse(line.TextValue));
        }

        return lines;
    }
}
