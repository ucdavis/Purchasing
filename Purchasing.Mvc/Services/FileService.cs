using System;
using System.IO;
using Microsoft.Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Purchasing.Mvc.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Return the attachment associated with the given attachmentId, with contents populated
        /// </summary>
        /// <param name="id">attachmentID</param>
        /// <returns></returns>
        Task<Attachment> GetAttachment(Guid id);

        /// <summary>
        /// Upload an attachment to blob storage
        /// </summary>
        Task UploadAttachment(Guid id, Stream fileStream);
    }

    public class FileService : IFileService
    {
        private readonly IRepositoryWithTypedId<Attachment, Guid> _attachmentRepository;
        private readonly BlobServiceClient _blobServiceClient;

        public FileService(IRepositoryWithTypedId<Attachment, Guid> attachmentRepository, IConfiguration configuration)
        {
            _attachmentRepository = attachmentRepository;

            var storageConnectionString =
                string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                              configuration["AzureStorageAccountName"],
                              configuration["AzureStorageKey"]);

            _blobServiceClient = new BlobServiceClient(storageConnectionString);
        }

        /// <summary>
        /// Return the attachment associated with the given attachmentId, with contents populated
        /// </summary>
        /// <param name="id">attachmentID</param>
        /// <returns></returns>
        public async Task<Attachment> GetAttachment(Guid id)
        {
            var file = _attachmentRepository.GetNullableById(id);

            if (file == null) return null;

            if (file.IsBlob)
            {
                //Get file from blob storage and populate the contents
                using (var stream = new MemoryStream())
                {
                    var containerClient = await GetBlobContainer();
                    var blobClient = containerClient.GetBlockBlobClient(id.ToString());
                    await blobClient.DownloadToAsync(stream);
                    using (var reader = new BinaryReader(stream))
                    {
                        stream.Position = 0;
                        file.Contents = reader.ReadBytes((int)stream.Length);
                    }
                }
            }

            return file;
        }

        /// <summary>
        /// Upload an attachment to blob storage
        /// </summary>
        public async Task UploadAttachment(Guid id, Stream fileStream)
        {
            var containerClient = await GetBlobContainer();
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
            var blobClient = containerClient.GetBlockBlobClient(id.ToString());
            await blobClient.UploadAsync(fileStream);
        }

        private async Task<BlobContainerClient> GetBlobContainer()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("oppattachments");
            await containerClient.CreateIfNotExistsAsync();

            return containerClient;
        }
    }
}