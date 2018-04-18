using System;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureStorageAccount
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string accountName = "YourAccountName";
            const string accountKey ="YourAccountKey";
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);

            var blobClient = storageAccount.CreateCloudBlobClient();
            // container name can not contain upper case letters
            var container = blobClient.GetContainerReference("myblockcontainer");

            //var pageBlob = CreatePageBlob(container);
            //CreateContainer(container);

            //UploadFile(container);

            //ReadBlobInformation(container);

            //SetMetadata(container);

            //ReadUserDefinedMetadata(container);

            //ReadSystemMetadata(container);

            //WriteToPageBlob(pageBlob);

            //ReadPageBlob(pageBlob);

            //Console.WriteLine(GetLeaseId(container));
        }

        private static CloudPageBlob CreatePageBlob(CloudBlobContainer container)
        {
            var pageBlobName = "mypageblob";
            var pageBlob = container.GetPageBlobReference(pageBlobName);

            CreatePageBlob(pageBlob);

            return pageBlob;
        }

        private static async void CreatePageBlob(CloudPageBlob pageBlob, int amountOfBlobs = 1)
        {
            await pageBlob.CreateAsync(512 * amountOfBlobs);
        }

        private static async void CreateContainer(CloudBlobContainer container)
        {
            try
            {
                await container.CreateIfNotExistsAsync();
            }
            catch (StorageException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void UploadFile(CloudBlobContainer container)
        {
            // Source
            const string fileToUpload = "FilePath";
            // Destination
            var blockBlob = container.GetBlockBlobReference("BlobName");
            // Create or overwrite the "BlobName" blob with contents from a local file.
            using (var fileStream = File.OpenRead(fileToUpload))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }

        private static void SetMetadata(CloudBlobContainer container)
        {
            container.Metadata.Add("Author", "Wolfgang");
            container.SetMetadata();
        }

        private static void ReadBlobInformation(CloudBlobContainer container)
        {
            foreach (var blob in container.ListBlobs())
            {
                Console.WriteLine($"- {blob.Uri} (type: {blob.GetType()})");
            }
        }

        private static void ReadUserDefinedMetadata(CloudBlobContainer container)
        {
            container.FetchAttributes();

            foreach (var metadataItem in container.Metadata)
            {
                Console.WriteLine($"Key: {metadataItem.Key}");
                Console.WriteLine($"Value: {metadataItem.Value}");
            }
        }

        private static void ReadSystemMetadata(CloudBlobContainer container)
        {
            container.FetchAttributes();
            Console.WriteLine($"LastModified: {container.Properties.LastModified}");
            Console.WriteLine($"LeaseState: {container.Properties.LeaseState}");
        }

        private static async void WriteToPageBlob(CloudPageBlob pageBlob)
        {
            var samplePagedata = new byte[512];
            var random = new Random();
            random.NextBytes(samplePagedata);
            
            await pageBlob.UploadFromByteArrayAsync(samplePagedata, 0, samplePagedata.Length);
        }

        private static async void ReadPageBlob(CloudPageBlob pageBlob)
        {
            var samplePagedata = new byte[512];

            await pageBlob.DownloadRangeToByteArrayAsync(samplePagedata,
                0, 0, samplePagedata.Length);
        }

        private static string GetLeaseId(CloudBlobContainer container)
        {
            TimeSpan? leaseTime = TimeSpan.FromSeconds(60);
            var leaseId = container.AcquireLease(leaseTime, null);

            return leaseId;
        }
    }
}
