using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobStorageConsoleExample.Models
{
    public class Common
    {
        public static CloudBlobClient CreateBlobClientStorageFromSAS(string SAStoken, string accountName)
        {
            CloudStorageAccount storageAccount;
            CloudBlobClient blobClient;
            try
            {
                bool useHttps = true;
                StorageCredentials storageCredentials = new StorageCredentials(SAStoken);
                storageAccount = new CloudStorageAccount(storageCredentials,
                accountName, null, useHttps);
                blobClient = storageAccount.CreateCloudBlobClient();
            }
            catch (Exception ex)
            {
                throw;
            }
            return blobClient;
        }
    }
}
