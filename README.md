# KafkaStorm

Simple .net client for Kafka based on **Confluent.Kafka**

## Features

- Create queue for messages that couldn't be send
- Concurrent consumers
- Producing messages concurrently

## Installation

### Using package manager:
```
Install-Package KafkaStorm -Version 1.9.0
```

# Usage/Examples
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

    // Use this line for starting producer queue:
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

> It's the same ConsumerConfig as Confluent.Kafka

## Consuming
```csharp
using KafkaStorm.Interfaces;  
using Microsoft.Extensions.Logging;

public class HelloConsumer : IConsumer<HelloEvent>  
{  
  private readonly ILogger<HelloConsumer> _logger;  
  
  public HelloConsumer(ILogger<HelloConsumer> logger)  
 {
	 _logger = logger;  
 }  
  public async Task Handle(HelloEvent @event, CancellationToken cancellationToken)  
 {  
	 _logger.LogDebug("Message Received");  
 }}
```

## Event
Your event (message) can be any class like this:
```csharp
public class HelloEvent  
{  
  public HelloEvent(DateTime time)  
 {
	 Message = "Hello";  
	 Time = time;  
 }  
 
  public string Message { get; }  
  public DateTime Time { get; }  
}
```

> ***Attention:*** if your class contains a property with Interface type it may cause exception while deserializing JSON


## Producing

Just use **IProducer** like a service (initialize it with constructor):
```csharp
using KafkaStorm.Interfaces;

private readonly IProducer _producer;  
  
public TestService(IProducer producer)  
{  
	_producer = producer;  
}
```

- ### Produce with queue
```csharp
_producer.Produce(new HelloEvent(DateTime.Now), "topicName");
```

- ### Produce right now
```csharp
await _producer.ProduceNowAsync(new HelloEvent(DateTime.Now), "topicName");
```


# Author

[@stormaref](https://www.github.com/stormaref)

# Related

## Here are some related projects

[Confluent's .NET Client](https://github.com/confluentinc/confluent-kafka-dotnet)
