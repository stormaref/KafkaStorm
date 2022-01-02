# KafkaStorm

Simple client for Kafka based on **Confluent.Kafka** (https://github.com/confluentinc/confluent-kafka-dotnet)

# How to install

Using package manager:
```
Install-Package KafkaStorm -Version 1.0.0
```

# How to use

**.Net6 Example**

## Consuming

```csharp
using Confluent.Kafka;

builder.Services.AddKafkaStorm(factory =>  
{  
 factory.SetConsumerConfig(new ConsumerConfig()  
  {  
	  BootstrapServers = "localhost:29092",  
	  GroupId = "TestGroup"  
  });  
  
 factory.AddProducer(new ProducerConfig()  
  {  
	  BootstrapServers = "localhost:29092",  
  });  
  
 factory.AddConsumer<HelloConsumer, HelloEvent>();  
});
```

> It's the same ConsumerConfig as Confluent.Kafka

Your consumer should look like this:
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

And send message like this:
```csharp
await _producer.ProduceAsync(new HelloEvent(DateTime.Now));
```
