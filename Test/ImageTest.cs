using System;
using System.Collections.Generic;
using System.Text;
using ManyForMany.Models.File;
using ManyForMany.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class ImageTest
    {
        private static OrderFileManager _orderFileManager = new OrderFileManager();

        [TestMethod]
        public void Upload()
        {
            var file = new FileViewModel()
            {
                Data = "jpg",
                Extension = "a"
            };
            
            _orderFileManager.UploadOrderImages(file, "40ffb216-1d67-46b5-937e-0b6039fab2f9", 2.ToString()).GetAwaiter().GetResult();
        }
        [TestMethod]
        public void Download()
        {
          var result = _orderFileManager.DownladOrderImages( "40ffb216-1d67-46b5-937e-0b6039fab2f9", 2.ToString()).GetAwaiter().GetResult();

          Assert.AreEqual(result.Length, 1);
        }

    }
}
