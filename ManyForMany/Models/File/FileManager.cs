using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManyForMany.Models.File
{
    public class FileManager
    {
        #region properties


        public static string Content { get; } = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameof(Content)));

        public static string LocalPath(params string[] directories) =>
            Path.Combine(Content,
                string.Join(Path.DirectorySeparatorChar,
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
            var directoryPath = LocalPath(directories);
            Directory.CreateDirectory(directoryPath);

            var tasks = files.Select(file => UploadFile(file, directoryPath));

            return (await Task.WhenAll(tasks)).All(x => x);
        }

        private async Task<bool> UploadFile(File file, string directoryPath)
        {
            var isCopied = false;
            //1 check if the file length is greater than 0 bytes 
            if (file.Data.Length > ushort.MinValue)
            {
                await file.Save(directoryPath);
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

        public async Task<File[]> DownloadFiles(params string[] directories)
        {
            var localPath = LocalPath(directories);
            return await DownloadFiles(localPath);
        }
        public async Task<File[]> DownloadFiles(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                var files = Directory.GetFiles(directoryPath);
                var tasks = files.Select(File.Load).ToArray();

                return await Task.WhenAll(tasks);
            }

            return Enumerable.Empty<File>().ToArray();
        }

        public void RemoveFiles(string[] directories, string[] fileNames)
        {
            var directory = LocalPath(directories);

            if (Directory.Exists(directory))
            {
                var directoryFiles = Directory.GetFiles(directory);

                foreach (var file in directoryFiles)
                {
                    if (fileNames.Any(x => file.Contains(x)))
                    {
                        RemoveFile(file);
                    }
                }
            }
        }

        public void RemoveFile(string fullFileName)
        {
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
        }

        public void RemoveFiles(string[] directories, string fileName)
        {
            var directory = LocalPath(directories);

            var directoryFiles = Directory.GetFiles(directory);

            foreach (var file in directoryFiles)
            {
                if (fileName.Any(x => file.Contains(x)))
                {
                    RemoveFile(file);
                }
            }
        }

        private static IEnumerable<uint> GetFileNames(string directoryPath)
        {
            return Directory.GetFiles(directoryPath).Select(Path.GetFileNameWithoutExtension)
                .Select(x => uint.TryParse(x, out var result) ? new uint?(result) : null).Where(x => x.HasValue).Select(x => x.Value);
        }

        #endregion
    }
}
