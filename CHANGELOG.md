# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2026-09-08

### Added

- Native AOT binaries for `osx-x64`.

### Changed

- The release workflow now gates every build/publish job on the existing
  `.NET build & test` workflow (format, csharpier, tests, coverage and mutation
  gates), called as a reusable workflow, instead of publishing an untested tag.
- `ICompetitor.GapMilliseconds`/`DifferenceMilliseconds` are now `Gap`/`Difference`,
  each paired with a `GapIsTime`/`DifferenceIsTime` flag, since a negative value
  encodes laps or sectors behind rather than a time.

## [0.1.0] - 2026-09-08

### Added

- `kartchrono` CLI: `tracks`, `session`, `live`, `laps` and `records` commands over the
  KartChrono WebSocket and archive endpoints, with `--track`, `--kart`, `--period`,
  `--json`, `--help` and `--version`.
- Native AOT packaging as a `dotnet tool` for `linux-x64`, `linux-arm64`, `osx-arm64`
  and `win-x64`, with a framework-dependent fallback for every other platform.
- Multi-targeted `net8.0`/`net9.0`/`net10.0` builds so the NuGet package installs on
  older supported .NET SDKs, not just the one used to build it.
- A release workflow that builds each native binary on a matching runner, publishes a
  single framework-dependent NuGet package via NuGet Trusted Publishing (OIDC, no
  stored API key), and attaches standalone binaries to the GitHub release.
- Package validation (`EnablePackageValidation`), ready to enforce API compatibility
  against a baseline once the first version is published.

### Changed

- Replaced the `Pure.Template` boilerplate with `kartchrono-cli` project metadata.
- Lowered the CI coverage gate to 80% and the mutation gate to 60%.
- Pointed Dependabot at the projects under `src/`.

### Removed

- The NuGet library publish workflow, which does not apply to a CLI.
