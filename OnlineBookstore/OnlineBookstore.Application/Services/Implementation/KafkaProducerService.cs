using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using OnlineBookstore.Application.Services.Interfaces;

namespace OnlineBookstore.Application.Services.Implementation
{
    public class KafkaProducerService : IKafkaProducerService, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;
        private bool _disposed;

        public KafkaProducerService(ILogger<KafkaProducerService> logger)
        {
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:29092",
                ClientId = "recommendations-group",
                MessageTimeoutMs = 5000,
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
            _logger.LogInformation("Kafka producer service initialized with bootstrap servers: {BootstrapServers}",
                "localhost:29092");
        }

        public async Task ProduceAsync<TKey, TValue>(string topic, TKey key, TValue value)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentNullException(nameof(topic));

            try
            {
                // Serialize the key and value
                string serializedKey = key?.ToString() ?? string.Empty;
                string serializedValue = JsonSerializer.Serialize(value);

                var message = new Message<string, string>
                {
                    Key = serializedKey,
                    Value = serializedValue
                };

                // Produce the message to Kafka
                var deliveryResult = await _producer.ProduceAsync(topic, message);

                _logger.LogInformation(
                    "Message delivered to {Topic} [{Partition}] at offset {Offset}",
                    deliveryResult.Topic,
                    deliveryResult.Partition,
                    deliveryResult.Offset);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Error producing message to topic {Topic}: {ErrorReason}",
                    topic, ex.Error.Reason);
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _producer?.Flush(TimeSpan.FromSeconds(10));
                _producer?.Dispose();
                _logger.LogInformation("Kafka producer service disposed");
            }

            _disposed = true;
        }
    }
}
