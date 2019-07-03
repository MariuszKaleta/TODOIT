using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.File;
using ManyForMany.Models.Entity.Order;
using ManyForMany.ViewModel.User;
using MultiLanguage.Validation.Attributes;

namespace ManyForMany.ViewModel.Order
{
    public class ShowPublicOrderViewModel : OrderViewModel
    {
        public ShowPublicOrderViewModel(Model.Entity.Ofert.Order order, OrderFileManager orderFileManager)
        {
            Author = order.Owner.ToUserThumbnail();
            Id = order.Id;
            Title = order.Title;
            Describe = order.Describe;
            Images = order.Images(orderFileManager).Result;
            Files = order.Files(orderFileManager).Result;
            DeadLine = order.DeadLine;
            RequiredSkills = order.RequiredSkills;
            GoodIfHave = order.GoodIfHave;
            Status = order.Status;
            CreateTime = order.CreateTime;
        }

        public string Id { get; set; }

        public ThumbnailUserViewModel Author { get; set; }

        public DateTime CreateTime { get; set; }

        public OrderStatus Status { get; set; }

        public File[] Images { get; set; }

        public File[] Files { get; set; }

        public List<Skill> RequiredSkills { get; set; }

        public List<Skill> GoodIfHave { get; set; }


    }
}
