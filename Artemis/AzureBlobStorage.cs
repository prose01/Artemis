using Artemis.Interfaces;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Artemis
{
    public class AzureBlobStorage : IAzureBlobStorage
    {
        private readonly string _connectionString;
        private readonly BlobContainerClient _container;

        public AzureBlobStorage(IConfiguration config)
        {
            _connectionString = config.GetValue<string>("ConnectionString");
            _container = new BlobContainerClient(_connectionString, "photos");
        }

        public async Task UploadAsync(string profileId, string fileName, Stream fileStream)
        {
            // Get a reference to a container or then create it
            //BlobContainerClient container = new BlobContainerClient(_connectionString, "photos");
            await _container.CreateIfNotExistsAsync();

            try
            {
                // Get a reference to a blob
                BlobClient blob = _container.GetBlobClient(profileId);

                // Upload data to blob storage
                //await blob.UploadAsync(fileStream);
                //_container.UploadBlobAsync(fileName, fileStream);

                //// Open the file and upload its data
                //using (var filestream = File.Create(profileIdPath + "/" + fileName + ".png"))
                //{
                //    await blob.UploadAsync(fileStream);
                //    filestream.Flush();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //finally
            //{
            //    // Clean up after we're finished
            //    await container.DeleteAsync();
            //}
        }

        public async Task<Stream> DownloadImageByFileNameAsync(string profileId, string fileName)
        {
            try
            {
                // Get a reference to a blob
                BlobClient blob = _container.GetBlobClient(Path.Combine(profileId, fileName + ".png"));

                // Download the blob's contents and save it to a file
                BlobDownloadInfo download = await blob.DownloadAsync();

                return download.Content;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteImageByFileNameAsync(string profileId, string fileName)
        {
            try
            {
                // Get a reference to a blob
                BlobClient blob = _container.GetBlobClient(Path.Combine(profileId, fileName + ".png"));

                // DeleteAsync
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Stream>> DownloadAllImagesAsync(string profileId)
        {
            try
            {
                List<Stream> streams = new List<Stream>();

                return await this.ListBlobsHierarchicalListing(profileId, streams);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<Stream>> ListBlobsHierarchicalListing(string prefix, List<Stream> streams)
        {
            try
            {
                // Call the listing operation and return pages of the specified size.
                var resultSegment = _container.GetBlobsByHierarchyAsync(prefix: prefix, delimiter: "/")
                    .AsPages(default, 10);

                // Enumerate the blobs returned for each page.
                await foreach (Azure.Page<BlobHierarchyItem> blobPage in resultSegment)
                {
                    // A hierarchical listing may return both virtual directories and blobs.
                    foreach (BlobHierarchyItem blobhierarchyItem in blobPage.Values)
                    {
                        if (blobhierarchyItem.IsPrefix)
                        {
                            // Call recursively with the prefix to traverse the virtual directory.
                            await ListBlobsHierarchicalListing(blobhierarchyItem.Prefix, streams);
                        }
                        else
                        {
                            BlobClient blobitem = _container.GetBlobClient(blobhierarchyItem.Blob.Name);

                            // Download the blob's contents and add it to list
                            BlobDownloadInfo download = await blobitem.DownloadAsync();
                            streams.Add(download.Content);
                        }
                    }
                }

                return streams;
            }
            catch (RequestFailedException ex)
            {
                throw ex;
            }
        }

        public async Task DeleteAllImagesAsync(string profileId)
        {
            try
            {
                // Get a reference to a blob
                BlobClient blob = _container.GetBlobClient(profileId);

                // DeleteAsync
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
