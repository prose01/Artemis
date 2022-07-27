using Artemis.Interfaces;
using Artemis.Model;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
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

        public async Task<Stream> DownloadImageByFileNameAsync(string profileId, string fileName)
        {
            try
            {
                // TODO: Temp condition to add jpeg to un-typed images.
                if (!fileName.Contains('.'))
                {
                    fileName += ".jpeg";
                }

                // Get a reference to a blob
                BlobClient blob = _container.GetBlobClient(Path.Combine(profileId, fileName));

                // Download the blob's contents and save it to a file
                BlobDownloadInfo download = await blob.DownloadAsync();

                return download.Content;

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

        public async Task<List<Stream>> DownloadAllImagesAsync(string profileId, ImageSizeEnum imageSize)
        {
            try
            {
                //var path = Path.Combine(profileId, imageSize.ToString());

                List<Stream> streams = new List<Stream>();

                // Get a reference to a blob
                var blobItems = _container.GetBlobsAsync(prefix: profileId + "/" + imageSize.ToString() + "/");

                // Get everything Async
                await foreach (BlobItem blobItem in blobItems)
                {
                    BlobClient blobClient = _container.GetBlobClient(blobItem.Name);

                    // Download the blob's contents and add it to list
                    BlobDownloadInfo download = await blobClient.DownloadAsync();
                    streams.Add(download.Content);
                }

                return streams;

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

        //private async Task<List<Stream>> ListBlobsHierarchicalListing(string prefix, List<Stream> streams)
        //{
        //    try
        //    {
        //        // Call the listing operation and return pages of the specified size.
        //        var resultSegment = _container.GetBlobsByHierarchyAsync(prefix: prefix, delimiter: "/")
        //            .AsPages(default, 10);

        //        // Enumerate the blobs returned for each page.
        //        await foreach (Azure.Page<BlobHierarchyItem> blobPage in resultSegment)
        //        {
        //            // A hierarchical listing may return both virtual directories and blobs.
        //            foreach (BlobHierarchyItem blobhierarchyItem in blobPage.Values)
        //            {
        //                if (blobhierarchyItem.IsPrefix)
        //                {
        //                    // Call recursively with the prefix to traverse the virtual directory.
        //                    await ListBlobsHierarchicalListing(blobhierarchyItem.Prefix, streams);
        //                }
        //                else
        //                {
        //                    BlobClient blobitem = _container.GetBlobClient(blobhierarchyItem.Blob.Name);

        //                    // Download the blob's contents and add it to list
        //                    BlobDownloadInfo download = await blobitem.DownloadAsync();
        //                    streams.Add(download.Content);
        //                }
        //            }
        //        }

        //        return streams;
        //    }
        //    catch (RequestFailedException ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
