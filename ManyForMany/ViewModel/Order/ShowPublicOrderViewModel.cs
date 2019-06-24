using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.Entity.Orders;
using ManyForMany.Model.File;
using ManyForMany.Models.Entity.Order;
using MultiLanguage.Validation.Attributes;

namespace ManyForMany.ViewModel.Order
{
    public class ShowPublicOrderViewModel : OrderViewModel
    {
        public ShowPublicOrderViewModel(Model.Entity.Ofert.Order order, ImageManager imageManager)
        {
            Title = order.Title;
            Describe = order.Describe;
            Images = order.Images(imageManager).Result;
            DeadLine = order.DeadLine;
            RequiredSkills = order.RequiredSkills;
            GoodIfHave = order.GoodIfHave;

            CreateTime = order.CreateTime;
        }

        public DateTime CreateTime { get; set; }

        public List<Skill> RequiredSkills { get; set; }

        public List<Skill> GoodIfHave { get; set; }
    }
}
