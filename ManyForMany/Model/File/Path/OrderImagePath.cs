using System.Collections.Generic;
using FileHelper.Model;
using TODOIT.Model.Entity.Order;

namespace TODOIT.Model.File.Path
{
    public class OrderImagePath : LocalPathDirectoryProvider
    {

        public OrderImagePath(Order order)
        {
            _orderId = order.Id.ToString();
        }

        public OrderImagePath(string orderId)
        {
            _orderId = orderId;
        }

        public const string OrderDirectory = nameof(Order);
        public const string Image = nameof(Image);

        private readonly string _orderId;


        public override IEnumerable<string> PathElements
        {
            get
            {
                yield return OrderDirectory;
                yield return _orderId;
                yield return Image;
            }
        }
    }
}
