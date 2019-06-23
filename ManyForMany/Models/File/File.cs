using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace ManyForMany.Model.File
{
    public class File
    {
        public string Extension { get; set; }

        [Required]
        public string Data { get; set; }

        public async Task Save(string path)
        {
            var fileCombine = Path.ChangeExtension(path, Extension);
            await System.IO.File.WriteAllTextAsync(fileCombine, Data);
        }

        public static async Task<T> Load<T>(string path)
        where T : File, new()
        {
            var extension = Path.GetExtension(path);
            var data = await System.IO.File.ReadAllTextAsync(path);

            return new T()
            {
                Data = data,
                Extension = extension
            };
        }
    }
}
