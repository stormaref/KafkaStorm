# Contributing to KafkaStorm

Thank you for your interest in contributing!

## Getting started

1. Fork the repository and clone it locally.
2. Ensure you have the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) installed.
3. Run the test suite:

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

## Making changes

- Keep pull requests focused on a single concern.
- Follow existing code style (file-scoped namespaces, nullable enabled, primary constructors where appropriate).
- Add or update unit tests for behavior changes.
- Do not call real Kafka brokers in unit tests.

## Pull request checklist

- [ ] `dotnet build -c Release` succeeds
- [ ] `dotnet test -c Release` passes
- [ ] README or CHANGELOG updated if user-facing behavior changed

## Commit messages

Use clear, descriptive commit messages. Prefer complete sentences that explain **why**, not just what.

## Questions

Open a [GitHub issue](https://github.com/stormaref/KafkaStorm/issues) for bugs or feature discussions before large changes.
