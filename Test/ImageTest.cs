using System;
using System.Collections.Generic;
using System.Text;
using ManyForMany.Model.Entity.Orders;
using ManyForMany.Model.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class ImageTest
    {
        private static ImageManager ImageManager = new ImageManager();

        [TestMethod]
        public void Upload()
        {
            ImageManager.UploadOrderImages(new Image()
            {
                Data = "jpg",
                Extension = "a"
            }, "40ffb216-1d67-46b5-937e-0b6039fab2f9", 2).GetAwaiter().GetResult();
        }
        [TestMethod]
        public void Download()
        {
          var result = ImageManager.DownladOrderImages( "40ffb216-1d67-46b5-937e-0b6039fab2f9", 2).GetAwaiter().GetResult();

          Assert.AreEqual(result.Length, 1);
        }

    }
}
