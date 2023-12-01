﻿using Artemis.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Artemis
{
    // Got most of the code from Azure Storage Blobs client library for .NET
    // https://github.com/Azure/azure-sdk-for-net/blob/Azure.Storage.Blobs_12.7.0/sdk/storage/Azure.Storage.Blobs/README.md

    public class AzureBlobStorage : IAzureBlobStorage
    {
        private readonly BlobContainerClient _container;

        public AzureBlobStorage(IConfiguration config)
        {
            _container = new BlobContainerClient(config.GetValue<string>("Storage_ConnectionString"), "photos");
        }

        public async Task UploadAsync(string profileId, string fileName, Stream fileStream)
        {
            try
            {
                await _container.UploadBlobAsync(Path.Combine(profileId, fileName), fileStream);
            }
            catch
            {
                throw;
            }
        }

        public async Task DeleteImageByFileNameAsync(string fileName)
        {
            try
            {
                // Get a reference to a blob
                BlobClient blob = _container.GetBlobClient(fileName);

                // DeleteAsync
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

            }
            catch
            {
                throw;
            }
        }

        public async Task DeleteAllImagesAsync(string profileId)
        {
            try
            {
                // Get a reference to a blob
                var blobItems = _container.GetBlobsAsync(prefix: profileId);

                // Delete everything Async
                await foreach (BlobItem blobItem in blobItems)
                {
                    BlobClient blobClient = _container.GetBlobClient(blobItem.Name);
                    await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                }

            }
            catch
            {
                throw;
            }
        }
    }
}
