using System;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using ManyForMany.Model.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using File = System.IO.File;
using Image = ManyForMany.Model.Entity.Orders.Image;

namespace Test
{
    [TestClass]
    public class FileTest
    {
        public static string TestFile = $"{AppDomain.CurrentDomain.BaseDirectory}{nameof(TestFile)}";
        public static string ResultFile = Path.Combine(TestFile, nameof(ResultFile));


        public static string Test = "test";
        public static int OrderId = 1;
        public static ImageManager ImageManager = new ImageManager();

        [TestInitialize]
        public void Create()
        {
            Directory.CreateDirectory(ResultFile);
        }



        [TestMethod]
        public void UploadFile()
        {
            var files = Directory.GetFiles(TestFile);

            foreach (var file in files)
            {
                var base64 = Convert.ToBase64String(File.ReadAllBytes(file));

                var image = new Image
                {
                    Data = base64,
                    Extension = "jpeg"
                };

                ImageManager.UploadFile(image, Test, OrderId);
            }
        }

        [TestMethod]
        public async Task DownloadFile()
        {
            var images = await ImageManager.DownladFiles(Test, OrderId);

            var index = 0;
            foreach (var file in images)
            {
                var bytes = Convert.FromBase64String(file.Data);
                var path = Path.Combine(ResultFile, Path.ChangeExtension($"{index++}", file.Extension));
                File.WriteAllBytes(path, bytes);
            }
        }
    }
}
