using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileHelper;
using ManyForMany.Model.Entity.Orders;
using Microsoft.AspNetCore.Http;

namespace ManyForMany.Model.File
{
    public class ImageManager
    {
        private readonly FileManager fileManager = new FileManager();

        public async Task<bool> UploadFile(Image[] file, string userId, int orderId)
        {
            return await fileManager.UploadFile(file, userId, orderId.ToString());
        }

        public async Task<bool> UploadFile(Image file, string userId, int orderId)
        {
            return await fileManager.UploadFile(file, userId, orderId.ToString());
        }

        public async Task<Image[]> DownladFiles(string userId, int orderId)
        {
            return await fileManager.DownloadFiles<Image>(userId, orderId.ToString());
        }
    }
}
