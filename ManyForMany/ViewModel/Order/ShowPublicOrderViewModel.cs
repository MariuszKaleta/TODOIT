using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.Entity.Orders;
using ManyForMany.Model.File;
using MultiLanguage.Validation.Attributes;

namespace ManyForMany.ViewModel.Order
{
    public class ShowPublicOrderViewModel : OrderViewModel
    {
        public ShowPublicOrderViewModel(Model.Entity.Ofert.Order order, ImageManager imageManager): base(order,imageManager)
        {
            CreateTime = order.CreateTime;
        }

        public DateTime CreateTime { get; set; }
    }
}
