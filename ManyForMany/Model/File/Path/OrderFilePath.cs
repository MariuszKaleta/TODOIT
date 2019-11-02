using System.Collections.Generic;
using FileHelper.Model;
using TODOIT.Model.Entity.Order;

namespace TODOIT.Model.File.Path
{
    public class OrderFilePath : LocalPathDirectoryProvider
    {

        public OrderFilePath(Order order)
        {
            _orderId = order.Id.ToString();
        }

        public OrderFilePath(string orderId)
        {
            _orderId = orderId;
        }

        public const string OrderDirectory = nameof(Order);
        public const string File = nameof(System.IO.File);

        private readonly string _orderId;


        public override IEnumerable<string> PathElements
        {
            get
            {
                yield return OrderDirectory;
                yield return _orderId;
                yield return File;
            }
        }
    }
}
