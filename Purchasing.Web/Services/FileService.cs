using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using UCDArch.Core.PersistanceSupport;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Services
{
    public class FileService
    {
        private readonly IRepositoryWithTypedId<Attachment, Guid> _attachmentRepository;
        private readonly CloudBlobContainer _container;

        public FileService(IRepositoryWithTypedId<Attachment, Guid> attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;

            var storageAccount =
                CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var blobClient = storageAccount.CreateCloudBlobClient();

            _container = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting("FileContainer"));
            _container.CreateIfNotExists();
            _container.SetPermissions(new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Off});

        }

        /// <summary>
        /// Return the attachment associated with the given attachmentId, with contents populated
        /// </summary>
        /// <param name="id">attachmentID</param>
        /// <returns></returns>
        public Attachment GetAttachment(Guid id)
        {
            var file = _attachmentRepository.GetNullableById(id);

            if (file == null) return null;

            if (file.IsBlob)
            {
                //Get file from blob storage and populate the contents
                var blob = _container.GetBlockBlobReference(id.ToString());
                blob.DownloadToByteArray(file.Contents, 0);
            }

            return file;
        }

        /// <summary>
        /// Upload an attachment to blob storage
        /// </summary>
        public void UploadAttachment(Attachment attachment)
        {
            if (attachment == null) return;

            var blob = _container.GetBlockBlobReference(attachment.Id.ToString());
            blob.UploadFromByteArray(attachment.Contents, 0, attachment.Contents.Length);

            attachment.IsBlob = true;
            attachment.Contents = null; //Clear the contents because they have been uploaded to blob storage
        }
    }
}