using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using FileHelper;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.Entity.Orders;
using Microsoft.AspNetCore.Http;

namespace ManyForMany.Model.File
{
    public class ImageManager
    {
        private readonly FileManager fileManager = new FileManager();

        public static readonly string OrderDirectory = nameof(Order);
        public static readonly string UserDirectory = nameof(ApplicationUser);

        public async Task<bool> UploadOrderImages(Image[] file, string userId, int orderId)
        {
            return await fileManager.UploadFile(file, UserDirectory, userId, OrderDirectory, orderId.ToString());
        }

        public async Task<bool> UploadOrderImages(Image file, string userId, int orderId)
        {
            return await fileManager.UploadFile(file, UserDirectory, userId, OrderDirectory, orderId.ToString());
        }

        public async Task<Image[]> DownladOrderImages(string mail, int orderId)
        {
            return await fileManager.DownloadFiles<Image>(UserDirectory, mail, OrderDirectory, orderId.ToString());
        }

        public async Task RemoveOrderImages(string mail, int orderId)
        {
            await fileManager.RemoveFiles(UserDirectory, mail, OrderDirectory, orderId.ToString());
        }
    }
}
