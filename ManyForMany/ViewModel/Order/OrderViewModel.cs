using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.Entity.Orders;
using ManyForMany.Model.File;
using MultiLanguage.Validation.Attributes;
using MvcHelper.Validation.Attributes;

namespace ManyForMany.ViewModel.Order
{
    public class OrderViewModel
    {
        public OrderViewModel(Model.Entity.Ofert.Order order, ImageManager imageManager)
        {
            Title = order.Title;
            Describe = order.Describe;
            Images = order.Images(imageManager).Result;
            DeadLine = order.DeadLine;
        }

        protected OrderViewModel()
        {

        }


        [Required]
        public string Title { get; set; }

        [Required]
        public string Describe { get; set; }

        public Image[] Images { get; set; }

        [DateInRange(MustBeAfterToday = true)]
        public DateTime DeadLine { get; set; }
    }
}
