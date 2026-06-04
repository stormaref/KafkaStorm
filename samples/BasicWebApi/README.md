# BasicWebApi Sample

Minimal .NET 10 Web API demonstrating KafkaStorm producer and consumer registration.

## Prerequisites

- .NET 10 SDK
- A running Kafka broker (default: `localhost:29092`)

## Run

```bash
cd samples/BasicWebApi
dotnet run
```

## Send a message

```bash
curl -X POST http://localhost:5000/messages \
  -H "Content-Type: application/json" \
  -d '{"message":"Hello KafkaStorm","timestamp":"2026-06-04T12:00:00Z"}'
```

The consumer logs the message when it arrives on the `hello-events` topic.
