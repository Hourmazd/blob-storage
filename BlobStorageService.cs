using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure;
using static Azure.Core.HttpHeader;

namespace AzureBlobStorage
{
    public class BlobStorageService
    {
        private BlobServiceClient CreateBlobServiceClient(string storageAccountUrl)
        {
            BlobServiceClient serviceClient = new BlobServiceClient(storageAccountUrl);

            return serviceClient;
        }

        public async Task<BlobServiceProperties> GetBlobServicePropertiesAsync(string storageAccountUrl)
        {
            var serviceClient = CreateBlobServiceClient(storageAccountUrl);
            var properties = await serviceClient.GetPropertiesAsync();

            if (properties is null)
            {
                throw new Exception($"Unable to fetch properties of storage service.");
            }

            return properties;
        }

        public BlobContainerClient GetBlobContainerClient(BlobServiceClient blobServiceClient, string containerName)
        {
            BlobContainerClient client = blobServiceClient.GetBlobContainerClient(containerName);

            return client;
        }

        public async Task<BlobContainerProperties> GetBlobContainerPropertiesAsync(string storageAccountUrl, string containerName)
        {
            var blobServiceClient = CreateBlobServiceClient(storageAccountUrl);
            BlobContainerClient client = GetBlobContainerClient(blobServiceClient, containerName);

            if (await client.ExistsAsync())
            {
                return await client.GetPropertiesAsync();
            }

            throw new Exception($"Container {containerName} does not exist.");
        }

        public async Task<BlobContainerInfo> CreateBlobContainerAsync(string storageAccountUrl, string containerName)
        {
            var blobServiceClient = CreateBlobServiceClient(storageAccountUrl);
            BlobContainerClient client = GetBlobContainerClient(blobServiceClient, containerName);

            var response = await client.CreateIfNotExistsAsync();

            return response.Value;
        }

        public async Task<List<BlobItem>> GetBlobItemsAsync(string storageAccountUrl, string containerName)
        {
            var blobServiceClient = CreateBlobServiceClient(storageAccountUrl);
            BlobContainerClient client = GetBlobContainerClient(blobServiceClient, containerName);

            var result = client.GetBlobsAsync();
            var enumerator = result.GetAsyncEnumerator();
            var items = new List<BlobItem>();

            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    items.Add(enumerator.Current);
                }
            }
            finally
            {
                if (enumerator != null)
                    await enumerator.DisposeAsync();
            }

            return items;
        }

        public async Task<BlobContentInfo> UploadFile(string storageAccountUrl, string containerName, string blobName, string blobPath)
        {
            var blobServiceClient = CreateBlobServiceClient(storageAccountUrl);
            BlobContainerClient client = GetBlobContainerClient(blobServiceClient, containerName);

            var stream = File.OpenRead(blobPath);

            var result = await client.UploadBlobAsync(blobName, stream);

            stream.Close();

            return result;
        }
    }
}
