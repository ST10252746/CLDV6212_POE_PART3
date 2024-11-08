using Azure.Storage.Queues;

namespace ST10252746_CLDV6212_POE_PART3.Services
{
    public class QueueService
    {
        private readonly string _connectionString;

        public QueueService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task SendMessageAsync(string queueName, string message)
        {
            var queueClient = new QueueClient(_connectionString, queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);
        }
    }
}