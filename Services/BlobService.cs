using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ST10252746_CLDV6212_POE_PART3.Services
{
    // A service class for managing Azure Blob Storage operations.
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "products";

        // Constructor initializes the BlobService with a connection string.
        public BlobService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        // Uploads a file to the specified blob container and returns the file's URL.
        public async Task<string> UploadAsync(Stream fileStream, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream);

            // Return the URL of the uploaded blob.
            return blobClient.Uri.ToString();
        }

        // Deletes a blob from the specified container based on the blob's URI.
        public async Task DeleteBlobAsync(string blobUri)
        {
            Uri uri = new Uri(blobUri);
            string blobName = uri.Segments[^1]; // Get the blob name from the URI.
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            // Delete the blob, including any snapshots.
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }
    }
}
