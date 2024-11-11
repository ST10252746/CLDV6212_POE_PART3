using Azure.Storage.Queues;

namespace ST10252746_CLDV6212_POE_PART3.Services
{
    // A service class for managing Azure Queue storage operations.
    public class QueueService
    {
        private readonly string _connectionString;

        // Constructor initializes the QueueService with a connection string.
        public QueueService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Method to send a message to a specified Azure Queue.
        public async Task SendMessageAsync(string queueName, string message)
        {
            // Create a QueueClient for the given queue using the connection string.
            var queueClient = new QueueClient(_connectionString, queueName);

            // Ensure the queue exists; create it if it doesn't.
            await queueClient.CreateIfNotExistsAsync();

            // Send the specified message to the queue.
            await queueClient.SendMessageAsync(message);
        }
    }
}
