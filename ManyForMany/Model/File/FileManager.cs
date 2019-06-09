using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FileHelper;
using ManyForMany.Model.Entity.Orders;
using Microsoft.AspNetCore.Http;
using GenericHelper;

namespace ManyForMany.Model.File
{
    public class FileManager
    {
        #region properties

        public const uint NextIndex = 1;

        public static string UploadedFiles { get; } = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameof(UploadedFiles)));

        public static string[] SupportedImageFormats =
            Helper.GetAllPropertiesOfType<ImageFormat, ImageFormat>(BindingFlags.Public | BindingFlags.Static).Select(x => x.ToString().ToLower()).ToArray();

        public static string LocalPath(params string[] directories) =>
            Path.Combine(UploadedFiles, string.Join(FileConstant.PathSeparator, directories));

        #endregion

        #region Method


        public async Task<bool> UploadFile(File[] files, params string[] directories)
        {
            var tasks = files.Select(file => UploadFile(file, directories));

            return (await Task.WhenAll(tasks)).All(x => x);
        }

        public async Task<bool> UploadFile(File file, params string[] directories)
        {
            var isCopied = false;
            //1 check if the file length is greater than 0 bytes 
            if (file.Data.Length > 0)
            {
                var fileName = string.Empty;

                var extension = file.Extension;

                if (SupportedImageFormats.Any(x => x == extension.ToLower()))
                {
                    var directoryPath = LocalPath(directories);
                    Directory.CreateDirectory(directoryPath);

                    await file.Save(GetAvailableFilename(directoryPath, fileName));
                    isCopied = true;
                }
                else
                {
                    throw new Exception(Exceptions.UnsupportedFileFormat(SupportedImageFormats));
                }
            }

            return isCopied;
        }

        public async Task<T[]> DownloadFiles<T>(params string[] directories)
        where T : File, new()
        {
            var localPath = LocalPath(directories);

            var files = Directory.GetFiles(localPath);
            var tasks = files.Select(File.Load<T>).ToArray();

            return await Task.WhenAll(tasks);
        }

        public async Task RemoveFiles(params string[] directories)
        {
            Directory.Delete(LocalPath(directories));
        }


        public static string GetAvailableFilename(string path, string filename)
        {

            var fullPath = Path.Combine(path, filename);
            if (!System.IO.File.Exists(fullPath) && !string.IsNullOrEmpty(filename)) return fullPath;

            string alternateFilename;
            var fileNameIndex = NextIndex;
            do
            {
                fileNameIndex += NextIndex;
                alternateFilename = CreateNumberedFilename(filename, fileNameIndex);

                fullPath = Path.Combine(path, alternateFilename);
            } while (System.IO.File.Exists(fullPath));

            return fullPath;
        }

        private static string CreateNumberedFilename(string filename, uint number)
        {
            var plainName = System.IO.Path.GetFileNameWithoutExtension(filename);
            var extension = System.IO.Path.GetExtension(filename);
            return $"{plainName}{number}{extension}";
        }

        #endregion



    }
}
