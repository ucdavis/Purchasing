﻿using System;
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
    public interface IFileService
    {
        /// <summary>
        /// Return the attachment associated with the given attachmentId, with contents populated
        /// </summary>
        /// <param name="id">attachmentID</param>
        /// <returns></returns>
        Attachment GetAttachment(Guid id);

        /// <summary>
        /// Upload an attachment to blob storage
        /// </summary>
        void UploadAttachment(Guid id, Stream fileStream);
    }

    public class FileService : IFileService
    {
        private readonly IRepositoryWithTypedId<Attachment, Guid> _attachmentRepository;
        private readonly CloudBlobContainer _container;

        public FileService(IRepositoryWithTypedId<Attachment, Guid> attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;

            var storageConnectionString =
                string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                              CloudConfigurationManager.GetSetting("AzureStorageAccountName"),
                              CloudConfigurationManager.GetSetting("AzureStorageKey"));

            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            _container = blobClient.GetContainerReference("oppattachments");
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
                using (var stream = new MemoryStream())
                {
                    blob.DownloadToStream(stream);
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
        public void UploadAttachment(Guid id, Stream fileStream)
        {
            var blob = _container.GetBlockBlobReference(id.ToString());
            blob.UploadFromStream(fileStream);
        }
    }
}