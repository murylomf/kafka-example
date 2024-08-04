using Carter;
using Carter.OpenApi;
using Confluent.Kafka;
using FluentValidation;
using KafkaExample.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KafkaExample.Features;
public class Kafka
{
    public record Command() : IRequest;

    public class Handler() : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = "meu-grupo",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("meu-topico");

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                while (true)
                {
                    var cr = consumer.Consume(cts.Token);
                    Console.WriteLine($"Mensagem: '{cr.Value}' recebida do tópico '{cr.Topic}' na partição '{cr.Partition}' no offset '{cr.Offset}'");
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
        }
    }
}

public class kafkaModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/create-consumer", async (ISender sender) =>
            {
                var query = new Kafka.Command();
                await sender.Send(query);
            })
            .WithTags("Kafka")
            .WithName("CreateProduct")
            .IncludeInOpenApi();
        
        app.MapPost("api/create-producer", async (ISender sender, ProducerRequest request) =>
            {
                var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

                using var producer = new ProducerBuilder<Null, string>(config).Build();
                try
                {
                    var deliveryResult = await producer.ProduceAsync("meu-topico", new Message<Null, string> { Value = request.Value });
                    Console.WriteLine($"Mensagem: '{deliveryResult.Value}' enviada para o tópico '{deliveryResult.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Erro ao enviar mensagem: {e.Error.Reason}");
                }
            })
            .WithTags("Kafka")
            .WithName("CreateProducer")
            .IncludeInOpenApi();
    }
}