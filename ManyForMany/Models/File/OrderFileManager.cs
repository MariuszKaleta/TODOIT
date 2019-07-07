using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Entity.Order;
using ManyForMany.ViewModel;
using Microsoft.AspNetCore.Http;

namespace ManyForMany.Models.File
{
    public class OrderFileManager
    {
        private readonly FileManager fileManager = new FileManager();

        public static readonly string OrderDirectory = nameof(Order);
        public static readonly string UserDirectory = nameof(HttpContext.User);
        public static readonly string ImageDirectory = nameof(Image);
        public static readonly string FileDirectory = nameof(File);

        public async Task<bool> UploadOrderImages(FileViewModel[] files, string userId, string orderId)
        {
            return await fileManager.UploadFile(files.Select(x => new File(x)).ToArray(), UserDirectory, userId,
                OrderDirectory, orderId, ImageDirectory);
        }

        public async Task<bool> UploadOrderImages(FileViewModel file, string userId, string orderId)
        {
            return await fileManager.UploadFile(new File(file), UserDirectory, userId, OrderDirectory,  orderId, ImageDirectory);
        }

        public async Task<File[]> DownladOrderImages(string userId, string orderId)
        {
            return await fileManager.DownloadFiles(UserDirectory, userId, OrderDirectory,  orderId, ImageDirectory);
        }

        public void RemoveOrderImages(string userId, string orderId, string[] fileNames)
        {
            fileManager.RemoveFiles(new[] { UserDirectory, userId, OrderDirectory, orderId, ImageDirectory }, fileNames);
        }

        public void RemoveOrderImages(string userId, string orderId, string fileName)
        {
            fileManager.RemoveFiles(new[] { UserDirectory, userId, OrderDirectory, orderId, ImageDirectory }, fileName);
        }
        public async Task<bool> UploadOrderFiles(FileViewModel[] files, string userId, string orderId)
        {
            return await fileManager.UploadFile(files.Select(x => new File(x)).ToArray(), UserDirectory, userId,
                OrderDirectory,  orderId, FileDirectory);
        }

        public async Task<bool> UploadOrderFiles(FileViewModel file, string userId, string orderId)
        {
            return await fileManager.UploadFile(new File(file), UserDirectory, userId, OrderDirectory,  orderId, FileDirectory);
        }

        public async Task<File[]> DownladOrderFiles(string userId, string orderId)
        {
            return await fileManager.DownloadFiles(UserDirectory, userId, OrderDirectory,  orderId, FileDirectory);
        }

        public void RemoveOrderFiles(string userId, string orderId, string[] fileNames)
        {
            fileManager.RemoveFiles(new[] { UserDirectory, userId, OrderDirectory, orderId, FileDirectory }, fileNames);
        }

        public void RemoveOrderFiles(string userId, string orderId, string fileName)
        {
            fileManager.RemoveFiles(new[] { UserDirectory, userId, OrderDirectory, orderId, FileDirectory }, fileName);
        }

        
    }
}
