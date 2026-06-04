# KafkaStorm

[![NuGet](https://img.shields.io/nuget/v/KafkaStorm.svg)](https://www.nuget.org/packages/KafkaStorm)
[![CI](https://github.com/stormaref/KafkaStorm/actions/workflows/ci.yml/badge.svg)](https://github.com/stormaref/KafkaStorm/actions/workflows/ci.yml)
[![License](https://img.shields.io/github/license/stormaref/KafkaStorm)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)

Simple .NET client for Kafka based on **Confluent.Kafka**.

## Table of contents

- [Requirements](#requirements)
- [Features](#features)
- [Installation](#installation)
- [Setup](#setup)
- [Assembly scanning](#assembly-scanning)
- [Consuming](#consuming)
- [Producing](#producing)
- [Examples](#examples)
- [Development](#development)
- [Migrating from 9.x](#migrating-from-9x)
- [Related](#related)

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (LTS)

## Features

- In-memory queue for messages that couldn't be sent
- Concurrent consumers via hosted services
- Producing messages concurrently
- JSON serialization via `System.Text.Json` (no Schema Registry required)

## Installation

### Package Manager

```
Install-Package KafkaStorm -Version 10.0.0
```

### .NET CLI

```
dotnet add package KafkaStorm --version 10.0.0
```

## Setup

```csharp
using Confluent.Kafka;
using KafkaStorm.Extensions;
using KafkaStorm.Interfaces;

builder.Services.AddKafkaStorm(factory =>
{
    factory.AddProducer(prf =>
    {
        prf.ConfigProducer(new ProducerConfig
        {
            BootstrapServers = host
        });

        prf.InMemoryQueue();
        prf.SetQueueLimit(65536);
    });

    // Start the background worker that drains the retry queue:
    factory.StartProducerHostedService();

    factory.AddConsumers(crf =>
    {
        crf.AddConsumer<HelloConsumer, HelloEvent>(new ConsumerConfig
        {
            BootstrapServers = "localhost:29092",
            GroupId = "TestGroup"
        }, "topicName");
    });
});
```

> Uses the same `ConsumerConfig` and `ProducerConfig` types as Confluent.Kafka.

## Assembly scanning

Register all `IConsumer<T>` / `IMessage` pairs from an assembly:

```csharp
using System.Reflection;
using Confluent.Kafka;
using KafkaStorm.Extensions;
using KafkaStorm.Interfaces;

builder.Services.AddKafkaStorm(factory =>
{
    factory.AddConsumers(crf =>
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:29092",
            GroupId = "TestGroup"
        };

        crf.AddConsumersFromAssembly(Assembly.GetExecutingAssembly(), config);
    });
});
```

Message types used with assembly scanning must implement `IMessage`.

## Consuming

```csharp
using KafkaStorm.Interfaces;
using Microsoft.Extensions.Logging;

public class HelloConsumer(ILogger<HelloConsumer> logger) : IConsumer<HelloEvent>
{
    public async Task Handle(HelloEvent @event, CancellationToken cancellationToken)
    {
        logger.LogDebug("Message received");
        await Task.CompletedTask;
    }
}
```

## Event (message)

```csharp
public class HelloEvent(DateTime time)
{
    public string Message { get; } = "Hello";
    public DateTime Time { get; } = time;
}
```

> **Note:** Properties typed as interfaces may cause JSON deserialization errors.

## Producing

Inject `IProducer`:

```csharp
using KafkaStorm.Interfaces;

public class TestService(IProducer producer)
{
    // Queue on failure (when in-memory queue is enabled):
    public Task QueueProduce() => producer.Produce(new HelloEvent(DateTime.Now), "topicName");

    // Send immediately:
    public Task SendNow() => producer.ProduceNowAsync(new HelloEvent(DateTime.Now), "topicName");
}
```

## Examples

See [`samples/BasicWebApi`](samples/BasicWebApi) for a minimal Web API that produces and consumes Kafka messages with KafkaStorm.

## Development

```bash
git clone https://github.com/stormaref/KafkaStorm.git
cd KafkaStorm
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

See [CONTRIBUTING.md](CONTRIBUTING.md) for contribution guidelines.

## Migrating from 9.x

- Target **.NET 10** and use package version **10.0.0**.
- Public registration APIs (`AddKafkaStorm`, `AddProducer`, `AddConsumer`, etc.) are unchanged.
- **Breaking:** Direct use of static fields on `ProducerRegistrationFactory` or `ConsumerRegistrationFactory` is no longer supported. Use normal DI registration instead.

See [CHANGELOG.md](CHANGELOG.md) for full release notes.

## Author

[@stormaref](https://www.github.com/stormaref)

## Related

- [Confluent's .NET Client](https://github.com/confluentinc/confluent-kafka-dotnet)
