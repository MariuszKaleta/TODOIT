using System;
using System.IO;
using System.Threading.Tasks;
using FileHelper;
using ManyForMany.ViewModel;

namespace ManyForMany.Models.File
{
    public class File
    {

        public File(FileViewModel model )
        {
            Extension = model.Extension;
            Data = model.Data;
        }

        protected File()
        {

        }

        public string FileName => Path.ChangeExtension(Id,Extension);

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Extension { get; private set; }

        public string Data { get; set; }

        public async Task Save(string directoryPath)
        {
            var fileName = $"{directoryPath}{Path.DirectorySeparatorChar}{Id}";

            var fileNameWithExtension = Path.ChangeExtension(fileName, Extension);

            await System.IO.File.WriteAllTextAsync(fileNameWithExtension, Data);
        }

        public static async Task<File> Load(string path)
        {
            var dataTask = System.IO.File.ReadAllTextAsync(path);
            var extension = Path.GetExtension(path).Replace(FileConstant.ExtensionSeparator.ToString(), string.Empty);
            var fileName = Path.GetFileNameWithoutExtension(path);


            return new File()
            {
                Id = fileName,
                Extension = extension,
                Data = await dataTask
            };
        }
    }
}
