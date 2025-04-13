using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Recommendations.Abstractions.MessageHandlers;
using Recommendations.Abstractions.Messages;
using Recommendations.Api.Settings;
using System.Text.Json;

namespace Recommendations.Api;
/// <summary>
/// Background service that consumes messages from Kafka
/// </summary>
public class KafkaConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly KafkaSettings _kafkaSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaConsumerService"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for creating scoped services</param>
    /// <param name="kafkaSettings">The Kafka configuration settings</param>
    /// <param name="logger">The logger</param>
    public KafkaConsumerService(
        IServiceProvider serviceProvider,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<KafkaConsumerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _kafkaSettings = kafkaSettings.Value;
    }

    /// <summary>
    /// Executes the consumer service.
    /// </summary>
    /// <param name="stoppingToken">Triggered when the service is stopping</param>
    /// <returns>A task representing the asynchronous operation</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer Service is starting");

        // Start a consumer for book deletion messages
        await ConsumeMessagesAsync<BookDeletedMessage>(
            _kafkaSettings.BookDeletedTopic,
            stoppingToken);

        _logger.LogInformation("Kafka Consumer Service is stopping");
    }

    /// <summary>
    /// Consumes messages from a Kafka topic and processes them with the appropriate handler
    /// </summary>
    /// <typeparam name="TMessage">The type of message to consume</typeparam>
    /// <param name="topic">The Kafka topic to consume from</param>
    /// <param name="stoppingToken">Triggered when the service is stopping</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task ConsumeMessagesAsync<TMessage>(string topic, CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting consumer for topic: {Topic}", topic);

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    if (consumeResult != null)
                    {
                        _logger.LogInformation("Received message from topic: {Topic}", topic);

                        var message = JsonSerializer.Deserialize<TMessage>(consumeResult.Message.Value);

                        if (message != null)
                        {
                            using var scope = _serviceProvider.CreateScope();
                            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<TMessage>>();
                            await handler.HandleAsync(message);
                        }

                        // Commit the offset after processing
                        consumer.Commit(consumeResult);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from topic: {Topic}", topic);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from topic: {Topic}", topic);
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }
}