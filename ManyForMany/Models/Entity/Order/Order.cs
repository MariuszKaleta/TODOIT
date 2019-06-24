using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using ManyForMany.Controller;
using ManyForMany.Model.Entity.Orders;
using ManyForMany.Model.File;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity.Order;
using ManyForMany.ViewModel.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MultiLanguage.Validation.Attributes;
using MvcHelper.Entity;
using MvcHelper.Validation.Attributes;

namespace ManyForMany.Model.Entity.Ofert
{
    public class Order : IId<int>
    {
        private Order()
        {

        }

        public Order(CreateOrderViewModel model,   ApplicationUser owner)
        {//TODO Add Required Skills
            Title = model.Title;
            Describe = model.Describe;
            OwnerId = owner.Id;
            CreateTime = DateTime.Now;
            DeadLine = model.DeadLine;
            Status = OrderStatus.CompleteTeam;
        }

        [Key]
        public int Id { get; private set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Describe { get; set; }

        [Required]
        public string OwnerId { get; private set; }

        //public int ProjectChatId { get; set; }

        [Required]
        public DateTime CreateTime { get; private set; }

        public DateTime DeadLine { get; set; }

        public OrderStatus Status { get; set; }

        public List<ApplicationUser> InterestedUsers { get; private set; }

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
        public static async Task<Order> Get<T>(this IQueryable<Order> users, int id, ILogger<T> logger)
        {
            return await users.Get(id, Errors.OrderIsNotExistInList, logger);
        }
        public static Order Get<T>(this IEnumerable<Order> users, int id, ILogger<T> logger)
        {
            return users.Get(id, Errors.OrderIsNotExistInList, logger);
        }


        public static ShowPublicOrderViewModel ToPublicInformation(this Order order, ImageManager imageManager)
        {
            return new ShowPublicOrderViewModel(order, imageManager);
        }

        public static async Task<Image[]> Images(this Order order, ImageManager imageManager)
        {
            return await imageManager.DownladOrderImages(order.OwnerId, order.Id);
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
        }

        public static void AddUserToProject(this Order order, ApplicationUser user)
        {
            order.InterestedUsers.Remove(user);
            user.InterestedOrders.Remove(order);

            order.ActualTeam.Add(user);
            user.MemberOfOrders.Add(order);
        }
        public static void RemoveUserFromProject(this Order order, ApplicationUser user)
        {
            order.ActualTeam.Remove(user);
            user.MemberOfOrders.Remove(order);
        }


        public static void Remove(this Order order, Context context)
        {
            context.Orders.Remove(order);

            foreach (var user in order.ActualTeam)
            {
                user.MemberOfOrders.Remove(order);
            }

            foreach (var user in order.InterestedUsers)
            {
                user.InterestedOrders.Remove(order);
            }
        }
    }
}
