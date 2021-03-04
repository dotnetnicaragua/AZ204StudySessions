using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoveItemsExampleConsole.Models
{
    public class Common
    {
        public static BlobServiceClient CreateBlobClientStorageFromSAS(string SASConnectionString)
        {
            BlobServiceClient blobClient;
            try
            {
                blobClient = new BlobServiceClient(SASConnectionString);
            }
            catch (System.Exception ex)
            {
                throw;
            }
            return blobClient;
        }
    }
}
