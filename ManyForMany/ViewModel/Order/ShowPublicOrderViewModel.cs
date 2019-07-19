using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Entity.Order;
using ManyForMany.Models.Entity.Skill;
using ManyForMany.Models.Entity.User;
using ManyForMany.Models.File;

namespace ManyForMany.ViewModel.Order
{
    public class ShowPublicOrderViewModel : OrderViewModel
    {
        public ShowPublicOrderViewModel(Models.Entity.Order.Order order, OrderFileManager orderFileManager)
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
            Categories = order.Categories.ToArray();
        }

        public string Id { get; set; }

        public ThumbnailUserViewModel Author { get; set; }

        public DateTime CreateTime { get; set; }

        public OrderStatus Status { get; set; }

        public File[] Images { get; set; }

        public File[] Files { get; set; }

        public List<Models.Entity.Skill.Skill> RequiredSkills { get; set; }

        public List<Models.Entity.Skill.Skill> GoodIfHave { get; set; }

        public Category[] Categories { get; set; }
    }
}
