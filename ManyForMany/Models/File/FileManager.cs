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

        public static string Content { get; } = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameof(Content)));

        public static string LocalPath(params string[] directories) =>
            Path.Combine(Content,
                string.Join(FileConstant.PathSeparator,
                    directories.Select(RemoveInvalidFileChars)));

        public static readonly string[] InvalidFileNameChars =
            Path.GetInvalidFileNameChars().Select(x => x.ToString()).ToArray();

        public static string RemoveInvalidFileChars(string text)
        {
            foreach (var invalidFileNameChar in InvalidFileNameChars)
            {
                text = text.Replace(invalidFileNameChar, string.Empty);
            }

            return text;
        }

        #endregion

        #region Method


        public async Task<bool> UploadFile(File[] files, params string[] directories)
        {
            var tasks = files.Select(file => UploadFile(file, directories));

            return (await Task.WhenAll(tasks)).All(x => x);
        }

        public async Task<bool> UploadFile(File file, string directoryPath)
        {
            var isCopied = false;
            //1 check if the file length is greater than 0 bytes 
            if (file.Data.Length > ushort.MinValue)
            {
                var fileName = string.Empty;

                var extension = file.Extension;

                await file.Save(Path.ChangeExtension(GetAvailableFilename(directoryPath, fileName), extension));
                isCopied = true;

            }

            return isCopied;
        }

        public async Task<bool> UploadFile(File file, params string[] directories)
        {
            var directoryPath = LocalPath(directories);
            Directory.CreateDirectory(directoryPath);

            return await UploadFile(file, directoryPath);
        }

        public async Task<T[]> DownloadFiles<T>(params string[] directories)
            where T : File, new()
        {
            var localPath = LocalPath(directories);
            return await DownloadFiles<T>(localPath);
        }
        public async Task<T[]> DownloadFiles<T>(string directoryPath)
        where T : File, new()
        {
            var files = Directory.GetFiles(directoryPath);
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
