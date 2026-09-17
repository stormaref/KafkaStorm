# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- Updated Microsoft.Extensions.* packages to 10.0.12 (ships the .NET 10.0.12 security fixes)
- Updated Confluent.Kafka to 2.15.1
- Updated FluentAssertions to 8.11.0
- Updated Microsoft.NET.Test.Sdk to 18.10.1
- Updated xunit.runner.visualstudio to 4.0.0

### Fixed

- `MessageStore` retry queue now has deterministic FIFO ordering. It was backed by a
  `ConcurrentDictionary` read via LINQ `.First()`/`.Last()`, whose enumeration order is undefined,
  so the message picked for retry and the message evicted at the queue limit were both arbitrary.
  Retries now preserve produce order and eviction always drops the oldest message.
- `MessageStore` operations are now O(1) instead of O(n); the producer polls the queue head in a
  loop, so the previous LINQ scans grew with queue depth.
- `ProducerHostedService` no longer spins without delay on a message that keeps failing to produce

### Added

- NuGet vulnerability auditing across direct and transitive packages (`NuGetAuditMode=all`)
- Reproducible/deterministic builds on CI so SourceLink resolves for package consumers

## [10.0.0] - 2026-06-04

### Added

- .NET 10 (LTS) target framework
- DI-backed configuration (`ProducerOptions`, `ConsumerRegistrationRegistry`, `ConsumerHostingOptions`)
- `BackgroundService`-based hosted services for producers and consumers
- Comprehensive unit test suite
- Sample Web API project under `samples/BasicWebApi`
- CI workflow separate from NuGet release workflow
- OSS documentation: CONTRIBUTING, SECURITY, CODE_OF_CONDUCT, issue/PR templates

### Changed

- Updated Confluent.Kafka to 2.14.0
- Updated Microsoft.Extensions.* packages to 10.0.0
- README modernized with badges, development guide, and examples
- Fixed NuGet `PackageId` typo (`KafKaStorm` → `KafkaStorm`)

### Removed

- Static registration fields on `ProducerRegistrationFactory` and `ConsumerRegistrationFactory`

### Breaking

- Direct access to static registration fields is no longer supported (use DI registration APIs)

## [9.0.0]

- Previous release targeting .NET 9

[Unreleased]: https://github.com/stormaref/KafkaStorm/compare/v10.0.0...HEAD
[10.0.0]: https://github.com/stormaref/KafkaStorm/compare/v9.0.0...v10.0.0
