using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobStorage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlobStorageController : ControllerBase
    {
        private readonly BlobStorageService service;

        public BlobStorageController()
        {
            service = new BlobStorageService();
        }

        [HttpGet(nameof(GetStorageServiceProperties))]
        public async Task<BlobServiceProperties> GetStorageServiceProperties(string storageAccountUrl)
        {
            return await service.GetBlobServicePropertiesAsync(storageAccountUrl);
        }

        [HttpGet(nameof(GetContainerProperties))]
        public async Task<BlobContainerProperties> GetContainerProperties(string storageAccountUrl, string containerName)
        {
            return await service.GetBlobContainerPropertiesAsync(storageAccountUrl, containerName);
        }

        [HttpGet(nameof(GetBlobItems))]
        public async Task<IEnumerable<string>> GetBlobItems(string storageAccountUrl, string containerName)
        {
            var items = await service.GetBlobItemsAsync(storageAccountUrl, containerName);

            return items.Select(e => e.Name).ToList();
        }

        [HttpPost(nameof(CreateContainer))]
        public async Task<BlobContainerInfo> CreateContainer(string storageAccountUrl, string containerName)
        {
            return await service.CreateBlobContainerAsync(storageAccountUrl, containerName);
        }

        [HttpPost(nameof(UploadBlobItem))]
        public async Task<BlobContentInfo> UploadBlobItem(string storageAccountUrl, string containerName, string blobPath)
        {
            return await service.UploadFile(storageAccountUrl, containerName, Path.GetFileName(blobPath), blobPath);
        }

        [HttpDelete(nameof(DeleteBlobItem))]
        public async Task<bool> DeleteBlobItem(string storageAccountUrl, string containerName, string blobName)
        {
            return await Task.FromResult(true);
        }

        [HttpDelete(nameof(DeleteContainer))]
        public async Task<bool> DeleteContainer(string storageAccountUrl, string containerName)
        {
            return await Task.FromResult(true);
        }
    }
}