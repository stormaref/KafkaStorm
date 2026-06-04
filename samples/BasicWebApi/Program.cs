using Confluent.Kafka;
using KafkaStorm.Extensions;
using KafkaStorm.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var kafkaBootstrap = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:29092";

builder.Services.AddKafkaStorm(factory =>
{
    factory.AddProducer(prf =>
    {
        prf.ConfigProducer(new ProducerConfig { BootstrapServers = kafkaBootstrap });
        prf.InMemoryQueue();
        prf.SetQueueLimit(65536);
    });

    factory.StartProducerHostedService();

    factory.AddConsumers(crf =>
    {
        crf.AddConsumer<HelloConsumer, HelloEvent>(new ConsumerConfig
        {
            BootstrapServers = kafkaBootstrap,
            GroupId = "BasicWebApi"
        }, "hello-events");
    });
});

var app = builder.Build();

app.MapPost("/messages", async (HelloEvent message, IProducer producer) =>
{
    await producer.ProduceNowAsync(message, "hello-events");
    return Results.Accepted();
});

app.MapGet("/", () => Results.Ok(new { service = "BasicWebApi", kafka = kafkaBootstrap }));

app.Run();

public record HelloEvent(string Message, DateTime Timestamp);

public sealed class HelloConsumer(ILogger<HelloConsumer> logger) : IConsumer<HelloEvent>
{
    public Task Handle(HelloEvent message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received message: {Message} at {Timestamp}", message.Message, message.Timestamp);
        return Task.CompletedTask;
    }
}
