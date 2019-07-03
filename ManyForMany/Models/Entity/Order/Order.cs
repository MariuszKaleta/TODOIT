using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using ManyForMany.Controller;
using ManyForMany.Model.File;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity.Order;
using ManyForMany.ViewModel.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MultiLanguage.Validation.Attributes;
using MvcHelper;
using MvcHelper.Entity;
using MvcHelper.Validation.Attributes;

namespace ManyForMany.Model.Entity.Ofert
{
    public class Order : IId<string>
    {
        private Order()
        {

        }

        public Order(CreateOrderViewModel model,  ApplicationUser owner, IQueryable<Skill> dataSkills)
        {//TODO Add Required Skills
            Title = model.Title;
            Describe = model.Describe;
            Owner = owner;
            CreateTime = DateTime.Now;
            DeadLine = model.DeadLine;
            Status = OrderStatus.CompleteTeam;
            
            RequiredSkills = new List<Skill>
            {
                {model.RequiredSkills, dataSkills}
            };

            GoodIfHave = new List<Skill>
            {
                {model.GoodIfHave, dataSkills}
            };
        }

        [Key]
        public string Id { get; private set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Describe { get; set; }

        [Required]
        public ApplicationUser Owner { get; private set; }

        //public int ProjectChatId { get; set; }

        [Required]
        public DateTime CreateTime { get; private set; }

        public DateTime DeadLine { get; set; }

        public OrderStatus Status { get; set; }

        public List<ApplicationUser> RejectedByUsers { get; private set; } 

        public List<ApplicationUser> InterestedByUsers { get; private set; }

        public List<ApplicationUser> ActualTeam { get; private set; }

        public List<Skill> RequiredSkills { get; private set; }

        public List<Skill> GoodIfHave { get; private set; }
    }

    public enum OrderStatus
    {
        CompleteTeam = 0,
        Suspend,
        InProgress,
        Completed,
        NotCompleted
    }

    public static class OrderExtension
    {
        public static async Task<Order> Get(this IQueryable<Order> users, string id, ILogger logger)
        {
            return await users.Get(id, Errors.OrderIsNotExistInList, logger);
        }

        public static ShowPublicOrderViewModel ToPublicInformation(this IQueryable<Order> orders,  string id, ILogger _logger, OrderFileManager orderFileManager)
        {
            return orders
                .Include(x => x.RequiredSkills)
                .Include(x => x.GoodIfHave)
                .Get(id, _logger).GetAwaiter()
                .GetResult().ToPublicInformation(orderFileManager);
        }
        public static IQueryable<ShowPublicOrderViewModel> ToPublicInformation(this IQueryable<Order> orders, OrderFileManager orderFileManager)
        {
            return orders
                .Include(x => x.RequiredSkills)
                .Include(x => x.GoodIfHave)
                .Select(x=>x.ToPublicInformation(orderFileManager));
        }

        public static ShowPublicOrderViewModel ToPublicInformation(this Order order, OrderFileManager orderFileManager)
        {
            return new ShowPublicOrderViewModel(order, orderFileManager);
        }

        public static async Task<File.File[]> Images(this Order order, OrderFileManager orderFileManager)
        {
            return await orderFileManager.DownladOrderImages(order.Owner.Id, order.Id);
        }
        public static async Task<File.File[]> Files(this Order order, OrderFileManager orderFileManager)
        {
            return await orderFileManager.DownladOrderFiles(order.Owner.Id, order.Id);
        }

        public static void Edit(this Order order, OrderViewModel model)
        {
            if (order.Title != model.Title)
            {
                order.Title = model.Title;
            }

            if (order.Describe != model.Describe)
            {
                order.Describe = model.Describe;
            }

            if (order.DeadLine != model.DeadLine)
            {
                order.DeadLine = model.DeadLine;
            }
        }


        public static void RemoveUserFromProject(this Order order, ApplicationUser user)
        {
            order.ActualTeam.Remove(user);
        }


        public static void Remove(this Order order, Context context)
        {
            context.Orders.Remove(order);
        }
    }
}
