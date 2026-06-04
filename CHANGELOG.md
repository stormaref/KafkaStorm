# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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

[10.0.0]: https://github.com/stormaref/KafkaStorm/compare/v9.0.0...v10.0.0
