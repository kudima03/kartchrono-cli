# kartchrono-cli

Query and monitor [KartChrono](https://kartchrono.com) live karting timing from your terminal.

[![.NET build & test](https://github.com/kudima03/kartchrono-cli/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/kudima03/kartchrono-cli/actions/workflows/build-and-test.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Overview

KartChrono is a karting timing system used by ~54 tracks across Europe. Each track publishes a
live scoreboard on its own subdomain, fed by a WebSocket stream. `kartchrono` is a Native AOT
command-line client for that stream: it lists tracks, prints a session snapshot, follows a
leaderboard live, and shows lap-by-lap sector times for a single kart — without a browser.

Everything is passed as an argument, so the tool composes with `grep`, `jq`, `watch` and shell
pipelines.

## Installation

```shell
dotnet tool install -g KartChrono.Cli
```

`dotnet tool install` fetches a framework-dependent package that runs on any platform with
the .NET runtime installed. Standalone Native AOT binaries — no runtime required — are also
published for `linux-x64`, `linux-arm64`, `osx-arm64` and `win-x64`, attached to
each [release](https://github.com/kudima03/kartchrono-cli/releases).

## Usage

```shell
kartchrono tracks                                   # list every connected track
kartchrono session --track mayak                    # one snapshot of the current run, then exit
kartchrono live    --track mayak                    # follow the leaderboard
kartchrono live    --track mayak --kart 25          # follow one kart
kartchrono laps    --track mayak --kart 25          # lap-by-lap with sector times
kartchrono records --track mayak --period week      # best laps from the archive
```

| Option | Description |
|---|---|
| `--track <slug>` | Track subdomain, as listed by `kartchrono tracks` |
| `--kart <number>` | Restrict output to a single kart number |
| `--period <today\|week\|month>` | Range for `records` |
| `--json` | Emit JSON instead of a table |
| `--timeout <seconds>` | Stop after the given time |
| `--no-color` | Disable ANSI colour |
| `--help`, `--version` | |

## Design

`kartchrono-cli` follows the [Pure](https://github.com/kudima03/Pure) ecosystem conventions:

- Every type is a `sealed record` implementing exactly one interface.
- Behaviour lives in property getters — there are no methods beyond what the language forces.
- Values are `IString`, `INumber<T>` and `IBool` from
  [`Pure.Primitives.Abstractions`](https://github.com/kudima03/Pure.Primitives.Abstractions),
  never raw primitives.
- Everything is lazy and immutable; nothing is cached.
- `IAsyncEnumerable<T>` carries every I/O-backed sequence — socket frames, session snapshots
  and rendered output lines.

## Building

All `dotnet` commands run from `./src`:

```shell
dotnet restore
dotnet build --no-restore -warnaserror
dotnet test --no-build --collect:"XPlat Code Coverage"
```
